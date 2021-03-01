using Xunit;
using RePacker.Buffers;
using System.Runtime.InteropServices;
using System.Linq;

namespace RePacker.Buffers.Tests
{
    public class BufferTests
    {
        (byte[] buf, Buffer buffer) MakeBuffer(int size)
        {
            var buf = new byte[size];

            return (buf, new Buffer(buf, 0));
        }

        [Fact]
        public void pop_wrong_type_too_large()
        {
            byte[] buf = new byte[sizeof(int)];
            Buffer buffer = new Buffer(buf, 0);

            int testVal = 100;
            buffer.Pack<int>(ref testVal);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.Unpack<ulong>(out ulong outVal));
        }

        [Fact]
        public void null_array_throws()
        {
            Assert.Throws<System.ArgumentNullException>(() => new Buffer(null));
        }

        [Fact]
        public void length_gives_active_elements_length()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(1024);

            int testVal = 10;

            for (int i = 0; i < 6; i++)
                buffer.Pack<int>(ref testVal);

            Assert.Equal(sizeof(int) * 6, buffer.Length());

            buffer.Unpack<int>(out int fromBuf);
            buffer.Unpack<int>(out fromBuf);
            buffer.Unpack<int>(out fromBuf);

            Assert.Equal(sizeof(int) * 3, buffer.Length());
        }

        [Fact]
        public void count_gives_total_size_used()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(1024);

            int testVal = 10;

            for (int i = 0; i < 6; i++)
                buffer.Pack<int>(ref testVal);

            Assert.Equal(sizeof(int) * 6, buffer.WriteCursor());

            buffer.Unpack<int>(out int fromBuf);
            buffer.Unpack<int>(out fromBuf);
            buffer.Unpack<int>(out fromBuf);

            Assert.Equal(sizeof(int) * 6, buffer.WriteCursor());
        }

        [Fact]
        public void cursor_gives_active_first_element_index()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(1024);

            int testVal = 10;

            for (int i = 0; i < 6; i++)
                buffer.Pack<int>(ref testVal);

            Assert.Equal(0, buffer.ReadCursor());

            buffer.Unpack<int>(out int fromBuf);
            buffer.Unpack<int>(out fromBuf);
            buffer.Unpack<int>(out fromBuf);

            Assert.Equal(sizeof(int) * 3, buffer.ReadCursor());
        }

        [Fact]
        public void flush_zeroes_out_used_memory()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(80);

            int testVal = 10;
            for (int i = 0; i < 10; i++)
                buffer.Pack<int>(ref testVal);

            buffer.Flush();

            for (int i = 0; i < 10; i++)
            {
                buffer.Unpack<int>(out int val);
                Assert.Equal(0, val);
            }
        }

        [Fact]
        public void flush_clears_cursor_and_offset()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(80);

            int testVal = 10;
            for (int i = 0; i < 10; i++)
                buffer.Pack<int>(ref testVal);

            buffer.Unpack<int>(out int _);
            buffer.Unpack<int>(out int _);
            buffer.Unpack<int>(out int _);

            buffer.Flush();

            Assert.Equal(0, buffer.ReadCursor());
            Assert.Equal(0, buffer.WriteCursor());
            Assert.Equal(0, buffer.Length());
        }

        [Fact]
        public void reset_clears_cursor_and_offset()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(80);

            int testVal = 10;
            for (int i = 0; i < 10; i++)
                buffer.Pack<int>(ref testVal);

            buffer.Unpack<int>(out int _);
            buffer.Unpack<int>(out int _);
            buffer.Unpack<int>(out int _);

            buffer.Reset();

            Assert.Equal(0, buffer.ReadCursor());
            Assert.Equal(0, buffer.WriteCursor());
            Assert.Equal(0, buffer.Length());
        }

        [Fact]
        public void get_array_gives_used_length()
        {
            var buffer = MakeBuffer(1024);

            int testVal = 10;

            buffer.buffer.Pack<int>(ref testVal);
            buffer.buffer.Pack<int>(ref testVal);
            buffer.buffer.Pack<int>(ref testVal);
            buffer.buffer.Pack<int>(ref testVal);
            buffer.buffer.Pack<int>(ref testVal);

            var arrayFromBuffer = buffer.buffer.Array;

            Assert.Equal(5 * sizeof(int), buffer.buffer.Length());
        }

        [Fact]
        public void can_fit_should_succeed()
        {
            var buffer = MakeBuffer(4);
            Assert.True(buffer.buffer.CanFitBytes(sizeof(int)));
        }

        [Fact]
        public void can_fit_should_fail()
        {
            var buffer = MakeBuffer(4);
            Assert.False(buffer.buffer.CanFitBytes(sizeof(ulong)));
        }

        [Fact]
        public void can_fit_generic_unmanaged_should_succeed()
        {
            var buffer = MakeBuffer(4);
            Assert.True(buffer.buffer.CanFit<int>(1));
        }

        [Fact]
        public void can_fit_generic_unmanaged_should_fail()
        {
            var buffer = MakeBuffer(4);
            Assert.False(buffer.buffer.CanFit<ulong>(1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1234)]
        public void push_int_pop_int_1(int data)
        {
            byte[] buf = new byte[sizeof(int)];
            Buffer buffer = new Buffer(buf, 0);

            buffer.Pack<int>(ref data);

            buffer.Unpack<int>(out int fromBuf);

            Assert.Equal(data, fromBuf);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1234)]
        public void push_int_pop_int_100(int data)
        {
            byte[] buf = new byte[sizeof(int) * 100];
            Buffer buffer = new Buffer(buf, 0);

            for (int i = 0; i < 100; i++)
                buffer.Pack<int>(ref data);

            for (int i = 0; i < 100; i++)
            {
                buffer.Unpack<int>(out int fromBuf);
                Assert.Equal(data, fromBuf);
            }
        }

        [Theory]
        [InlineData(1.2345f)]
        [InlineData(12.345f)]
        [InlineData(123.45f)]
        [InlineData(1234.5f)]
        public void push_float_pop_float_1(float data)
        {
            byte[] buf = new byte[sizeof(float)];
            Buffer buffer = new Buffer(buf, 0);

            buffer.Pack<float>(ref data);

            buffer.Unpack<float>(out float fromBuf);

            Assert.Equal(data, fromBuf);
        }

        [Theory]
        [InlineData(1.2345f)]
        [InlineData(12.345f)]
        [InlineData(123.45f)]
        [InlineData(1234.5f)]
        public void push_float_pop_float_100(float data)
        {
            byte[] buf = new byte[sizeof(float) * 100];
            Buffer buffer = new Buffer(buf, 0);

            for (float i = 0; i < 100; i++)
                buffer.Pack<float>(ref data);

            for (float i = 0; i < 100; i++)
            {
                buffer.Unpack<float>(out float fromBuf);
                Assert.Equal(data, fromBuf);
            }
        }

        public struct TestBlittableStruct
        {
            public float Float;
            public int Int;
        }

        [Fact]
        public void push_and_pop_blittable_struct()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(80);

            var testStruct = new TestBlittableStruct { Float = 1.234f, Int = 1234 };

            buffer.Pack(ref testStruct);

            buffer.Unpack<TestBlittableStruct>(out var fromBuf);

            Assert.Equal(testStruct.Float, fromBuf.Float);
            Assert.Equal(testStruct.Int, fromBuf.Int);
        }

        [Fact]
        public void push_and_pop_blittable_struct_100()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(800);

            var testStruct = new TestBlittableStruct { Float = 1.234f, Int = 1234 };

            for (int i = 0; i < 100; i++)
            {
                buffer.Pack(ref testStruct);
            }

            for (int i = 0; i < 100; i++)
            {
                buffer.Unpack<TestBlittableStruct>(out var fromBuf);
                Assert.Equal(testStruct.Float, fromBuf.Float);
                Assert.Equal(testStruct.Int, fromBuf.Int);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
        public struct TestPaddingStruct
        {
            public ushort TwoBytes;
            public byte OneByte;
        }

        [Fact]
        public void struct_with_pack_1_has_no_padding()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(800);

            var testStruct = new TestPaddingStruct { TwoBytes = 1234, OneByte = 128 };

            buffer.Pack(ref testStruct);

            Assert.Equal(3, buffer.WriteCursor());
        }

        #region DirectPacking
        [Fact]
        public void direct_packing_char()
        {
            var buffer = new Buffer(new byte[2]);

            char value = 'G';

            buffer.PushChar(ref value);
            buffer.PopChar(out char posFromBuf);
            Assert.Equal(value, posFromBuf);
        }

        [Fact]
        public void direct_packing_char_out_of_bounds()
        {
            var buffer = new Buffer(new byte[3]);

            char value = 'G';

            buffer.PushChar(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PushChar(ref value));
        }

        [Fact]
        public void direct_packing_chars()
        {
            var buffer = new Buffer(new byte[2 * 256]);

            for (int i = 0; i < 255; i++)
            {
                char val = (char)i;
                buffer.PushChar(ref val);
            }

            for (int i = 0; i < 255; i++)
            {
                buffer.PopChar(out char posFromBuf);
                Assert.Equal((char)i, posFromBuf);
            }
        }

        [Fact]
        public void direct_packing_bool()
        {
            var buffer = new Buffer(new byte[1]);

            bool value = true;

            buffer.PushBool(ref value);
            buffer.PopBool(out bool posFromBuf);
            Assert.Equal(value, posFromBuf);
        }

        [Fact]
        public void direct_packing_bool_out_of_bounds()
        {
            var buffer = new Buffer(new byte[2]);

            bool value = true;

            buffer.PushBool(ref value);
            buffer.PushBool(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PushBool(ref value));
        }

        [Fact]
        public void direct_packing_short()
        {
            var buffer = new Buffer(new byte[4]);

            short positive = 23423;
            short negative = -5342;

            buffer.PushShort(ref positive);
            buffer.PopShort(out short posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.Reset();

            buffer.PushShort(ref negative);
            buffer.PopShort(out short negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_ushort()
        {
            var buffer = new Buffer(new byte[4]);

            ushort positive = 23423;

            buffer.PushUShort(ref positive);
            buffer.PopUShort(out ushort posFromBuf);
            Assert.Equal(positive, posFromBuf);
        }

        [Fact]
        public void direct_packing_ushort_out_of_bounds()
        {
            var buffer = new Buffer(new byte[3]);

            ushort value = 'G';

            buffer.PushUShort(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PushUShort(ref value));
        }

        [Fact]
        public void direct_packing_int()
        {
            var buffer = new Buffer(new byte[4]);

            int positive = 1234567890;
            int negative = -1234567890;

            buffer.PushInt(ref positive);
            buffer.PopInt(out int posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.Reset();

            buffer.PushInt(ref negative);
            buffer.PopInt(out int negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_ints()
        {
            var buffer = new Buffer(new byte[1 << 16]);

            for (int i = int.MinValue; i < int.MaxValue - 100_000_000; i += 100_000_000)
            {
                buffer.PushInt(ref i);
            }

            for (int i = int.MinValue; i < int.MaxValue - 100_000_000; i += 100_000_000)
            {
                buffer.PopInt(out int val);
                Assert.Equal(i, val);
            }
        }

        [Fact]
        public void direct_packing_int_out_of_bounds()
        {
            var buffer = new Buffer(new byte[7]);

            int value = 'G';

            buffer.PushInt(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PushInt(ref value));
        }

        [Fact]
        public void direct_packing_uint()
        {
            var buffer = new Buffer(new byte[1024]);

            uint positive = 1234567890;
            buffer.PushUInt(ref positive);
            buffer.PopUInt(out uint posFromBuf);
            Assert.Equal(positive, posFromBuf);
        }

        [Fact]
        public void direct_packing_uint_out_of_bounds()
        {
            var buffer = new Buffer(new byte[7]);

            uint value = 'G';

            buffer.PushUInt(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PushUInt(ref value));
        }

        [Fact]
        public void direct_packing_long()
        {
            var buffer = new Buffer(new byte[1024]);

            long positive = 12345678901234123;
            long negative = -1234567890123435;

            buffer.PushLong(ref positive);
            buffer.PopLong(out long posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.PushLong(ref negative);
            buffer.PopLong(out long negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_long_out_of_bounds()
        {
            var buffer = new Buffer(new byte[15]);

            long value = 'G';

            buffer.PushLong(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PushLong(ref value));
        }

        [Fact]
        public void direct_packing_ulong()
        {
            var buffer = new Buffer(new byte[1024]);

            ulong positive = 12345678902345243623;

            buffer.PushULong(ref positive);
            buffer.PopULong(out ulong posFromBuf);
            Assert.Equal(positive, posFromBuf);
        }

        [Fact]
        public void direct_packing_ulong_out_of_bounds()
        {
            var buffer = new Buffer(new byte[15]);

            ulong value = 'G';

            buffer.PushULong(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PushULong(ref value));
        }


        [Fact]
        public void direct_packing_float()
        {
            var buffer = new Buffer(new byte[4]);

            float positive = 3.141598f;
            float negative = -6.2826f;

            buffer.PushFloat(ref positive);
            buffer.PopFloat(out float posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.Reset();

            buffer.PushFloat(ref negative);
            buffer.PopFloat(out float negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_floats()
        {
            var buffer = new Buffer(new byte[2048]);

            float val = 2_000_000_000f;
            float step = 2_000_000_000f / 128f;

            for (float f = -val; f < val; f += step)
            {
                buffer.PushFloat(ref f);
            }

            for (float f = -val; f < val; f += step)
            {
                buffer.PopFloat(out float posFromBuf);

                Assert.Equal(f, posFromBuf);
            }
        }

        [Fact]
        public void direct_packing_float_out_of_bounds()
        {
            var buffer = new Buffer(new byte[7]);

            float value = 'G';

            buffer.PushFloat(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PushFloat(ref value));
        }

        [Fact]
        public void direct_packing_double()
        {
            var buffer = new Buffer(new byte[8]);

            double positive = 3.1415989283475918234120012341276357816438762142736507821634783617856417856812736481723645781236481237657619871263497812635871236;
            double negative = -6.28269283475918234120012341276357816438762142736507821634783617856417856812736481723645781236481237657619871263497812635871236;

            buffer.PushDouble(ref positive);
            buffer.PopDouble(out double posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.Reset();

            buffer.PushDouble(ref negative);
            buffer.PopDouble(out double negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_doubles()
        {
            var buffer = new Buffer(new byte[2048]);

            double val = 2_000_000_000.0;
            double step = 2_000_000_000.0 / 128.0;

            for (double f = -val; f < val; f += step)
            {
                buffer.PushDouble(ref f);
            }

            for (double f = -val; f < val; f += step)
            {
                buffer.PopDouble(out double posFromBuf);

                Assert.Equal(f, posFromBuf);
            }
        }

        [Fact]
        public void direct_packing_double_out_of_bounds()
        {
            var buffer = new Buffer(new byte[15]);

            double value = 'G';

            buffer.PushDouble(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PushDouble(ref value));
        }

        [Fact]
        public void direct_packing_decimal()
        {
            var buffer = new Buffer(new byte[24]);

            decimal positive = 3.141598234523423452345234623455423762345234565346234544236234554264235423526243523452562345234536234523462345243623452364564567456786784567845624356568234623455324521343432435234532234523452346354665344523542354326234576562345234541890123457812348913489071345789012347890123483458972345912357983477659127481275913480712893471298579485723894719823419234712973598234758932075981327948718943567239485128934579843769283745918798523798234596832495874390857190873401298567129034697123456134784239889123471289356102734982316523648712638571293845M;
            decimal negative = -6.28476345623423452345234623455423762345234565346234544236234554264235423526243523452562345234536234523462345243623452364564567456786784567845624356568234623455342525624523452345623234523452346354665344523542354326234545345236234521890123457812348913489071345789012347890123483458972345912357983477659127481275913480712893471298579485723894719823419234712973598234758932075981327948718943567239485128934579843769283745918798523798234596832495874390857190873401298567129034697123456134784239889123471289356102734982316523648712638571293846M;

            buffer.PushDecimal(ref positive);
            buffer.PopDecimal(out decimal posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.Reset();

            buffer.PushDecimal(ref negative);
            buffer.PopDecimal(out decimal negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_decimal_out_of_bounds()
        {
            var buffer = new Buffer(new byte[47]);

            decimal value = 'G';

            buffer.PushDecimal(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PushDecimal(ref value));
        }
        #endregion

        #region Array
        [Fact]
        public void direct_packing_int_array()
        {
            var buffer = new Buffer(new byte[sizeof(int) * 101 + sizeof(ulong)]);
            var data = Enumerable.Range(1, 100).ToArray();

            buffer.MemoryCopyFromUnsafe(data);
            var fromBuf = buffer.MemoryCopyToUnsafe<int>();

            for (int i = 0; i < 100; i++)
            {
                Assert.Equal(data[i], fromBuf[i]);
            }
        }

        [Fact]
        public void direct_packing_int_array_out_of_bounds()
        {
            var buffer = new Buffer(new byte[sizeof(int) * 100 + sizeof(ulong)]);
            var data = Enumerable.Range(1, 100).ToArray();

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.MemoryCopyFromUnsafe(data));
        }

        T[] GenerateArray<T>(int length, System.Func<int, T> generator) where T : unmanaged
        {
            var array = new T[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = generator.Invoke(i);
            }

            return array;
        }

        [Fact]
        public void direct_pack_array_multiple()
        {
            var buffer = new Buffer(new byte[1 << 16]);

            var bytes = GenerateArray<byte>(32, i => (byte)(i % 255));
            var shorts = GenerateArray<short>(32, i => (short)(i % 255));
            var ints = GenerateArray<int>(32, i => (int)(i % 255));
            var longs = GenerateArray<long>(32, i => (long)(i % 255));
            var floats = GenerateArray<float>(32, i => (float)(i % 255));

            buffer.MemoryCopyFromUnsafe(bytes);
            buffer.MemoryCopyFromUnsafe(shorts);
            buffer.MemoryCopyFromUnsafe(ints);
            buffer.MemoryCopyFromUnsafe(longs);
            buffer.MemoryCopyFromUnsafe(floats);

            var bytesFromBuf = buffer.MemoryCopyToUnsafe<byte>();
            var shortsFromBuf = buffer.MemoryCopyToUnsafe<short>();
            var intsFromBuf = buffer.MemoryCopyToUnsafe<int>();
            var longsFromBuf = buffer.MemoryCopyToUnsafe<long>();
            var floatsFromBuf = buffer.MemoryCopyToUnsafe<float>();

            Assert.Equal(32, bytesFromBuf.Length);
            Assert.Equal(32, shortsFromBuf.Length);
            Assert.Equal(32, intsFromBuf.Length);
            Assert.Equal(32, longsFromBuf.Length);
            Assert.Equal(32, floatsFromBuf.Length);

            for (int i = 0; i < 32; i++)
            {
                Assert.Equal(bytes[i], bytesFromBuf[i]);
                Assert.Equal(shorts[i], shortsFromBuf[i]);
                Assert.Equal(ints[i], intsFromBuf[i]);
                Assert.Equal(longs[i], longsFromBuf[i]);
                Assert.Equal(floats[i], floatsFromBuf[i]);
            }
        }
        #endregion

        [Fact]
        public void moving_write_cursor()
        {
            var buffer = new Buffer(new byte[10]);

            buffer.MoveWriteCursor(9);

            Assert.Equal(buffer.WriteCursor(), 9);
        }

        [Fact]
        public void moving_write_cursor_throws()
        {
            var buffer = new Buffer(new byte[10]);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.MoveWriteCursor(11));
        }

        [Fact]
        public void moving_read_cursor()
        {
            var buffer = new Buffer(new byte[10]);

            buffer.MoveReadCursor(9);

            Assert.Equal(buffer.ReadCursor(), 9);
        }

        [Fact]
        public void moving_read_cursor_throws()
        {
            var buffer = new Buffer(new byte[10]);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.MoveReadCursor(11));
        }

        [Fact]
        public void can_fit_types()
        {
            var buffer = new Buffer(new byte[24]);

            Assert.True(buffer.CanFit<short>(12));
            Assert.True(buffer.CanFit<int>(6));
            Assert.True(buffer.CanFit<long>(3));
            Assert.True(buffer.CanFit<decimal>());

            Assert.False(buffer.CanFit<short>(13));
            Assert.False(buffer.CanFit<int>(7));
            Assert.False(buffer.CanFit<long>(4));
            Assert.False(buffer.CanFit<decimal>(2));
        }

        [Fact]
        public void can_fit_bytes()
        {
            var buffer = new Buffer(new byte[24]);

            Assert.True(buffer.CanFitBytes(24));
            Assert.False(buffer.CanFitBytes(25));
        }

        [Fact]
        public void can_copy_buffer()
        {
            var buffer1 = new Buffer(new byte[1024]);
            var buffer2 = new Buffer(new byte[1024]);

            for (int i = 0; i < 10; i++)
            {
                buffer1.Pack(ref i);
            }

            buffer1.Copy(ref buffer2);

            for (int i = 0; i < 10; i++)
            {
                buffer1.Unpack<int>(out int b1);
                buffer2.Unpack<int>(out int b2);

                Assert.Equal(b1, b2);
            }
        }

        [Fact]
        public void copy_buffer_out_of_bounds()
        {
            var buffer1 = new Buffer(new byte[1024]);
            var buffer2 = new Buffer(new byte[8]);

            for (int i = 0; i < 10; i++)
            {
                buffer1.Pack(ref i);
            }

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer1.Copy(ref buffer2));
        }
    }
}

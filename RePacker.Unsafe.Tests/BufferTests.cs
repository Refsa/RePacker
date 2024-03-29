using Xunit;
using RePacker.Buffers;
using RePacker.Buffers.Extra;
using System.Runtime.InteropServices;
using System.Linq;

namespace RePacker.Buffers.Tests
{
    public class BufferTests
    {
        (byte[] buf, ReBuffer buffer) MakeBuffer(int size)
        {
            var buf = new byte[size];

            return (buf, new ReBuffer(buf, 0));
        }

        [Fact]
        public void pop_wrong_type_too_large()
        {
            byte[] buf = new byte[sizeof(int)];
            var buffer = new ReBuffer(buf, 0);

            int testVal = 100;
            buffer.Pack<int>(ref testVal);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.Unpack<ulong>(out ulong outVal));
        }

        [Fact]
        public void null_array_throws()
        {
            Assert.Throws<System.ArgumentNullException>(() => new ReBuffer(null));
        }

        [Fact]
        public void length_gives_active_elements_length()
        {
            (byte[] buf, ReBuffer buffer) = MakeBuffer(1024);

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
            (byte[] buf, ReBuffer buffer) = MakeBuffer(1024);

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
            (byte[] buf, ReBuffer buffer) = MakeBuffer(1024);

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
            (byte[] buf, ReBuffer buffer) = MakeBuffer(80);

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
            (byte[] buf, ReBuffer buffer) = MakeBuffer(80);

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
            (byte[] buf, ReBuffer buffer) = MakeBuffer(80);

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

        // TODO: This test does nothing
        [Fact]
        public void get_array_gives_used_length()
        {
            var buffer = MakeBuffer(1024).buffer;

            int testVal = 10;

            buffer.Pack<int>(ref testVal);
            buffer.Pack<int>(ref testVal);
            buffer.Pack<int>(ref testVal);
            buffer.Pack<int>(ref testVal);
            buffer.Pack<int>(ref testVal);

            var arrayFromBuffer = buffer.Array;

            Assert.Equal(5 * sizeof(int), buffer.Length());
        }

        [Fact]
        public void can_fit_should_succeed()
        {
            var buffer = MakeBuffer(4).buffer;
            Assert.True(buffer.CanWriteBytes(sizeof(int)));
        }

        [Fact]
        public void can_fit_should_fail()
        {
            var buffer = MakeBuffer(4).buffer;
            Assert.False(buffer.CanWriteBytes(sizeof(ulong)));
        }

        [Fact]
        public void can_fit_generic_unmanaged_should_succeed()
        {
            var buffer = MakeBuffer(4).buffer;
            Assert.True(buffer.CanWrite<int>(1));
        }

        [Fact]
        public void can_fit_generic_unmanaged_should_fail()
        {
            var buffer = MakeBuffer(4).buffer;
            Assert.False(buffer.CanWrite<ulong>(1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1234)]
        public void push_int_pop_int_1(int data)
        {
            byte[] buf = new byte[sizeof(int)];
            var buffer = new ReBuffer(buf, 0);

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
            var buffer = new ReBuffer(buf, 0);

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
            var buffer = new ReBuffer(buf, 0);

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
            var buffer = new ReBuffer(buf, 0);

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
            (byte[] buf, ReBuffer buffer) = MakeBuffer(80);

            var testStruct = new TestBlittableStruct { Float = 1.234f, Int = 1234 };

            buffer.Pack(ref testStruct);

            buffer.Unpack<TestBlittableStruct>(out var fromBuf);

            Assert.Equal(testStruct.Float, fromBuf.Float);
            Assert.Equal(testStruct.Int, fromBuf.Int);
        }

        [Fact]
        public void push_and_pop_blittable_struct_100()
        {
            (byte[] buf, ReBuffer buffer) = MakeBuffer(800);

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
            (byte[] buf, ReBuffer buffer) = MakeBuffer(800);

            var testStruct = new TestPaddingStruct { TwoBytes = 1234, OneByte = 128 };

            buffer.Pack(ref testStruct);

            Assert.Equal(3, buffer.WriteCursor());
        }

        #region DirectPacking
        [Fact]
        public void direct_packing_char()
        {
            var buffer = new ReBuffer(new byte[2]);

            char value = 'G';

            buffer.PackChar(ref value);
            buffer.UnpackChar(out char posFromBuf);
            Assert.Equal(value, posFromBuf);
        }

        [Fact]
        public void direct_packing_char_out_of_bounds()
        {
            var buffer = new ReBuffer(new byte[3]);

            char value = 'G';

            buffer.PackChar(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackChar(ref value));
        }

        [Fact]
        public void direct_packing_chars()
        {
            var buffer = new ReBuffer(new byte[2 * 256]);

            for (int i = 0; i < 255; i++)
            {
                char val = (char)i;
                buffer.PackChar(ref val);
            }

            for (int i = 0; i < 255; i++)
            {
                buffer.UnpackChar(out char posFromBuf);
                Assert.Equal((char)i, posFromBuf);
            }
        }

        [Fact]
        public void direct_packing_bool()
        {
            var buffer = new ReBuffer(new byte[1]);

            bool value = true;

            buffer.PackBool(ref value);
            buffer.UnpackBool(out bool posFromBuf);
            Assert.Equal(value, posFromBuf);
        }

        [Fact]
        public void direct_packing_bool_out_of_bounds()
        {
            var buffer = new ReBuffer(new byte[2]);

            bool value = true;

            buffer.PackBool(ref value);
            buffer.PackBool(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackBool(ref value));
        }

        [Fact]
        public void direct_packing_byte()
        {
            var buffer = new ReBuffer(4);
            byte value = 128;

            buffer.PackByte(ref value);
            buffer.UnpackByte(out byte fromBuf);

            Assert.Equal(value, fromBuf);
        }

        [Fact]
        public void direct_packing_sbyte()
        {
            var buffer = new ReBuffer(4);
            sbyte pos = 127;
            sbyte neg = -127;

            buffer.PackSByte(ref pos);
            buffer.PackSByte(ref neg);

            buffer.UnpackSByte(out sbyte fromBuf);
            Assert.Equal(pos, fromBuf);

            buffer.UnpackSByte(out fromBuf);
            Assert.Equal(neg, fromBuf);
        }

        [Fact]
        public void direct_packing_short()
        {
            var buffer = new ReBuffer(new byte[4]);

            short positive = 23423;
            short negative = -5342;

            buffer.PackShort(ref positive);
            buffer.UnpackShort(out short posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.Reset();

            buffer.PackShort(ref negative);
            buffer.UnpackShort(out short negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_ushort()
        {
            var buffer = new ReBuffer(new byte[4]);

            ushort positive = 23423;

            buffer.PackUShort(ref positive);
            buffer.UnpackUShort(out ushort posFromBuf);
            Assert.Equal(positive, posFromBuf);
        }

        [Fact]
        public void direct_packing_ushort_out_of_bounds()
        {
            var buffer = new ReBuffer(new byte[3]);

            ushort value = 'G';

            buffer.PackUShort(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackUShort(ref value));
        }

        [Fact]
        public void direct_packing_int()
        {
            var buffer = new ReBuffer(new byte[4]);

            int positive = 1234567890;
            int negative = -1234567890;

            buffer.PackInt(ref positive);
            buffer.UnpackInt(out int posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.Reset();

            buffer.PackInt(ref negative);
            buffer.UnpackInt(out int negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_ints()
        {
            var buffer = new ReBuffer(new byte[1 << 16]);

            for (int i = int.MinValue; i < int.MaxValue - 100_000_000; i += 100_000_000)
            {
                buffer.PackInt(ref i);
            }

            for (int i = int.MinValue; i < int.MaxValue - 100_000_000; i += 100_000_000)
            {
                buffer.UnpackInt(out int val);
                Assert.Equal(i, val);
            }
        }

        [Fact]
        public void direct_packing_int_out_of_bounds()
        {
            var buffer = new ReBuffer(new byte[7]);

            int value = 'G';

            buffer.PackInt(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackInt(ref value));
        }

        [Fact]
        public void direct_packing_uint()
        {
            var buffer = new ReBuffer(new byte[1024]);

            uint positive = 1234567890;
            buffer.PackUInt(ref positive);
            buffer.UnpackUInt(out uint posFromBuf);
            Assert.Equal(positive, posFromBuf);
        }

        [Fact]
        public void direct_packing_uint_out_of_bounds()
        {
            var buffer = new ReBuffer(new byte[7]);

            uint value = 'G';

            buffer.PackUInt(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackUInt(ref value));
        }

        [Fact]
        public void direct_packing_long()
        {
            var buffer = new ReBuffer(new byte[1024]);

            long positive = 12345678901234123;
            long negative = -1234567890123435;

            buffer.PackLong(ref positive);
            buffer.UnpackLong(out long posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.PackLong(ref negative);
            buffer.UnpackLong(out long negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_long_out_of_bounds()
        {
            var buffer = new ReBuffer(new byte[15]);

            long value = 'G';

            buffer.PackLong(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackLong(ref value));
        }

        [Fact]
        public void direct_packing_ulong()
        {
            var buffer = new ReBuffer(new byte[1024]);

            ulong positive = 12345678902345243623;

            buffer.PackULong(ref positive);
            buffer.UnpackULong(out ulong posFromBuf);
            Assert.Equal(positive, posFromBuf);
        }

        [Fact]
        public void direct_packing_ulong_out_of_bounds()
        {
            var buffer = new ReBuffer(new byte[15]);

            ulong value = 'G';

            buffer.PackULong(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackULong(ref value));
        }


        [Fact]
        public void direct_packing_float()
        {
            var buffer = new ReBuffer(new byte[4]);

            float positive = 3.141598f;
            float negative = -6.2826f;

            buffer.PackFloat(ref positive);
            buffer.UnpackFloat(out float posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.Reset();

            buffer.PackFloat(ref negative);
            buffer.UnpackFloat(out float negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_floats()
        {
            var buffer = new ReBuffer(new byte[2048]);

            float val = 2_000_000_000f;
            float step = 2_000_000_000f / 128f;

            for (float f = -val; f < val; f += step)
            {
                buffer.PackFloat(ref f);
            }

            for (float f = -val; f < val; f += step)
            {
                buffer.UnpackFloat(out float posFromBuf);

                Assert.Equal(f, posFromBuf);
            }
        }

        [Fact]
        public void direct_packing_float_out_of_bounds()
        {
            var buffer = new ReBuffer(new byte[7]);

            float value = 'G';

            buffer.PackFloat(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackFloat(ref value));
        }

        [Fact]
        public void direct_packing_double()
        {
            var buffer = new ReBuffer(new byte[8]);

            double positive = 3.1415989283475918234120012341276357816438762142736507821634783617856417856812736481723645781236481237657619871263497812635871236;
            double negative = -6.28269283475918234120012341276357816438762142736507821634783617856417856812736481723645781236481237657619871263497812635871236;

            buffer.PackDouble(ref positive);
            buffer.UnpackDouble(out double posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.Reset();

            buffer.PackDouble(ref negative);
            buffer.UnpackDouble(out double negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_doubles()
        {
            var buffer = new ReBuffer(new byte[2048]);

            double val = 2_000_000_000.0;
            double step = 2_000_000_000.0 / 128.0;

            for (double f = -val; f < val; f += step)
            {
                buffer.PackDouble(ref f);
            }

            for (double f = -val; f < val; f += step)
            {
                buffer.UnpackDouble(out double posFromBuf);

                Assert.Equal(f, posFromBuf);
            }
        }

        [Fact]
        public void direct_packing_double_out_of_bounds()
        {
            var buffer = new ReBuffer(new byte[15]);

            double value = 'G';

            buffer.PackDouble(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackDouble(ref value));
        }

        [Fact]
        public void direct_packing_decimal()
        {
            var buffer = new ReBuffer(new byte[24]);

            decimal positive = 3.141598234523423452345234623455423762345234565346234544236234554264235423526243523452562345234536234523462345243623452364564567456786784567845624356568234623455324521343432435234532234523452346354665344523542354326234576562345234541890123457812348913489071345789012347890123483458972345912357983477659127481275913480712893471298579485723894719823419234712973598234758932075981327948718943567239485128934579843769283745918798523798234596832495874390857190873401298567129034697123456134784239889123471289356102734982316523648712638571293845M;
            decimal negative = -6.28476345623423452345234623455423762345234565346234544236234554264235423526243523452562345234536234523462345243623452364564567456786784567845624356568234623455342525624523452345623234523452346354665344523542354326234545345236234521890123457812348913489071345789012347890123483458972345912357983477659127481275913480712893471298579485723894719823419234712973598234758932075981327948718943567239485128934579843769283745918798523798234596832495874390857190873401298567129034697123456134784239889123471289356102734982316523648712638571293846M;

            buffer.PackDecimal(ref positive);
            buffer.UnpackDecimal(out decimal posFromBuf);
            Assert.Equal(positive, posFromBuf);

            buffer.Reset();

            buffer.PackDecimal(ref negative);
            buffer.UnpackDecimal(out decimal negFromBuf);
            Assert.Equal(negative, negFromBuf);
        }

        [Fact]
        public void direct_packing_decimal_out_of_bounds()
        {
            var buffer = new ReBuffer(new byte[47]);

            decimal value = 'G';

            buffer.PackDecimal(ref value);
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackDecimal(ref value));
        }
        #endregion

        #region Array
        [Fact]
        public void direct_packing_int_array()
        {
            var buffer = new ReBuffer(new byte[sizeof(int) * 101 + sizeof(ulong)]);
            var data = Enumerable.Range(1, 100).ToArray();

            buffer.PackArray(data);
            var fromBuf = buffer.UnpackArray<int>();

            for (int i = 0; i < 100; i++)
            {
                Assert.Equal(data[i], fromBuf[i]);
            }
        }

        [Fact]
        public void direct_packing_int_array_out_of_bounds()
        {
            var buffer = new ReBuffer(new byte[sizeof(int) * 100 + sizeof(ulong)]);
            var data = Enumerable.Range(1, 101).ToArray();

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackArray(data));
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
            var buffer = new ReBuffer(new byte[1 << 16]);

            var bytes = GenerateArray<byte>(32, i => (byte)(i % 255));
            var shorts = GenerateArray<short>(32, i => (short)(i % 255));
            var ints = GenerateArray<int>(32, i => (int)(i % 255));
            var longs = GenerateArray<long>(32, i => (long)(i % 255));
            var floats = GenerateArray<float>(32, i => (float)(i % 255));

            buffer.PackArray(bytes);
            buffer.PackArray(shorts);
            buffer.PackArray(ints);
            buffer.PackArray(longs);
            buffer.PackArray(floats);

            var bytesFromBuf = buffer.UnpackArray<byte>();
            var shortsFromBuf = buffer.UnpackArray<short>();
            var intsFromBuf = buffer.UnpackArray<int>();
            var longsFromBuf = buffer.UnpackArray<long>();
            var floatsFromBuf = buffer.UnpackArray<float>();

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
            var buffer = new ReBuffer(new byte[10]);

            buffer.MoveWriteCursor(9);

            Assert.Equal(9, buffer.WriteCursor());
        }

        [Fact]
        public void moving_write_cursor_throws()
        {
            var buffer = new ReBuffer(new byte[10]);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.MoveWriteCursor(11));
        }

        [Fact]
        public void moving_read_cursor()
        {
            var buffer = new ReBuffer(new byte[10]);

            buffer.MoveReadCursor(9);

            Assert.Equal(9, buffer.ReadCursor());
        }

        [Fact]
        public void moving_read_cursor_throws()
        {
            var buffer = new ReBuffer(new byte[10]);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.MoveReadCursor(11));
        }

        [Fact]
        public void can_fit_types()
        {
            var buffer = new ReBuffer(new byte[24]);

            Assert.True(buffer.CanWrite<short>(12));
            Assert.True(buffer.CanWrite<int>(6));
            Assert.True(buffer.CanWrite<long>(3));
            Assert.True(buffer.CanWrite<decimal>());

            Assert.False(buffer.CanWrite<short>(13));
            Assert.False(buffer.CanWrite<int>(7));
            Assert.False(buffer.CanWrite<long>(4));
            Assert.False(buffer.CanWrite<decimal>(2));
        }

        [Fact]
        public void can_fit_bytes()
        {
            var buffer = new ReBuffer(new byte[24]);

            Assert.True(buffer.CanWriteBytes(24));
            Assert.False(buffer.CanWriteBytes(25));
        }

        [Fact]
        public void can_copy_buffer()
        {
            var buffer1 = new ReBuffer(new byte[1024]);
            var buffer2 = new ReBuffer(new byte[1024]);

            for (int i = 0; i < 10; i++)
            {
                buffer1.Pack(ref i);
            }

            buffer2.Copy(buffer1);

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
            var buffer1 = new ReBuffer(new byte[1024]);
            var buffer2 = new ReBuffer(new byte[8]);

            for (int i = 0; i < 10; i++)
            {
                buffer1.Pack(ref i);
            }

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer2.Copy(buffer1));
        }

        [Fact]
        public void copy_buffer_into_new_buffer()
        {
            var buffer = new ReBuffer(new byte[128]);
            for (int i = 0; i < 10; i++)
            {
                buffer.Pack(ref i);
            }

            var copy = buffer.Clone();

            Assert.Equal(40, copy.Array.Length);
            Assert.Equal(40, copy.Length());

            for (int i = 0; i < 10; i++)
            {
                buffer.Unpack<int>(out int b);
                copy.Unpack<int>(out int c);
                Assert.Equal(b, c);
            }
        }

        [Fact]
        public void shrink_fit_resizes_to_used_space()
        {
            var buffer = new ReBuffer(32);

            for (int i = 0; i < 4; i++) buffer.Pack(ref i);

            {
                buffer.ShrinkFit();
                Assert.Equal(16, buffer.Array.Length);
            }

            {
                buffer.Unpack<int>(out var val);
                Assert.Equal(0, val);
            }

            {
                buffer.ShrinkFit();
                Assert.Equal(12, buffer.Array.Length);
            }

            for (int i = 1; i < 4; i++)
            {
                buffer.Unpack<int>(out var val);
                Assert.Equal(i, val);
            }
        }

        struct Ints
        {
            public int Int1;
            public float Int2;
            public byte Int3;
            public short Int4;
        }

        [Fact]
        public void array_memory_copy_int_single()
        {
            var buffer = new ReBuffer(2048);

            int[] array = Enumerable.Range(0, 256).ToArray();

            buffer.PackArray(array);
            int[] fromBuf = buffer.UnpackArray<int>();

            for (int i = 0; i < 256; i++)
            {
                Assert.Equal(array[i], fromBuf[i]);
            }
        }

        [Fact]
        public void array_memory_copy_int_multiple()
        {
            var buffer = new ReBuffer(2048);

            int[] array = Enumerable.Range(0, 64).ToArray();

            for (int i = 0; i < 4; i++)
            {
                buffer.PackArray(array);
            }

            Assert.Equal(sizeof(int) * 64 * 4 + sizeof(ulong) * 4, buffer.Length());

            for (int i = 1; i < 5; i++)
            {
                int[] fromBuf = buffer.UnpackArray<int>();
                for (int j = 0; j < 64; j++)
                {
                    Assert.Equal(array[j], fromBuf[j]);
                }

                Assert.Equal(sizeof(int) * 64 * i + sizeof(ulong) * i, buffer.ReadCursor());
            }
        }

        [Fact]
        public void array_memory_copy_struct_single()
        {
            var buffer = new ReBuffer(2048);

            Ints[] array = Enumerable.Range(0, 64)
                .Select(e => new Ints
                {
                    Int1 = e,
                    Int2 = e * 3.14159f,
                    Int3 = (byte)(e * 3),
                    Int4 = (short)(e * 4),
                })
                .ToArray();

            buffer.PackArray(array);

            Ints[] fromBuf = buffer.UnpackArray<Ints>();

            for (int i = 0; i < 64; i++)
            {
                Assert.Equal(array[i].Int1, fromBuf[i].Int1);
                Assert.Equal(array[i].Int2, fromBuf[i].Int2);
                Assert.Equal(array[i].Int3, fromBuf[i].Int3);
                Assert.Equal(array[i].Int4, fromBuf[i].Int4);
            }
        }

        [Fact]
        public void array_memory_copy_struct_multiple()
        {
            var buffer = new ReBuffer(2048 * 4);

            Ints[] array = Enumerable.Range(0, 64)
                .Select(e => new Ints
                {
                    Int1 = e,
                    Int2 = e * 3.14159f,
                    Int3 = (byte)(e * 3),
                    Int4 = (short)(e * 4),
                })
                .ToArray();

            for (int i = 0; i < 4; i++)
            {
                buffer.PackArray(array);
            }

            Assert.Equal(Marshal.SizeOf<Ints>() * 64 * 4 + sizeof(ulong) * 4, buffer.Length());

            for (int i = 1; i < 5; i++)
            {
                Ints[] fromBuf = buffer.UnpackArray<Ints>();
                for (int j = 0; j < 64; j++)
                {
                    Assert.Equal(array[j].Int1, fromBuf[j].Int1);
                    Assert.Equal(array[j].Int2, fromBuf[j].Int2);
                    Assert.Equal(array[j].Int3, fromBuf[j].Int3);
                    Assert.Equal(array[j].Int4, fromBuf[j].Int4);
                }

                Assert.Equal(Marshal.SizeOf<Ints>() * 64 * i + sizeof(ulong) * i, buffer.ReadCursor());
            }
        }

        [Fact]
        public void packing_null_array_inserts_zero_ulong()
        {
            var buffer = new ReBuffer(128);
            int[] data = null;

            buffer.PackArray(data);
            Assert.Equal(buffer.Length(), 8);
        }

        [Fact]
        public void packing_too_large_array_throws()
        {
            var buffer = new ReBuffer(16);
            int[] data = new int[8];

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.PackArray(data));
        }

        [Fact]
        public void unpacking_too_large_array_returns_empty_array()
        {
            var buffer = new ReBuffer(16);
            ulong size = 8;
            buffer.Pack(ref size);

            Assert.Equal(0, buffer.UnpackArray<int>().Length);
        }

        [Fact]
        public void free_space_gives_remaining_space()
        {
            var buffer = new ReBuffer(128);
            for (int i = 0; i < 16; i++) buffer.Pack(ref i);

            Assert.Equal(64, buffer.FreeSpace());
        }

        [Fact]
        public void setting_read_cursor_out_of_bounds_throws()
        {
            var buffer = new ReBuffer(8);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.SetReadCursor(9));
            Assert.Equal(0, buffer.ReadCursor());
        }

        [Fact]
        public void setting_read_cursor_moves_it()
        {
            var buffer = new ReBuffer(8);

            buffer.SetReadCursor(4);
            Assert.Equal(4, buffer.ReadCursor());
        }

        [Fact]
        public void setting_write_cursor_out_of_bounds_throws()
        {
            var buffer = new ReBuffer(8);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.SetWriteCursor(9));
            Assert.Equal(0, buffer.WriteCursor());
        }

        [Fact]
        public void setting_write_cursor_moves_it()
        {
            var buffer = new ReBuffer(8);

            buffer.SetWriteCursor(4);
            Assert.Equal(4, buffer.WriteCursor());
        }

        [Fact]
        public void can_read_bytes()
        {
            var buffer = new ReBuffer(4);

            Assert.True(buffer.CanReadBytes(4));
            Assert.False(buffer.CanReadBytes(5));
        }

        [Fact]
        public void can_write_bytes()
        {
            var buffer = new ReBuffer(4);

            Assert.True(buffer.CanWriteBytes(4));
            Assert.False(buffer.CanWriteBytes(5));
        }

        [Fact]
        public void can_write_bytes_expands_auto_buffer()
        {
            var buffer = new ReBuffer(4, true);

            Assert.True(buffer.CanWriteBytes(8));
            Assert.Equal(8, buffer.Array.Length);
        }

        [Fact]
        public void flush_clears_cursor_and_array()
        {
            var buffer = new ReBuffer(64);
            for (int i = 0; i < 16; i++) buffer.Pack(ref i);

            buffer.Flush();
            Assert.Equal(0, buffer.ReadCursor());
            Assert.Equal(0, buffer.WriteCursor());
            Assert.Equal(0, buffer.Array.Sum(e => e));
        }

        [Fact]
        public void read_cursor_plus_length_is_write_cursor()
        {
            var buffer = new ReBuffer(64);
            int data = 10;
            buffer.Pack(ref data);

            int readPlusLength = buffer.ReadCursor() + buffer.Length();
            Assert.Equal(readPlusLength, buffer.WriteCursor());
        }

        [Fact]
        public void copy_copies_used_data_from_source()
        {
            var buffer = new ReBuffer(16);
            for (int i = 0; i < 4; i++)
            {
                buffer.Pack(ref i);
            }

            buffer.Unpack<int>(out int _);
            buffer.Unpack<int>(out int _);

            var targetBuffer = new ReBuffer(16);
            targetBuffer.Copy(buffer);

            Assert.Equal(buffer.Length(), targetBuffer.Length());

            Assert.Equal(buffer.Peek<int>(0), targetBuffer.Peek<int>(0));
            Assert.Equal(buffer.Peek<int>(4), targetBuffer.Peek<int>(4));
        }

        [Fact]
        public void copy_copies_to_back_of_destination()
        {
            var sourceBuffer = new ReBuffer(16);
            var targetBuffer = new ReBuffer(32);

            for (int i = 0; i < 4; i++)
            {
                sourceBuffer.Pack(ref i);
                targetBuffer.Pack(ref i);
            }

            targetBuffer.Copy(sourceBuffer);

            Assert.Equal(32, targetBuffer.WriteCursor());

            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(sourceBuffer.Peek<int>(i * 4), targetBuffer.Peek<int>((i + 4) * 4));
            }
        }

        [Fact]
        public void copy_copes_to_back_of_destination_multiple()
        {
            var sourceBuffer = new ReBuffer(16);
            var targetBuffer = new ReBuffer(64);

            for (int i = 0; i < 4; i++)
            {
                sourceBuffer.Pack(ref i);
                targetBuffer.Pack(ref i);
            }

            targetBuffer.Copy(sourceBuffer);
            targetBuffer.Copy(sourceBuffer);
            targetBuffer.Copy(sourceBuffer);

            Assert.Equal(64, targetBuffer.WriteCursor());

            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(sourceBuffer.Peek<int>(i * 4), targetBuffer.Peek<int>((i + 4) * 4));
                Assert.Equal(sourceBuffer.Peek<int>(i * 4), targetBuffer.Peek<int>((i + 8) * 4));
                Assert.Equal(sourceBuffer.Peek<int>(i * 4), targetBuffer.Peek<int>((i + 12) * 4));
            }
        }

        [Fact]
        public void copy_expands_auto_buffer()
        {
            var sourceBuffer = new ReBuffer(16);
            var targetBuffer = new ReBuffer(4, true);

            for (int i = 0; i < 4; i++)
            {
                sourceBuffer.Pack(ref i);
            }

            targetBuffer.Copy(sourceBuffer);

            Assert.Equal(16, targetBuffer.Length());

            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(sourceBuffer.Peek<int>(i * 4), targetBuffer.Peek<int>(i * 4));
            }
        }

        [Fact]
        public void copy_expands_auto_buffer_multiple()
        {
            var sourceBuffer = new ReBuffer(16);
            var targetBuffer = new ReBuffer(4, true);

            for (int i = 0; i < 4; i++)
            {
                sourceBuffer.Pack(ref i);
            }

            targetBuffer.Copy(sourceBuffer);
            targetBuffer.Copy(sourceBuffer);
            targetBuffer.Copy(sourceBuffer);
            targetBuffer.Copy(sourceBuffer);

            Assert.Equal(64, targetBuffer.Length());

            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(sourceBuffer.Peek<int>(i * 4), targetBuffer.Peek<int>(i * 4));
            }
        }

        [Fact]
        public void clone_makes_copy_of_buffer()
        {
            var buffer = new ReBuffer(64);
            for (int i = 0; i < 16; i++) buffer.Pack(ref i);

            var cloned = buffer.Clone();

            for (int i = 0; i < 64; i++)
            {
                Assert.Equal(buffer.Array[i], cloned.Array[i]);
            }
        }

        [Fact]
        public void buffer_constructor_makes_shallow_copy()
        {
            var buffer = new ReBuffer(64);
            for (int i = 0; i < 16; i++) buffer.Pack(ref i);

            var copied = new ReBuffer(buffer);

            Assert.Equal(buffer.WriteCursor(), copied.WriteCursor());
            Assert.Equal(buffer.ReadCursor(), copied.ReadCursor());
            Assert.Equal(buffer.Array, copied.Array);
        }

        [Fact]
        public void expand_increases_size_and_clones_values()
        {
            var buffer = new ReBuffer(32, true);
            for (int i = 0; i < 16; i++) buffer.Pack(ref i);

            Assert.Equal(64, buffer.Array.Length);

            for (int i = 0; i < 16; i++)
            {
                buffer.Unpack<int>(out int fromBuf);
                Assert.Equal(i, fromBuf);
            }
        }

        [Fact]
        public void can_write_expands_auto_buffer()
        {
            var buffer = new ReBuffer(4, true);

            Assert.True(buffer.CanWrite<long>());
            Assert.Equal(8, buffer.Array.Length);
        }

        [Fact]
        public void move_read_cursor_out_of_bounds_throws()
        {
            var buffer = new ReBuffer(8);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.MoveReadCursor(9));
            Assert.Equal(0, buffer.ReadCursor());
        }

        [Fact]
        public void move_write_cursor_out_of_bounds_throws()
        {
            var buffer = new ReBuffer(8);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.MoveWriteCursor(9));
            Assert.Equal(0, buffer.WriteCursor());
        }

        [Fact]
        public void peek_gives_value_without_changing_buffer()
        {
            var buffer = new ReBuffer(8);
            int val = 12345;
            int val2 = 99999;
            buffer.Pack(ref val);
            buffer.Pack(ref val2);

            Assert.Equal(val, buffer.Peek<int>());
            Assert.Equal(val2, buffer.Peek<int>(4));
        }

        [Fact]
        public void peek_out_of_bounds_throws()
        {
            var buffer = new ReBuffer(4);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.Peek<ulong>());
            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.Peek<short>(3));
        }

        [Fact]
        public void pack_buffer_only_copies_used_memory()
        {
            var internalBuffer = new ReBuffer(64);
            for (int i = 0; i < 8; i++) internalBuffer.Pack(ref i);

            var buffer = new ReBuffer(64);
            buffer.PackBuffer(internalBuffer);

            // 32 for data, 16 for read/write cursor, 1 for endianness
            Assert.Equal(32 + 16 + 1, buffer.Length());
            Assert.Equal(internalBuffer.ReadCursor(), buffer.ReadCursor());
            Assert.Equal(internalBuffer.WriteCursor() + 17, buffer.WriteCursor());
        }

        [Fact]
        public void unpack_buffer()
        {
            var internalBuffer = new ReBuffer(64);
            for (int i = 0; i < 8; i++) internalBuffer.Pack(ref i);

            var buffer = new ReBuffer(64);
            buffer.PackBuffer(internalBuffer);

            var fromBuf = buffer.UnpackBuffer();

            Assert.Equal(32, fromBuf.Array.Length);
            Assert.Equal(internalBuffer.ReadCursor(), fromBuf.ReadCursor());
            Assert.Equal(internalBuffer.WriteCursor(), fromBuf.WriteCursor());
        }

        [Fact]
        public void get_reference_to_primitive()
        {
            var buffer = new ReBuffer(16);

            int value = 10;
            buffer.Pack(ref value);

            ref int valueRef = ref buffer.GetRef<int>();
            valueRef *= 10;

            buffer.Unpack<int>(out int fromBuf);

            Assert.Equal(100, fromBuf);
        }

        [Fact]
        public void get_reference_to_unmanaged_struct()
        {
            var buffer = new ReBuffer(32);

            var value = new TestBlittableStruct
            {
                Float = 1.234f,
                Int = 3452,
            };
            buffer.Pack(ref value);

            ref TestBlittableStruct valueRef = ref buffer.GetRef<TestBlittableStruct>();
            valueRef.Float *= 10;
            valueRef.Int *= 100;

            buffer.Unpack<TestBlittableStruct>(out TestBlittableStruct fromBuf);

            Assert.Equal(12.34f, fromBuf.Float);
            Assert.Equal(345200, fromBuf.Int);
        }

        [Fact]
        public void get_reference_to_primitive_with_offset()
        {
            var buffer = new ReBuffer(16);

            int value = 10;
            buffer.Pack(ref value);
            buffer.Pack(ref value);
            buffer.Pack(ref value);

            ref int valueRef = ref buffer.GetRef<int>(sizeof(int) * 2);
            valueRef *= 10;

            buffer.Unpack<int>(out int fromBuf);
            Assert.Equal(10, fromBuf);
            buffer.Unpack<int>(out fromBuf);
            Assert.Equal(10, fromBuf);
            buffer.Unpack<int>(out fromBuf);
            Assert.Equal(100, fromBuf);
        }

        [Fact]
        public void get_reference_to_unmanaged_struct_with_offset()
        {
            var buffer = new ReBuffer(32);

            var value = new TestBlittableStruct
            {
                Float = 1.234f,
                Int = 3452,
            };
            buffer.Pack(ref value);
            buffer.Pack(ref value);
            buffer.Pack(ref value);

            ref TestBlittableStruct valueRef = ref buffer.GetRef<TestBlittableStruct>(16);
            valueRef.Float *= 10;
            valueRef.Int *= 100;

            buffer.Unpack<TestBlittableStruct>(out TestBlittableStruct fromBuf);
            Assert.Equal(1.234f, fromBuf.Float);
            Assert.Equal(3452, fromBuf.Int);
            buffer.Unpack<TestBlittableStruct>(out fromBuf);
            Assert.Equal(1.234f, fromBuf.Float);
            Assert.Equal(3452, fromBuf.Int);
            buffer.Unpack<TestBlittableStruct>(out fromBuf);
            Assert.Equal(12.34f, fromBuf.Float);
            Assert.Equal(345200, fromBuf.Int);
        }

        [Fact]
        public void get_reference_throws_on_out_of_bounds()
        {
            var buffer = new ReBuffer(8);

            Assert.Throws<System.IndexOutOfRangeException>(() =>
            {
                ref var _ = ref buffer.GetRef<int>(8);
            });
        }

        [Fact]
        public void unpack_short_with_endianness()
        {
            // Pack with Little-Endian, see if it reflects as the Big-Endian variant

            var buffer = new ReBuffer(4);
            buffer.SetEndianness(Endianness.LittleEndian);
            short val = 8192;
            buffer.Pack(ref val);

            buffer.SetEndianness(Endianness.BigEndian);
            buffer.Unpack(out val);

            Assert.Equal(32, val);
        }

        [Fact]
        public void unpack_int_with_endianness()
        {
            // Pack with Little-Endian, see if it reflects as the Big-Endian variant

            var buffer = new ReBuffer(4);
            buffer.SetEndianness(Endianness.LittleEndian);
            int val = 536870912;
            buffer.Pack(ref val);

            buffer.SetEndianness(Endianness.BigEndian);
            buffer.Unpack(out val);

            Assert.Equal(32, val);
        }

        [Fact]
        public void unpack_long_with_endianness()
        {
            // Pack with Little-Endian, see if it reflects as the Big-Endian variant

            var buffer = new ReBuffer(8);
            buffer.SetEndianness(Endianness.LittleEndian);
            long val = 1697783392239648;
            buffer.Pack(ref val);
            
            buffer.SetEndianness(Endianness.BigEndian);
            buffer.Unpack(out val);

            Assert.Equal(2305843009751090688, val);
        }

        [Fact]
        public void unpack_float_with_endianness()
        {
            // Pack with Little-Endian, see if it reflects as the Big-Endian variant

            var buffer = new ReBuffer(8);
            buffer.SetEndianness(Endianness.LittleEndian);
            float val = 2.20224633e-38f;
            buffer.Pack(ref val);

            buffer.SetEndianness(Endianness.BigEndian);
            buffer.Unpack(out val);

            Assert.Equal(-1.46324619e-12f, val);
        }
    }
}

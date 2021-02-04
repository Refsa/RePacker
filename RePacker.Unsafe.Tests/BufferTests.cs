using Xunit;
using Refsa.RePacker.Buffers;
using System.Runtime.InteropServices;

namespace Refsa.RePacker.Buffers.Tests
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
            buffer.Push<int>(ref testVal);

            Assert.Throws<System.ArgumentOutOfRangeException>(() => buffer.Pop<ulong>(out ulong outVal));
        }

        [Fact]
        public void get_array_gives_array()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(1024);

            Assert.Equal(buf, buffer.GetArray().array);
        }

        [Fact]
        public void length_gives_active_elements_length()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(1024);

            int testVal = 10;

            for (int i = 0; i < 6; i++)
                buffer.Push<int>(ref testVal);

            Assert.Equal(sizeof(int) * 6, buffer.Length());

            buffer.Pop<int>(out int fromBuf);
            buffer.Pop<int>(out fromBuf);
            buffer.Pop<int>(out fromBuf);

            Assert.Equal(sizeof(int) * 3, buffer.Length());
        }

        [Fact]
        public void count_gives_total_size_used()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(1024);

            int testVal = 10;

            for (int i = 0; i < 6; i++)
                buffer.Push<int>(ref testVal);

            Assert.Equal(sizeof(int) * 6, buffer.Count());

            buffer.Pop<int>(out int fromBuf);
            buffer.Pop<int>(out fromBuf);
            buffer.Pop<int>(out fromBuf);

            Assert.Equal(sizeof(int) * 6, buffer.Count());
        }

        [Fact]
        public void cursor_gives_active_first_element_index()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(1024);

            int testVal = 10;

            for (int i = 0; i < 6; i++)
                buffer.Push<int>(ref testVal);

            Assert.Equal(0, buffer.Cursor());

            buffer.Pop<int>(out int fromBuf);
            buffer.Pop<int>(out fromBuf);
            buffer.Pop<int>(out fromBuf);

            Assert.Equal(sizeof(int) * 3, buffer.Cursor());
        }

        [Fact]
        public void flush_zeroes_out_used_memory()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(80);

            int testVal = 10;
            for (int i = 0; i < 10; i++)
                buffer.Push<int>(ref testVal);

            buffer.Flush();

            for (int i = 0; i < 10; i++)
            {
                buffer.Pop<int>(out int val);
                Assert.Equal(0, val);
            }
        }

        [Fact]
        public void flush_clears_cursor_and_offset()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(80);

            int testVal = 10;
            for (int i = 0; i < 10; i++)
                buffer.Push<int>(ref testVal);

            buffer.Pop<int>(out int _);
            buffer.Pop<int>(out int _);
            buffer.Pop<int>(out int _);

            buffer.Flush();

            Assert.Equal(0, buffer.Cursor());
            Assert.Equal(0, buffer.Count());
            Assert.Equal(0, buffer.Length());
        }

        [Fact]
        public void reset_clears_cursor_and_offset()
        {
            (byte[] buf, Buffer buffer) = MakeBuffer(80);

            int testVal = 10;
            for (int i = 0; i < 10; i++)
                buffer.Push<int>(ref testVal);

            buffer.Pop<int>(out int _);
            buffer.Pop<int>(out int _);
            buffer.Pop<int>(out int _);

            buffer.Reset();

            Assert.Equal(0, buffer.Cursor());
            Assert.Equal(0, buffer.Count());
            Assert.Equal(0, buffer.Length());
        }

        [Fact]
        public void get_array_gives_used_length()
        {
            var buffer = MakeBuffer(1024);

            int testVal = 10;

            buffer.buffer.Push<int>(ref testVal);
            buffer.buffer.Push<int>(ref testVal);
            buffer.buffer.Push<int>(ref testVal);
            buffer.buffer.Push<int>(ref testVal);
            buffer.buffer.Push<int>(ref testVal);

            var arrayFromBuffer = buffer.buffer.GetArray();

            Assert.Equal(5 * sizeof(int), arrayFromBuffer.length);
        }

        [Fact]
        public void can_fit_should_succeed()
        {
            var buffer = MakeBuffer(4);
            Assert.True(buffer.buffer.CanFit(sizeof(int)));
        }

        [Fact]
        public void can_fit_should_fail()
        {
            var buffer = MakeBuffer(4);
            Assert.False(buffer.buffer.CanFit(sizeof(ulong)));
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

            buffer.Push<int>(ref data);

            buffer.Pop<int>(out int fromBuf);

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
                buffer.Push<int>(ref data);

            for (int i = 0; i < 100; i++)
            {
                buffer.Pop<int>(out int fromBuf);
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

            buffer.Push<float>(ref data);

            buffer.Pop<float>(out float fromBuf);

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
                buffer.Push<float>(ref data);

            for (float i = 0; i < 100; i++)
            {
                buffer.Pop<float>(out float fromBuf);
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

            buffer.Push(ref testStruct);

            buffer.Pop<TestBlittableStruct>(out var fromBuf);

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
                buffer.Push(ref testStruct);
            }

            for (int i = 0; i < 100; i++)
            {
                buffer.Pop<TestBlittableStruct>(out var fromBuf);
                Assert.Equal(testStruct.Float, fromBuf.Float);
                Assert.Equal(testStruct.Int, fromBuf.Int);
            }
        }

        public struct TestNonBlittableStruct
        {
            public string String;
        }

        // [Fact]
        // public void push_and_pop_non_blittable_struct_throws()
        // {
        //     (byte[] buf, Buffer buffer) = MakeBuffer(800);

        //     var testStruct = new TestNonBlittableStruct { String = "ABCD" };

        //     Assert.Throws<System.ArgumentException>(() => buffer.Push(ref testStruct));
        // }

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

            buffer.Push(ref testStruct);

            Assert.Equal(3, buffer.Count());
        }

        #region DirectPacking
        [Fact]
        public void direct_packing_bool()
        {
            var buffer = new Buffer(new byte[2]);

            char value = 'G';

            buffer.PushChar(ref value);
            buffer.PopChar(out char posFromBuf);
            Assert.Equal(value, posFromBuf);
        }

        [Fact]
        public void direct_packing_char()
        {
            var buffer = new Buffer(new byte[1]);

            bool value = true;

            buffer.PushBool(ref value);
            buffer.PopBool(out bool posFromBuf);
            Assert.Equal(value, posFromBuf);
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
        public void direct_packing_uint()
        {
            var buffer = new Buffer(new byte[1024]);

            uint positive = 1234567890;
            buffer.PushUInt(ref positive);
            buffer.PopUInt(out uint posFromBuf);
            Assert.Equal(positive, posFromBuf);
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
        public void direct_packing_ulong()
        {
            var buffer = new Buffer(new byte[1024]);

            ulong positive = 12345678902345243623;

            buffer.PushULong(ref positive);
            buffer.PopULong(out ulong posFromBuf);
            Assert.Equal(positive, posFromBuf);
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
        public void direct_packing_double()
        {
            var buffer = new Buffer(new byte[8]);

            double positive = 3.141598f;
            double negative = -6.2826f;

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

            double val = 2_000_000_000f;
            double step = 2_000_000_000f / 128f;

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
        #endregion
    }
}

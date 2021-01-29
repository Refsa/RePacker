using Xunit;
using Refsa.Repacker.Buffers;

namespace Refsa.Repacker.Buffers.Tests
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
    }
}

using Xunit;
using RePacker.Buffers;
using System.Runtime.InteropServices;

namespace RePacker.Buffers.Tests
{
    public class BufferPoolTests
    {
        [Fact]
        public void can_get_buffer()
        {
            var bufPool = new BufferPool(128, 128);
            Assert.Equal(128, bufPool.Capacity);

            bufPool.GetBuffer(out Buffer buffer);
            Assert.Equal(128, buffer.FreeSpace());
        }

        [Fact]
        public void can_release_buffer()
        {
            var bufPool = new BufferPool(128, 128);
            Assert.Equal(128, bufPool.Available);

            bufPool.GetBuffer(out Buffer buffer);
            Assert.Equal(127, bufPool.Available);

            bufPool.FreeBuffer(buffer);
            Assert.Equal(128, bufPool.Available);
        }

        [Fact]
        public void can_release_buffer_multi()
        {
            var bufPool = new BufferPool(128, 128);
            Assert.Equal(128, bufPool.Available);

            var usedBuffers = new Buffer[64];

            for (int i = 0; i < 64; i++)
            {
                bufPool.GetBuffer(out Buffer buffer);
                usedBuffers[i] = buffer;
            }

            Assert.Equal(64, bufPool.Available);

            for (int i = 0; i < 64; i++)
            {
                bufPool.FreeBuffer(usedBuffers[i]);
            }
            Assert.Equal(128, bufPool.Available);
        }

        [Fact]
        public void can_expand_pool()
        {
            var bufPool = new BufferPool(128, 64);
            Assert.Equal(64, bufPool.Available);

            for (int i = 0; i < 64; i++)
            {
                bufPool.GetBuffer(out var _);
            }

            Assert.Equal(0, bufPool.Available);

            bufPool.GetBuffer(out var _);
            
            Assert.Equal(63, bufPool.Available);
        }
    }
}
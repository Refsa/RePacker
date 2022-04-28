using Xunit;

namespace RePacker.Unsafe.Tests
{
    public class NativeBlobTests
    {
        [Fact]
        public void NativeBlob_Write_and_Read()
        {
            var blob = new NativeBlob(1 * sizeof(int));

            blob.Write(0, 1234);
            Assert.Equal(1234, blob.Read<int>(0));

            blob.Dispose();
        }

        [Fact]
        public void NativeBlob_Write_and_Read_many()
        {
            int stride = sizeof(int);
            var blob = new NativeBlob(10 * stride);

            for (int i = 0; i < 10; i++)
            {
                blob.Write(i * stride, i + 1);
            }

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i + 1, blob.Read<int>(i * stride));
            }

            blob.Dispose();
        }

        [Fact]
        public void NativeBlob_Read_throw_index_out_of_range()
        {
            Assert.Throws<System.IndexOutOfRangeException>(() =>
            {
                var blob = new NativeBlob(4);
                try
                {
                    int _ = blob.Read<int>(4);
                }
                catch
                {
                    blob.Dispose();
                    throw;
                }
            });
        }

        [Fact]
        public void NativeBlob_Write_throw_index_out_of_range()
        {
            Assert.Throws<System.IndexOutOfRangeException>(() =>
            {
                var blob = new NativeBlob(4);
                try
                {
                    blob.Write<int>(4, 4);
                }
                catch
                {
                    blob.Dispose();
                    throw;
                }
            });
        }

        [Fact]
        public void NativeBlob_GetRef()
        {
            int stride = sizeof(int);
            var blob = new NativeBlob(5 * stride);

            for (int i = 0; i < 5; i++)
            {
                blob.Write(i * stride, i + 1);
            }

            blob.GetRef<int>(2 * stride) = 1234;

            Assert.Equal(1, blob.Read<int>(0 * stride));
            Assert.Equal(2, blob.Read<int>(1 * stride));
            Assert.Equal(1234, blob.Read<int>(2 * stride));
            Assert.Equal(4, blob.Read<int>(3 * stride));
            Assert.Equal(5, blob.Read<int>(4 * stride));

            blob.Dispose();
        }

        [Fact]
        public void NativeBlob_EnsureCapacity()
        {
            int stride = sizeof(int);
            var blob = new NativeBlob(5 * stride);
            for (int i = 0; i < 5; i++)
            {
                blob.Write(i * stride, i + 1);
            }

            blob.EnsureCapacity(10 * stride);
            Assert.Equal(10 * stride, blob.Capacity);

            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(i + 1, blob.Read<int>(i * stride));
            }

            blob.Dispose();
        }

        [Fact]
        public void NativeBlob_ShrinkFit_from_zero()
        {
            int stride = sizeof(int);
            var blob = new NativeBlob(10 * stride);
            for (int i = 0; i < 5; i++) blob.Write(i * stride, i + 1);

            blob.ShrinkFit(0, 5 * stride);
            Assert.Equal(5 * stride, blob.Capacity);

            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(i + 1, blob.Read<int>(i * stride));
            }
        }

        [Fact]
        public void NativeBlob_ShrinkFit_with_start_offset()
        {
            int stride = sizeof(int);
            var blob = new NativeBlob(10 * stride);
            for (int i = 0; i < 10; i++) blob.Write(i * stride, i + 1);

            blob.ShrinkFit(5 * stride, 5 * stride);
            Assert.Equal(5 * stride, blob.Capacity);

            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(i + 1 + 5, blob.Read<int>(i * stride));
            }
        }

        [Fact]
        public void NativeBlob_ToArray()
        {
            int stride = sizeof(int);
            var blob = new NativeBlob(10 * stride);
            for (int i = 0; i < 10; i++) blob.Write(i * stride, i + 1);

            var array = blob.ToArray(blob.Capacity);
            Assert.Equal(blob.Capacity, array.Length);

            for (int i = 0; i < blob.Capacity; i++)
            {
                Assert.Equal(blob.Read<byte>(i), array[i]);
            }
        }

        [Fact]
        public void NativeBlob_CopyTo_Blob_to_Blob()
        {
            int stride = sizeof(int);
            var src = new NativeBlob(10 * stride);
            var dst = new NativeBlob(10 * stride);
        }
    }
}
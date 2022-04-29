using System.Linq;
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

            blob.Dispose();
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

            blob.Dispose();
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

            blob.Dispose();
        }

        [Fact]
        public void NativeBlob_CopyTo_Blob_to_Blob_no_offsets()
        {
            int stride = sizeof(int);
            var src = new NativeBlob(10 * stride);
            var dst = new NativeBlob(10 * stride);

            for (int i = 0; i < 10; i++) src.Write(i * stride, i + 1);

            src.CopyTo(ref dst, src.Capacity);

            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(src.Read<int>(i * stride), dst.Read<int>(i * stride));
            }

            src.Dispose();
            dst.Dispose();
        }

        [Fact]
        public void NativeBlob_CopyTo_Blob_to_Blob_with_src_offsets()
        {
            int stride = sizeof(int);
            var src = new NativeBlob(10 * stride);
            var dst = new NativeBlob(5 * stride);

            for (int i = 0; i < 10; i++) src.Write(i * stride, i + 1);

            src.CopyTo(ref dst, 5 * stride, 5 * stride);

            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(src.Read<int>((i + 5) * stride), dst.Read<int>(i * stride));
            }

            src.Dispose();
            dst.Dispose();
        }

        [Fact]
        public void NativeBlob_CopyTo_Blob_to_Blob_with_dst_offsets()
        {
            int stride = sizeof(int);
            var src = new NativeBlob(5 * stride);
            var dst = new NativeBlob(10 * stride);

            for (int i = 0; i < 5; i++) src.Write(i * stride, i + 1);

            src.CopyTo(ref dst, 5 * stride, dstOffset: 5 * stride);

            for (int i = 0; i < 5; i++)
            {
                Assert.Equal(src.Read<int>(i * stride), dst.Read<int>((i + 5) * stride));
            }

            src.Dispose();
            dst.Dispose();
        }

        [Fact]
        public void NativeBlob_CopyTo_Blob_src_offset_throws_index_out_of_range()
        {
            Assert.Throws<System.IndexOutOfRangeException>(() =>
            {
                var src = new NativeBlob(10);
                var dst = new NativeBlob(10);

                try
                {
                    src.CopyTo(ref dst, 5, 6);
                }
                catch
                {
                    src.Dispose();
                    dst.Dispose();
                    throw;
                }
            });
        }

        [Fact]
        public void NativeBlob_CopyTo_Blob_dst_offset_throws_index_out_of_range()
        {
            Assert.Throws<System.IndexOutOfRangeException>(() =>
            {
                var src = new NativeBlob(10);
                var dst = new NativeBlob(10);

                try
                {
                    src.CopyTo(ref dst, 5, dstOffset: 6);
                }
                catch
                {
                    src.Dispose();
                    dst.Dispose();
                    throw;
                }
            });
        }

        [Fact]
        public void NativeBlob_Append()
        {
            int stride = sizeof(int);
            int[] src = Enumerable.Range(0, 10).ToArray();
            var dst = new NativeBlob(src.Length * stride);

            dst.Append(src, 0, 0, src.Length);

            for (int i = 0; i < src.Length; i++)
            {
                Assert.Equal(src[i], dst.Read<int>(i * stride));
            }

            dst.Dispose();
        }

        [Fact]
        public void NativeBlob_Append_srcOffset_throws()
        {
            int stride = sizeof(int);
            int[] src = new int[] { 1, 2, 3, 4 };

            Assert.Throws<System.IndexOutOfRangeException>(() =>
            {
                var dst = new NativeBlob(src.Length * stride);
                try
                {
                    dst.Append(src, 5, 0, 5);
                }
                catch
                {
                    dst.Dispose();
                    throw;
                }
            });
        }

        [Fact]
        public void NativeBlob_Append_dstOffset_throws()
        {
            int stride = sizeof(int);
            int[] src = new int[] { 1, 2, 3, 4 };

            Assert.Throws<System.IndexOutOfRangeException>(() =>
            {
                var dst = new NativeBlob(src.Length * stride);
                try
                {
                    dst.Append(src, 0, 5 * stride, 5);
                }
                catch
                {
                    dst.Dispose();
                    throw;
                }
            });
        }
    }
}
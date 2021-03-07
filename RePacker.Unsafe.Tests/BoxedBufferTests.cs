using Xunit;
using RePacker.Buffers;
using System.Runtime.InteropServices;
using System.Linq;

namespace RePacker.Buffers.Tests
{
    public class BoxedBufferTests
    {
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
    }
}
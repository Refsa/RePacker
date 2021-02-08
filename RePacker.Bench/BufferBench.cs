

using BenchmarkDotNet.Attributes;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Benchmarks
{
    [MemoryDiagnoser]
    public class BufferBench
    {
        static byte[] backingBuffer;
        static Buffer buffer;

        public BufferBench()
        {
            backingBuffer = new byte[1024];
            buffer = new Buffer(backingBuffer, 0);
        }

        [Benchmark]
        public void BufferPushPopInt()
        {
            for (int j = 0; j < 400; j++)
            {
                for (int i = 0; i < 1024 / sizeof(int); i++)
                {
                    int val = i;
                    buffer.Push<int>(ref val);
                }

                for (int i = 0; i < 1024 / sizeof(int); i++)
                {
                    int val = 0;
                    buffer.Pop<int>(out val);
                }

                buffer.Reset();
            }
        }

        [Benchmark]
        public void BufferPushPopFloat()
        {
            for (int j = 0; j < 400; j++)
            {
                for (int i = 0; i < 1024 / sizeof(float); i++)
                {
                    float val = i;
                    buffer.Push<float>(ref val);
                }

                for (int i = 0; i < 1024 / sizeof(float); i++)
                {
                    float val = 0;
                    buffer.Pop<float>(out val);
                }

                buffer.Reset();
            }
        }

        [Benchmark]
        public void BufferPushPopULong()
        {
            for (int j = 0; j < 400; j++)
            {
                for (int i = 0; i < 1024 / sizeof(ulong); i++)
                {
                    ulong val = (ulong)i;
                    buffer.Push<ulong>(ref val);
                }

                for (int i = 0; i < 1024 / sizeof(ulong); i++)
                {
                    ulong val = 0;
                    buffer.Pop<ulong>(out val);
                }

                buffer.Reset();
            }
        }

        struct TestBlittableStruct
        {
            public int Int1;
            public int Int2;
            public int Int3;
            public int Int4;
            public int Int5;
            public int Int6;
            public int Int7;
            public int Int8;
        }

        [Benchmark]
        public void BufferPushPopStruct()
        {
            for (int j = 0; j < 12_500; j++)
            {
                for (int i = 0; i < 1024 / System.Runtime.InteropServices.Marshal.SizeOf<TestBlittableStruct>(); i++)
                {
                    TestBlittableStruct val = new TestBlittableStruct();
                    buffer.Push<TestBlittableStruct>(ref val);
                }

                for (int i = 0; i < 1024 / System.Runtime.InteropServices.Marshal.SizeOf<TestBlittableStruct>(); i++)
                {
                    buffer.Pop<TestBlittableStruct>(out var val);
                }

                buffer.Reset();
            }
        }
    }
}
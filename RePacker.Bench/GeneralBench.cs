using BenchmarkDotNet.Attributes;
using RePacker.Buffers;
using RePacker.Builder;

namespace RePacker.Benchmarks
{
    [MemoryDiagnoser]
    public class GeneralBenches
    {
        BoxedBuffer buffer = new BoxedBuffer(1 << 16);

        /* [Benchmark]
        public void BenchRePackerSpeedInt()
        {
            int val = 123456789;

            buffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack(buffer, ref val);
            }
        } */

        /* 
        TODO: Remove me
        [Benchmark]
        public void BenchPackerSpeedInt()
        {
            if (!RePacker.IsSetup)
            {
                RePacker.Init();
            }

            var buffer = new BoxedBuffer(1 << 16);
            int val = 123456789;

            if (TypeCache.TryGetTypePacker<int>(out var packer))
            {
                if (packer.GetTypePacker<RePackerWrapper<int>>() is RePackerWrapper<int> wrapper)
                {
                    for (int i = 0; i < 10_000; i++)
                    {
                        wrapper.Pack(buffer, ref val);
                    }
                }
            }
        } */

        [Benchmark]
        public void BenchDirectSpeedInt()
        {
            int val = 123456789;

            buffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                buffer.Buffer.Pack(ref val);
            }
        }

        [Benchmark]
        public void BenchRawSpeedInt()
        {
            int val = 123456789;

            buffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                buffer.Buffer.Unpack(out val);
            }
        }
    }
}
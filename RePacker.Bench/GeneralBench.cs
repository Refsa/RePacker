using BenchmarkDotNet.Attributes;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Builder;

namespace Refsa.RePacker.Benchmarks
{
    [MemoryDiagnoser]
    public class GeneralBenches
    {
        [Benchmark]
        public void BenchRePackerSpeedInt()
        {
            if (!RePacker.IsSetup)
            {
                RePacker.Init();
            }

            var buffer = new BoxedBuffer(1 << 16);
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack(buffer, ref val);
            }
        }

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
        }

        [Benchmark]
        public void BenchDirectSpeedInt()
        {
            var buffer = new Buffer(new byte[1 << 16]);
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                buffer.Push(ref val);
            }
        }

        [Benchmark]
        public void BenchRawSpeedInt()
        {
            var buffer = new Buffer(new byte[1 << 16]);
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                buffer.PushInt(ref val);
            }
        }
    }
}
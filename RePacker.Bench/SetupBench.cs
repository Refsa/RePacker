using BenchmarkDotNet.Attributes;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Builder;

namespace Refsa.RePacker.Benchmarks
{
    [MemoryDiagnoser]
    public class SetupBench
    {
        [Benchmark]
        public void BenchSetupTime()
        {
            RePacker.Init();
        }
    }
}
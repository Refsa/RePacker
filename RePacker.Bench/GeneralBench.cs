using BenchmarkDotNet.Attributes;
using RePacker.Buffers;
using RePacker.Builder;

namespace RePacker.Benchmarks
{
    [MemoryDiagnoser]
    public class GeneralBenches
    {
        Buffer buffer = new Buffer(1 << 16);

/*
                    Method |      Mean
        ------------------ |----------
         BenchSizeOfSimple |  33.44 us
        BenchSizeOfComplex | 188.47 us
*/

        [Benchmark]
        public void BenchSizeOfSimple()
        {
            int val = 10;
            for (int i = 0; i < 10_000; i++)
            {
                int size = RePacking.SizeOf(ref val);
            }
        }

        [Benchmark]
        public void BenchSizeOfComplex()
        {
            var val = new Parent();
            for (int i = 0; i < 10_000; i++)
            {
                int size = RePacking.SizeOf(ref val);
            }
        }
    }
}
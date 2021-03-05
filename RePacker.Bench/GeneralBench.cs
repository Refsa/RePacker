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
        -------------------- |----------
           BenchSizeOfSimple |  33.54 us
          BenchSizeOfComplex |  96.56 us
        BenchSizeOfHasString | 358.52 us
           BenchSizeOfString | 195.57 us
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

        StructWithString structWithString = new StructWithString { String1 = "hello" };

        [Benchmark]
        public void BenchSizeOfHasString()
        {
            for (int i = 0; i < 10_000; i++)
            {
                int size = RePacking.SizeOf(ref structWithString);
            }
        }

        [Benchmark]
        public void BenchSizeOfString()
        {
            var val = "hello";

            for (int i = 0; i < 10_000; i++)
            {
                int size = RePacking.SizeOf(ref val);
            }
        }
    }
}
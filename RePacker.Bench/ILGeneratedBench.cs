
using BenchmarkDotNet.Attributes;
using RePacker.Buffers;

namespace RePacker.Benchmarks
{
    [MemoryDiagnoser]
    public class ILGenerated
    {
        static byte[] backingBuffer = new byte[1024];
        const int RUNS = 100_000;

        [Benchmark]
        public void BenchStructILGen()
        {
            var ts2 = new TestStruct2
            {
                Bool = true,
                // Char = 'X',
                Sbyte = 10,
                Byte = 20,
                Short = 30,
                Ushort = 40,
                Int = 50,
                Uint = 60,
                Long = 70,
                Ulong = 80,
                Float = 90,
                Double = 100,
                Decimal = 1000,
            };

            var buffer = new ReBuffer(backingBuffer);

            for (int i = 0; i < RUNS; i++)
            {
                RePacking.Pack<TestStruct2>(buffer, ref ts2);
                var _ = RePacking.Unpack<TestStruct2>(buffer);

                buffer.Reset();
            }
        }

        [Benchmark]
        public void BenchStructNonILGen()
        {
            var ts2 = new TestStruct2
            {
                Bool = true,
                // Char = 'X',
                Sbyte = 10,
                Byte = 20,
                Short = 30,
                Ushort = 40,
                Int = 50,
                Uint = 60,
                Long = 70,
                Ulong = 80,
                Float = 90,
                Double = 100,
                Decimal = 1000,
            };

            var buffer = new ReBuffer(backingBuffer, 0);

            for (int i = 0; i < RUNS; i++)
            {
                buffer.Pack(ref ts2);
                buffer.Unpack<TestStruct2>(out var _);

                buffer.Reset();
            }
        }

        [Benchmark]
        public void BenchClassILGen()
        {
            var ts2 = new TestClass2
            {
                Bool = true,
                // Char = 'X',
                Sbyte = 10,
                Byte = 20,
                Short = 30,
                Ushort = 40,
                Int = 50,
                Uint = 60,
                Long = 70,
                Ulong = 80,
                Float = 90,
                Double = 100,
                Decimal = 1000,
            };

            var buffer = new ReBuffer(backingBuffer);

            for (int i = 0; i < RUNS; i++)
            {
                RePacking.Pack<TestClass2>(buffer, ref ts2);
                var _ = RePacking.Unpack<TestClass2>(buffer);

                buffer.Reset();
            }
        }

        [Benchmark]
        public void BenchNestedTypesILGen()
        {
            var p = new Parent
            {
                Float = 1.337f,
                ULong = 987654321,
                Child = new Child
                {
                    Float = 10f,
                    Byte = 120,
                },
            };

            var buffer = new ReBuffer(backingBuffer);

            for (int i = 0; i < RUNS; i++)
            {
                RePacking.Pack<Parent>(buffer, ref p);
                var fromBuf = RePacking.Unpack<Parent>(buffer);
                buffer.Reset();
            }
        }
    }
}

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

            BoxedBuffer boxedBuffer = new BoxedBuffer(backingBuffer);

            for (int i = 0; i < RUNS; i++)
            {
                RePacker.Pack<TestStruct2>(boxedBuffer, ref ts2);
                var _ = RePacker.Unpack<TestStruct2>(boxedBuffer);

                boxedBuffer.Buffer.Reset();
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

            Buffer buffer = new Buffer(backingBuffer, 0);

            for (int i = 0; i < RUNS; i++)
            {
                buffer.Push(ref ts2);
                buffer.Pop<TestStruct2>(out var _);

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

            BoxedBuffer boxedBuffer = new BoxedBuffer(backingBuffer);

            for (int i = 0; i < RUNS; i++)
            {
                RePacker.Pack<TestClass2>(boxedBuffer, ref ts2);
                var _ = RePacker.Unpack<TestClass2>(boxedBuffer);

                boxedBuffer.Buffer.Reset();
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

            var buffer = new BoxedBuffer(backingBuffer);

            for (int i = 0; i < RUNS; i++)
            {
                RePacker.Pack<Parent>(buffer, ref p);
                var fromBuf = RePacker.Unpack<Parent>(buffer);
                buffer.Buffer.Reset();
            }
        }
    }
}
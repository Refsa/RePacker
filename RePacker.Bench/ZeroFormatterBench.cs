
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using RePacker.Buffers;
using RePacker.Builder;

namespace RePacker.Benchmarks
{
    [MemoryDiagnoser]
    public class ZeroFormatterBench
    {
        /* netcoreapp3.1
                                         Method |            Mean
        --------------------------------------- |----------------
                  ILGen_SmallObjectSerialize10K |       469.52 us
                ILGen_SmallObjectDeserialize10K |       894.33 us
             ILGen_Auto_SmallObjectSerialize10K |     1,059.54 us
                       ILGen_VectorSerialize10K |       145.73 us
                     ILGen_VectorDeserialize10K |       189.76 us
                  ILGen_Auto_VectorSerialize10K |       349.87 us
                          ILGen_IntSerialize10K |        68.20 us
                        ILGen_IntDeserialize10K |        60.84 us
                     ILGen_Auto_IntSerialize10K |       182.11 us
                            PackIntSerialize10K |        37.97 us
                          PackIntDeserialize10K |        30.38 us
             ILGen_SmallObjectArraySerialize10K |   427,456.17 us
           ILGen_SmallObjectArrayDeserialize10K |   877,289.80 us
        ILGen_Auto_SmallObjectArraySerialize10K |   974,992.79 us
                  ILGen_VectorArraySerialize10K |       417.39 us
                ILGen_VectorArrayDeserialize10K |       966.85 us
             ILGen_Auto_VectorArraySerialize10K |     1,123.19 us
                     ILGen_IntArraySerialize10K |       833.65 us
                   ILGen_IntArrayDeserialize10K |       433.98 us
                ILGen_Auto_IntArraySerialize10K |     2,874.74 us
                     ILGen_LargeStringSerialize |   344,236.84 us
                   ILGen_LargeStringDeserialize | 2,351,884.98 us
                ILGen_Auto_LargeStringSerialize | 1,673,592.76 us
        */

        /* net4.6.1
                                         Method |            Mean
        --------------------------------------- |----------------
                  ILGen_SmallObjectSerialize10K |       610.88 us
                ILGen_SmallObjectDeserialize10K |     1,043.60 us
             ILGen_Auto_SmallObjectSerialize10K |     1,411.37 us
                       ILGen_VectorSerialize10K |       145.52 us
                     ILGen_VectorDeserialize10K |       159.96 us
                  ILGen_Auto_VectorSerialize10K |       276.04 us
                          ILGen_IntSerialize10K |        62.07 us
                        ILGen_IntDeserialize10K |        56.58 us
                     ILGen_Auto_IntSerialize10K |       165.58 us
                            PackIntSerialize10K |        34.85 us
                          PackIntDeserialize10K |        30.32 us
             ILGen_SmallObjectArraySerialize10K |   547,066.33 us
           ILGen_SmallObjectArrayDeserialize10K |   948,959.85 us
        ILGen_Auto_SmallObjectArraySerialize10K | 1,385,727.33 us
                  ILGen_VectorArraySerialize10K |       375.05 us
                ILGen_VectorArrayDeserialize10K |       987.88 us
             ILGen_Auto_VectorArraySerialize10K |     1,214.24 us
                     ILGen_IntArraySerialize10K |       717.80 us
                   ILGen_IntArrayDeserialize10K |       482.71 us
                ILGen_Auto_IntArraySerialize10K |     2,929.60 us
                     ILGen_LargeStringSerialize | 1,518,598.64 us
                   ILGen_LargeStringDeserialize | 3,454,324.89 us
                ILGen_Auto_LargeStringSerialize | 3,887,686.05 us
        */

        static ReBuffer buffer;

        Person[] personArray;
        ReBuffer personBuffer;
        ReBuffer personArrayBuffer;

        Vector[] vectorArray;
        ReBuffer vectorArrayBuffer;

        int[] intArray;
        ReBuffer intArrayBuffer;

        string largeString;
        ReBuffer largeStringBuffer;

        Person p = new Person
        {
            Age = 99999,
            FirstName = "Windows",
            LastName = "Server",
            Sex = Sex.Male,
        };

        Vector vector = new Vector { X = 1.234f, Y = 2.345f, Z = 3.456f };
        ReBuffer vectorBuffer;

        ReBuffer intBuffer;

        public ZeroFormatterBench()
        {
            buffer = new ReBuffer(1 << 24);

            {
                largeString = System.IO.File.ReadAllText("CSharpHtml.txt");
                largeStringBuffer = new ReBuffer(1 << 24);
                RePacking.Pack(largeStringBuffer, ref largeString);
            }

            {
                personArray = Enumerable.Range(1000, 1000).Select(e => new Person { Age = e, FirstName = "Windows", LastName = "Server", Sex = Sex.Female }).ToArray();

                personBuffer = new ReBuffer(1024);
                RePacking.Pack<Person>(personBuffer, ref p);

                personArrayBuffer = new ReBuffer(1 << 24);
                RePacking.Pack(personArrayBuffer, ref personArray);
            }

            {
                vectorBuffer = new ReBuffer(1024);
                RePacking.Pack<Vector>(vectorBuffer, ref vector);
            }

            {
                vectorArray = Enumerable.Range(1, 100).Select(_ => new Vector { X = 12345.12345f, Y = 3994.35226f, Z = 325125.52426f }).ToArray();
                vectorArrayBuffer = new ReBuffer(1 << 24);
                RePacking.Pack(vectorArrayBuffer, ref vectorArray);
            }

            {
                int val = 123456789;
                intBuffer = new ReBuffer(1024);
                RePacking.Pack<int>(intBuffer, ref val);
            }

            {
                intArray = Enumerable.Range(0, 1000).ToArray();
                intArrayBuffer = new ReBuffer(sizeof(int) * 1100);
                RePacking.Pack(intArrayBuffer, ref intArray);
            }
        }

        [Benchmark]
        public void ILGen_SmallObjectSerialize10K()
        {
            buffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack<Person>(buffer, ref p);
                buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_SmallObjectDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var _ = RePacking.Unpack<Person>(personBuffer);
                personBuffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_Auto_SmallObjectSerialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack<Person>(ref p);
            }
        }

        [Benchmark]
        public void ILGen_VectorSerialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack<Vector>(buffer, ref vector);
                buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_VectorDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var _ = RePacking.Unpack<Vector>(vectorBuffer);
                vectorBuffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_Auto_VectorSerialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack<Vector>(ref vector);
            }
        }

        [Benchmark]
        public void ILGen_IntSerialize10K()
        {
            buffer.Reset();
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack<int>(buffer, ref val);
                buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_IntDeserialize10K()
        {
            intBuffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                var _ = RePacking.Unpack<int>(intBuffer);
                intBuffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_Auto_IntSerialize10K()
        {
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                var _ = RePacking.Pack<int>(ref val);
            }
        }

        [Benchmark]
        public void PackIntSerialize10K()
        {
            buffer.Reset();
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                buffer.Pack(ref val);
                buffer.Reset();
            }

        }

        [Benchmark]
        public void PackIntDeserialize10K()
        {
            buffer.Reset();
            int val = 123456789;
            buffer.Pack(ref val);

            for (int i = 0; i < 10_000; i++)
            {
                buffer.Unpack<int>(out int _);
                buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_SmallObjectArraySerialize10K()
        {
            buffer.Reset();

            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack(buffer, ref personArray);
                buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_SmallObjectArrayDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var p = RePacking.Unpack<Person[]>(personArrayBuffer);
                personArrayBuffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_Auto_SmallObjectArraySerialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack(ref personArray);
            }
        }

        [Benchmark]
        public void ILGen_VectorArraySerialize10K()
        {
            buffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack(buffer, ref vectorArray);
                buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_VectorArrayDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var p = RePacking.Unpack<Vector[]>(vectorArrayBuffer);
                vectorArrayBuffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_Auto_VectorArraySerialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack(ref vectorArray);
            }
        }

        [Benchmark]
        public void ILGen_IntArraySerialize10K()
        {
            buffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack(buffer, ref intArray);
                buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_IntArrayDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var p = RePacking.Unpack<int[]>(vectorArrayBuffer);
                vectorArrayBuffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_Auto_IntArraySerialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack(ref intArray);
            }
        }

        [Benchmark]
        public void ILGen_LargeStringSerialize()
        {
            for (int i = 0; i < 10_000; i++)
            {
                buffer.Reset();
                RePacking.Pack(buffer, ref largeString);
            }
        }

        [Benchmark]
        public void ILGen_LargeStringDeserialize()
        {
            for (int i = 0; i < 10_000; i++)
            {
                largeStringBuffer.Reset();
                var _ = RePacking.Unpack<string>(largeStringBuffer);
            }
        }

        [Benchmark]
        public void ILGen_Auto_LargeStringSerialize()
        {
            for (int i = 0; i < 10_000; i++)
            {
                RePacking.Pack(ref largeString);
            }
        }

        public enum Sex : sbyte
        {
            Unknown, Male, Female,
        }

        [RePacker(false)]
        public class Person
        {
            [RePack]
            public virtual int Age { get; set; }
            [RePack]
            public virtual string FirstName { get; set; }
            [RePack]
            public virtual string LastName { get; set; }
            [RePack]
            public virtual Sex Sex { get; set; }
        }

        [RePacker]
        public struct Vector
        {
            public float X;
            public float Y;
            public float Z;
        }

        [RePacker]
        public struct VectorContainer
        {
            public IList<Vector> Vectors;
        }
    }
}
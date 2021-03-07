
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
                  ILGen_SmallObjectSerialize10K |       490.27 us
                ILGen_SmallObjectDeserialize10K |       923.34 us
             ILGen_Auto_SmallObjectSerialize10K |       990.48 us
                       ILGen_VectorSerialize10K |       157.06 us
                     ILGen_VectorDeserialize10K |       170.06 us
                  ILGen_Auto_VectorSerialize10K |       292.60 us
                          ILGen_IntSerialize10K |        66.02 us
                        ILGen_IntDeserialize10K |        54.38 us
                     ILGen_Auto_IntSerialize10K |       186.27 us
                            PackIntSerialize10K |        41.46 us
                          PackIntDeserialize10K |        31.39 us
             ILGen_SmallObjectArraySerialize10K |   444,948.51 us
           ILGen_SmallObjectArrayDeserialize10K |   913,954.46 us
        ILGen_Auto_SmallObjectArraySerialize10K |   875,662.90 us
                  ILGen_VectorArraySerialize10K |       551.38 us
                ILGen_VectorArrayDeserialize10K |     1,153.95 us
             ILGen_Auto_VectorArraySerialize10K |     1,364.21 us
                     ILGen_IntArraySerialize10K |       869.84 us
                   ILGen_IntArrayDeserialize10K |       507.00 us
                ILGen_Auto_IntArraySerialize10K |     3,635.05 us
                     ILGen_LargeStringSerialize |   349,815.43 us
                   ILGen_LargeStringDeserialize | 2,425,005.47 us
                ILGen_Auto_LargeStringSerialize | 1,728,142.72 us
        */

        /* net4.6.1
                                         Method |            Mean
        --------------------------------------- |----------------
                  ILGen_SmallObjectSerialize10K |       603.29 us
                ILGen_SmallObjectDeserialize10K |     1,049.03 us
             ILGen_Auto_SmallObjectSerialize10K |     1,136.67 us
                       ILGen_VectorSerialize10K |       148.27 us
                     ILGen_VectorDeserialize10K |       163.87 us
                  ILGen_Auto_VectorSerialize10K |       295.62 us
                          ILGen_IntSerialize10K |        60.33 us
                        ILGen_IntDeserialize10K |        59.03 us
                     ILGen_Auto_IntSerialize10K |       181.51 us
                            PackIntSerialize10K |        36.37 us
                          PackIntDeserialize10K |        30.47 us
             ILGen_SmallObjectArraySerialize10K |   558,122.50 us
           ILGen_SmallObjectArrayDeserialize10K |   978,614.09 us
        ILGen_Auto_SmallObjectArraySerialize10K | 1,117,109.08 us
                  ILGen_VectorArraySerialize10K |       388.54 us
                ILGen_VectorArrayDeserialize10K |     1,199.36 us
             ILGen_Auto_VectorArraySerialize10K |     1,381.09 us
                     ILGen_IntArraySerialize10K |       735.23 us
                   ILGen_IntArrayDeserialize10K |       558.60 us
                ILGen_Auto_IntArraySerialize10K |     3,756.75 us
                     ILGen_LargeStringSerialize | 1,519,634.85 us
                   ILGen_LargeStringDeserialize | 3,488,777.75 us
                ILGen_Auto_LargeStringSerialize | 3,916,041.86 us
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

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
        ------------------------------------ |----------------
               ILGen_SmallObjectSerialize10K |       465.48 us
             ILGen_SmallObjectDeserialize10K |       845.22 us
                    ILGen_VectorSerialize10K |       131.45 us
                  ILGen_VectorDeserialize10K |       152.72 us
                       ILGen_IntSerialize10K |        63.37 us
                     ILGen_IntDeserialize10K |        63.35 us
                             IntSerialize10K |        28.93 us
                           IntDeserialize10K |        28.27 us
                         PackIntSerialize10K |        73.24 us
                       PackIntDeserialize10K |        30.68 us
          ILGen_SmallObjectArraySerialize10K |   418,014.42 us
        ILGen_SmallObjectArrayDeserialize10K |   807,005.58 us
               ILGen_VectorArraySerialize10K |       902.92 us
             ILGen_VectorArrayDeserialize10K |     1,129.75 us
                  ILGen_IntArraySerialize10K |       946.20 us
                ILGen_IntArrayDeserialize10K |       658.83 us
                  ILGen_LargeStringSerialize |   342,157.91 us
                ILGen_LargeStringDeserialize | 2,350,264.08 us
        */

        /* net4.6.1
                                      Method |            Mean
        ------------------------------------ |----------------
               ILGen_SmallObjectSerialize10K |       605.04 us
             ILGen_SmallObjectDeserialize10K |       969.37 us
                    ILGen_VectorSerialize10K |       131.59 us
                  ILGen_VectorDeserialize10K |       149.86 us
                       ILGen_IntSerialize10K |        57.65 us
                     ILGen_IntDeserialize10K |        66.09 us
                             IntSerialize10K |        27.56 us
                           IntDeserialize10K |        26.70 us
                         PackIntSerialize10K |        30.55 us
                       PackIntDeserialize10K |        28.33 us
          ILGen_SmallObjectArraySerialize10K |   550,697.77 us
        ILGen_SmallObjectArrayDeserialize10K |   961,492.53 us
               ILGen_VectorArraySerialize10K |       886.19 us
             ILGen_VectorArrayDeserialize10K |     1,224.39 us
                  ILGen_IntArraySerialize10K |     1,300.44 us
                ILGen_IntArrayDeserialize10K |       712.72 us
                  ILGen_LargeStringSerialize | 1,516,295.97 us
                ILGen_LargeStringDeserialize | 3,420,358.10 us

                                      Method |            Mean
        ------------------------------------ |----------------
               ILGen_SmallObjectSerialize10K |     2,097.11 us
             ILGen_SmallObjectDeserialize10K |     2,559.01 us
                    ILGen_VectorSerialize10K |     1,309.62 us
                  ILGen_VectorDeserialize10K |     1,330.98 us
                       ILGen_IntSerialize10K |        55.16 us
                     ILGen_IntDeserialize10K |        58.75 us
                             IntSerialize10K |        29.26 us
                           IntDeserialize10K |        28.86 us
                         PackIntSerialize10K |       416.97 us
                       PackIntDeserialize10K |       413.60 us
          ILGen_SmallObjectArraySerialize10K | 1,994,053.45 us
        ILGen_SmallObjectArrayDeserialize10K | 2,552,074.20 us
               ILGen_VectorArraySerialize10K |     1,192.39 us
             ILGen_VectorArrayDeserialize10K |     1,821.62 us
                  ILGen_IntArraySerialize10K |     1,555.60 us
                ILGen_IntArrayDeserialize10K |     1,309.03 us
                  ILGen_LargeStringSerialize | 1,515,993.05 us
                ILGen_LargeStringDeserialize | 3,450,938.04 us
        */

        static Buffer buffer;

        Person[] personArray;
        Buffer personBuffer;
        Buffer personArrayBuffer;

        Vector[] vectorArray;
        Buffer vectorArrayBuffer;

        int[] intArray;
        Buffer intArrayBuffer;

        string largeString;
        Buffer largeStringBuffer;

        Person p = new Person
        {
            Age = 99999,
            FirstName = "Windows",
            LastName = "Server",
            Sex = Sex.Male,
        };

        Vector vector = new Vector { X = 1.234f, Y = 2.345f, Z = 3.456f };
        Buffer vectorBuffer;

        Buffer intBuffer;

        public ZeroFormatterBench()
        {
            buffer = new Buffer(1 << 24);

            {
                largeString = System.IO.File.ReadAllText("CSharpHtml.txt");
                largeStringBuffer = new Buffer(1 << 24);
                RePacking.Pack(largeStringBuffer, ref largeString);
            }

            {
                personArray = Enumerable.Range(1000, 1000).Select(e => new Person { Age = e, FirstName = "Windows", LastName = "Server", Sex = Sex.Female }).ToArray();

                personBuffer = new Buffer(1024);
                RePacking.Pack<Person>(personBuffer, ref p);

                personArrayBuffer = new Buffer(1 << 24);
                RePacking.Pack(personArrayBuffer, ref personArray);
            }

            {
                vectorBuffer = new Buffer(1024);
                RePacking.Pack<Vector>(vectorBuffer, ref vector);
            }

            {
                vectorArray = Enumerable.Range(1, 100).Select(_ => new Vector { X = 12345.12345f, Y = 3994.35226f, Z = 325125.52426f }).ToArray();
                vectorArrayBuffer = new Buffer(1 << 24);
                RePacking.Pack(vectorArrayBuffer, ref vectorArray);
            }

            {
                int val = 123456789;
                intBuffer = new Buffer(1024);
                RePacking.Pack<int>(intBuffer, ref val);
            }

            {
                intArray = Enumerable.Range(0, 1000).ToArray();
                intArrayBuffer = new Buffer(sizeof(int) * 1100);
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

        /* [Benchmark]
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
        } */

        /* [Benchmark]
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
        } */

        /* [Benchmark]
        public void IntSerialize10K()
        {
            buffer.Reset();
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                buffer.PushInt(ref val);
                buffer.Reset();
            }

        }

        [Benchmark]
        public void IntDeserialize10K()
        {
            buffer.Reset();
            int val = 123456789;
            buffer.Pack(ref val);

            for (int i = 0; i < 10_000; i++)
            {
                buffer.PopInt(out int _);
                buffer.Reset();
            }
        } */

        /* [Benchmark]
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
        } */

        /* [Benchmark]
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
        } */

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
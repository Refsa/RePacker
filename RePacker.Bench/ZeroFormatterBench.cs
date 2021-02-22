
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
               ILGen_SmallObjectSerialize10K |       463.22 us
             ILGen_SmallObjectDeserialize10K |       870.49 us
                    ILGen_VectorSerialize10K |       119.37 us
                  ILGen_VectorDeserialize10K |       135.90 us
                       ILGen_IntSerialize10K |        51.54 us
                     ILGen_IntDeserialize10K |        51.49 us
                             IntSerialize10K |        30.93 us
                           IntDeserialize10K |        28.42 us
                         PackIntSerialize10K |        63.23 us
                       PackIntDeserialize10K |        30.40 us
          ILGen_SmallObjectArraySerialize10K |   420,579.11 us
        ILGen_SmallObjectArrayDeserialize10K |   773,566.65 us
               ILGen_VectorArraySerialize10K |       659.67 us
             ILGen_VectorArrayDeserialize10K |     1,220.17 us
                  ILGen_IntArraySerialize10K |       948.97 us
                ILGen_IntArrayDeserialize10K |       654.16 us
                  ILGen_LargeStringSerialize |   339,452.53 us
                ILGen_LargeStringDeserialize | 1,996,564.20 us
        */

        /* net4.6.1
                                      Method |            Mean
        ------------------------------------ |----------------
               ILGen_SmallObjectSerialize10K |       573.19 us
             ILGen_SmallObjectDeserialize10K |       977.45 us
                    ILGen_VectorSerialize10K |       119.48 us
                  ILGen_VectorDeserialize10K |       142.76 us
                       ILGen_IntSerialize10K |        49.17 us
                     ILGen_IntDeserialize10K |        51.57 us
                             IntSerialize10K |        27.43 us
                           IntDeserialize10K |        26.57 us
                         PackIntSerialize10K |        30.79 us
                       PackIntDeserialize10K |        28.23 us
          ILGen_SmallObjectArraySerialize10K |   529,452.41 us
        ILGen_SmallObjectArrayDeserialize10K |   962,823.00 us
               ILGen_VectorArraySerialize10K |       858.31 us
             ILGen_VectorArrayDeserialize10K |     1,287.13 us
                  ILGen_IntArraySerialize10K |     1,389.04 us
                ILGen_IntArrayDeserialize10K |       735.67 us
                  ILGen_LargeStringSerialize | 1,506,050.15 us
                ILGen_LargeStringDeserialize | 3,069,118.35 us
        */

        static byte[] backingBuffer;
        static Buffer buffer;
        static BoxedBuffer boxedBuffer;

        Person[] personArray;
        BoxedBuffer personBoxedBuffer;
        BoxedBuffer personArrayBoxedBuffer;

        Vector[] vectorArray;
        BoxedBuffer vectorArrayBoxedBuffer;

        int[] intArray;
        BoxedBuffer intArrayBuffer;

        string largeString;
        BoxedBuffer largeStringBoxedBuffer;

        Person p = new Person
        {
            Age = 99999,
            FirstName = "Windows",
            LastName = "Server",
            Sex = Sex.Male,
        };

        Vector vector = new Vector { X = 1.234f, Y = 2.345f, Z = 3.456f };
        BoxedBuffer vectorBuffer;

        BoxedBuffer intBuffer;

        public ZeroFormatterBench()
        {
            backingBuffer = new byte[1 << 24];
            buffer = new Buffer(backingBuffer, 0);
            boxedBuffer = new BoxedBuffer(1 << 24);

            {
                largeString = System.IO.File.ReadAllText("CSharpHtml.txt");
                largeStringBoxedBuffer = new BoxedBuffer(1 << 24);
                largeStringBoxedBuffer.Pack(ref largeString);
            }

            {
                personArray = Enumerable.Range(1000, 1000).Select(e => new Person { Age = e, FirstName = "Windows", LastName = "Server", Sex = Sex.Female }).ToArray();

                personBoxedBuffer = new BoxedBuffer(1024);
                RePacker.Pack<Person>(personBoxedBuffer, ref p);

                personArrayBoxedBuffer = new BoxedBuffer(1 << 24);
                personArrayBoxedBuffer.Pack(ref personArray);
            }

            {
                vectorBuffer = new BoxedBuffer(1024);
                RePacker.Pack<Vector>(vectorBuffer, ref vector);
            }

            {
                vectorArray = Enumerable.Range(1, 100).Select(_ => new Vector { X = 12345.12345f, Y = 3994.35226f, Z = 325125.52426f }).ToArray();
                vectorArrayBoxedBuffer = new BoxedBuffer(1 << 24);
                vectorArrayBoxedBuffer.Pack(ref vectorArray);
            }

            {
                int val = 123456789;
                intBuffer = new BoxedBuffer(1024);
                RePacker.Pack<int>(intBuffer, ref val);
            }

            {
                intArray = Enumerable.Range(0, 1000).ToArray();
                intArrayBuffer = new BoxedBuffer(sizeof(int) * 1100);
                intArrayBuffer.Pack(ref intArray);
            }
        }

        [Benchmark]
        public void ILGen_SmallObjectSerialize10K()
        {
            boxedBuffer.Buffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack<Person>(boxedBuffer, ref p);
                // boxedBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_SmallObjectDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var _ = RePacker.Unpack<Person>(personBoxedBuffer);
                personBoxedBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_VectorSerialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack<Vector>(boxedBuffer, ref vector);
                boxedBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_VectorDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var _ = RePacker.Unpack<Vector>(vectorBuffer);
                vectorBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_IntSerialize10K()
        {
            boxedBuffer.Buffer.Reset();
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack<int>(boxedBuffer, ref val);
                boxedBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_IntDeserialize10K()
        {
            intBuffer.Buffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                var _ = RePacker.Unpack<int>(intBuffer);
                intBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
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
                buffer.Unpack(out int _);
                buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_SmallObjectArraySerialize10K()
        {
            boxedBuffer.Reset();

            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack(boxedBuffer, ref personArray);
                boxedBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_SmallObjectArrayDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var p = RePacker.Unpack<Person[]>(personArrayBoxedBuffer);
                personArrayBoxedBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_VectorArraySerialize10K()
        {
            boxedBuffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack(boxedBuffer, ref vectorArray);
                boxedBuffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_VectorArrayDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var p = RePacker.Unpack<Vector[]>(vectorArrayBoxedBuffer);
                vectorArrayBoxedBuffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_IntArraySerialize10K()
        {
            boxedBuffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack(boxedBuffer, ref intArray);
                boxedBuffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_IntArrayDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var p = RePacker.Unpack<int[]>(vectorArrayBoxedBuffer);
                vectorArrayBoxedBuffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_LargeStringSerialize()
        {
            for (int i = 0; i < 10_000; i++)
            {
                boxedBuffer.Reset();
                boxedBuffer.Pack(ref largeString);
            }
        }

        [Benchmark]
        public void ILGen_LargeStringDeserialize()
        {
            for (int i = 0; i < 10_000; i++)
            {
                largeStringBoxedBuffer.Reset();
                var _ = largeStringBoxedBuffer.Unpack<string>();
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
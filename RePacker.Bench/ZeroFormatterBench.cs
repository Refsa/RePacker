
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
               ILGen_SmallObjectSerialize10K |       452.35 us
             ILGen_SmallObjectDeserialize10K |       877.94 us
                    ILGen_VectorSerialize10K |        92.80 us
                  ILGen_VectorDeserialize10K |       154.12 us
                       ILGen_IntSerialize10K |        51.37 us
                     ILGen_IntDeserialize10K |       102.73 us
                             IntSerialize10K |        21.38 us
                           IntDeserialize10K |        18.21 us
                         PackIntSerialize10K |        21.36 us
                       PackIntDeserialize10K |        18.24 us
          ILGen_SmallObjectArraySerialize10K |   409,596.86 us
        ILGen_SmallObjectArrayDeserialize10K |   779,397.85 us
               ILGen_VectorArraySerialize10K |       646.22 us
             ILGen_VectorArrayDeserialize10K |     1,401.55 us
                  ILGen_IntArraySerialize10K |       928.24 us
                ILGen_IntArrayDeserialize10K |       716.96 us
                  ILGen_LargeStringSerialize |   339,940.90 us
                ILGen_LargeStringDeserialize | 1,995,480.99 us
        */

        /* net4.6.1
                                      Method |            Mean
        ------------------------------------ |----------------
               ILGen_SmallObjectSerialize10K |       656.38 us
             ILGen_SmallObjectDeserialize10K |       974.51 us
                    ILGen_VectorSerialize10K |       212.66 us
                  ILGen_VectorDeserialize10K |       218.84 us
                       ILGen_IntSerialize10K |       105.20 us
                     ILGen_IntDeserialize10K |       108.01 us
                             IntSerialize10K |        23.56 us
                           IntDeserialize10K |        18.92 us
                         PackIntSerialize10K |        21.75 us
                       PackIntDeserialize10K |        18.93 us
          ILGen_SmallObjectArraySerialize10K |   562,578.19 us
        ILGen_SmallObjectArrayDeserialize10K |   916,597.95 us
               ILGen_VectorArraySerialize10K |       921.05 us
             ILGen_VectorArrayDeserialize10K |     1,359.91 us
                  ILGen_IntArraySerialize10K |     1,301.56 us
                ILGen_IntArrayDeserialize10K |       792.07 us
                  ILGen_LargeStringSerialize | 1,503,845.62 us
                ILGen_LargeStringDeserialize | 3,051,454.12 us
        */

        static byte[] backingBuffer;
        static Buffer buffer;
        static BoxedBuffer boxedBuffer;

        Person[] personArray;
        BoxedBuffer personBoxedBuffer;
        BoxedBuffer personArrayBoxedBuffer;

        Vector[] vectorArray;
        BoxedBuffer vectorArrayBoxedBuffer;
        VectorContainer vectorContainer;
        BoxedBuffer vectorContainerBuffer;

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

                personArrayBoxedBuffer = new BoxedBuffer(1 << 16);
                personArrayBoxedBuffer.Pack(ref personArray);
            }

            {
                vectorBuffer = new BoxedBuffer(1024);
                RePacker.Pack<Vector>(vectorBuffer, ref vector);
            }

            {
                vectorArray = Enumerable.Range(1, 100).Select(_ => new Vector { X = 12345.12345f, Y = 3994.35226f, Z = 325125.52426f }).ToArray();
                vectorArrayBoxedBuffer = new BoxedBuffer(2048);
                vectorArrayBoxedBuffer.Pack(ref vectorArray);

                vectorContainer = new VectorContainer { Vectors = vectorArray };
                vectorContainerBuffer = new BoxedBuffer(4096);
                vectorContainerBuffer.Pack(ref vectorContainer);
            }

            {
                int val = 123456789;
                intBuffer = new BoxedBuffer(1024);
                RePacker.Pack<int>(intBuffer, ref val);
            }

            {
                intArray = Enumerable.Range(0, 1000).ToArray();
                intArrayBuffer = new BoxedBuffer(sizeof(int) * 1002);
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
            var buffer = new BoxedBuffer(1 << 16);

            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack(buffer, ref personArray);
                buffer.Buffer.Reset();
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
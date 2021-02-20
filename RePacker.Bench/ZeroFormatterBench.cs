
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Builder;

namespace Refsa.RePacker.Benchmarks
{
    [MemoryDiagnoser]
    public class ZeroFormatterBench
    {
        /* netcoreapp3.1
                                      Method |            Mean
        ------------------------------------ |----------------
               ILGen_SmallObjectSerialize10K |       727.38 us
             ILGen_SmallObjectDeserialize10K |     1,128.23 us
                    ILGen_VectorSerialize10K |        96.32 us
                  ILGen_VectorDeserialize10K |       155.38 us
                       ILGen_IntSerialize10K |        51.53 us
                     ILGen_IntDeserialize10K |       103.11 us
                             IntSerialize10K |        21.49 us
                           IntDeserialize10K |        18.28 us
                         PackIntSerialize10K |        21.48 us
                       PackIntDeserialize10K |        18.28 us
          ILGen_SmallObjectArraySerialize10K |   691,455.41 us
        ILGen_SmallObjectArrayDeserialize10K | 1,089,353.67 us
               ILGen_VectorArraySerialize10K |       677.41 us
             ILGen_VectorArrayDeserialize10K |     1,548.71 us
                  ILGen_IntArraySerialize10K |     1,018.70 us
                ILGen_IntArrayDeserialize10K |     1,044.99 us
                  ILGen_LargeStringSerialize |   342,457.98 us
                ILGen_LargeStringDeserialize | 2,004,193.04 us
        */

        /* net4.6.1 - latest
                                      Method |            Mean
        ------------------------------------ |----------------
               ILGen_SmallObjectSerialize10K |     1,082.88 us
             ILGen_SmallObjectDeserialize10K |     1,396.81 us
                    ILGen_VectorSerialize10K |       217.88 us
                  ILGen_VectorDeserialize10K |       235.53 us
                       ILGen_IntSerialize10K |       105.54 us
                     ILGen_IntDeserialize10K |       105.72 us
                             IntSerialize10K |        21.82 us
                           IntDeserialize10K |        17.69 us
                         PackIntSerialize10K |        21.95 us
                       PackIntDeserialize10K |        17.67 us
          ILGen_SmallObjectArraySerialize10K | 1,002,928.50 us
        ILGen_SmallObjectArrayDeserialize10K | 1,395,526.24 us
               ILGen_VectorArraySerialize10K |       928.49 us
             ILGen_VectorArrayDeserialize10K |     1,580.59 us
                  ILGen_IntArraySerialize10K |     1,412.97 us
                ILGen_IntArrayDeserialize10K |     1,006.31 us
                  ILGen_LargeStringSerialize | 1,510,192.95 us
                ILGen_LargeStringDeserialize | 3,074,888.27 us
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
            buffer.Push(ref val);

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
            buffer.Push(ref val);

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
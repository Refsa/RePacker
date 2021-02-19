
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
        /* netcoreapp3.0
                                      Method |            Mean |         Error |        StdDev |       Gen 0 | Gen 1 | Gen 2 |    Allocated |
        ------------------------------------ |----------------:|--------------:|--------------:|------------:|------:|------:|-------------:|
                     SmallObjectSerialize10K |     2,390.44 us |      8.205 us |      7.274 us |    203.1250 |     - |     - |     640039 B |
               ILGen_SmallObjectSerialize10K |     1,718.52 us |      3.855 us |      3.417 us |    203.1250 |     - |     - |     640003 B |
                   SmallObjectDeserialize10K |     2,977.74 us |     11.231 us |      8.768 us |    378.9063 |     - |     - |    1200063 B |
             ILGen_SmallObjectDeserialize10K |     1,963.73 us |      7.465 us |      6.982 us |    380.8594 |     - |     - |    1200000 B |
                    ILGen_VectorSerialize10K |       443.76 us |      1.046 us |      0.979 us |           - |     - |     - |          3 B |
                  ILGen_VectorDeserialize10K |       474.71 us |      1.645 us |      1.538 us |           - |     - |     - |          1 B |
                       ILGen_IntSerialize10K |       361.22 us |      1.433 us |      1.341 us |     20.5078 |     - |     - |      65609 B |
                     ILGen_IntDeserialize10K |       367.86 us |      1.867 us |      1.746 us |           - |     - |     - |            - |
                             IntSerialize10K |        63.08 us |      0.428 us |      0.400 us |     20.7520 |     - |     - |      65561 B |
                           IntDeserialize10K |        45.48 us |      0.127 us |      0.106 us |      0.3052 |     - |     - |       1048 B |
                SmallObjectArraySerialize10K | 2,402,383.46 us | 14,699.587 us | 13,750.003 us | 204000.0000 |     - |     - |  640097960 B |
              SmallObjectArrayDeserialize10K | 3,011,003.06 us |  9,063.164 us |  8,034.259 us | 408000.0000 |     - |     - | 1280302168 B |
          ILGen_SmallObjectArraySerialize10K | 1,447,606.43 us |  6,322.598 us |  5,914.163 us | 204000.0000 |     - |     - |  640066960 B |
        ILGen_SmallObjectArrayDeserialize10K | 1,778,030.16 us |  8,599.822 us |  8,044.278 us | 408000.0000 |     - |     - | 1280240000 B |
        */

        /* netcoreapp3.0
                                      Method |            Mean
        ------------------------------------ |----------------
               ILGen_SmallObjectSerialize10K |     1,607.45 us
             ILGen_SmallObjectDeserialize10K |     1,754.58 us
                    ILGen_VectorSerialize10K |       245.71 us
                  ILGen_VectorDeserialize10K |       251.05 us
                       ILGen_IntSerialize10K |       165.09 us
                     ILGen_IntDeserialize10K |       163.17 us
                             IntSerialize10K |        43.51 us
                           IntDeserialize10K |        40.91 us
          ILGen_SmallObjectArraySerialize10K | 1,374,717.61 us
        ILGen_SmallObjectArrayDeserialize10K | 1,756,913.44 us
        */

        /* net4.6.1
                                      Method |           Mean |
        ------------------------------------ |---------------:|
               ILGen_SmallObjectSerialize10K |     2,508.0 us |
             ILGen_SmallObjectDeserialize10K |     3,196.8 us |
                    ILGen_VectorSerialize10K |       601.1 us |
                  ILGen_VectorDeserialize10K |       633.6 us |
                       ILGen_IntSerialize10K |       328.1 us |
                     ILGen_IntDeserialize10K |       326.8 us |
                             IntSerialize10K |       146.6 us |
                           IntDeserialize10K |       144.5 us |
          ILGen_SmallObjectArraySerialize10K | 2,349,831.5 us |
        ILGen_SmallObjectArrayDeserialize10K | 2,591,480.2 us |
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
            backingBuffer = new byte[1024];
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

        /* [Benchmark]
        public void ILGen_SmallObjectSerialize10K()
        {
            boxedBuffer.Buffer.Reset();
            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack<Person>(boxedBuffer, ref p);
                // boxedBuffer.Buffer.Reset();
            }
        } */

        [Benchmark]
        public void ILGen_SmallObjectDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var _ = RePacker.Unpack<Person>(personBoxedBuffer);
                personBoxedBuffer.Buffer.Reset();
            }
        }

        /* [Benchmark]
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
        } */

        /* [Benchmark]
        public void ILGen_IntSerialize10K()
        {
            var buffer = new BoxedBuffer(1 << 16);
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack<int>(buffer, ref val);
            }

            // buffer.Buffer.Reset();
        }

        [Benchmark]
        public void ILGen_IntDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var _ = RePacker.Unpack<int>(intBuffer);
                intBuffer.Buffer.Reset();
            }
        } */

        /* [Benchmark]
        public void IntSerialize10K()
        {
            var buffer = new Buffer(new byte[1 << 16]);
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                buffer.PushInt(ref val);
            }

            // buffer.Reset();
        }

        [Benchmark]
        public void IntDeserialize10K()
        {
            var buffer = new Buffer(new byte[1024]);
            int val = 123456789;
            buffer.Push(ref val);

            for (int i = 0; i < 10_000; i++)
            {
                buffer.PopInt(out int _);
                buffer.Reset();
            }
        } */

        /* [Benchmark]
        public void ILGen_SmallObjectArraySerialize10K()
        {
            var buffer = new BoxedBuffer(1 << 16);

            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack(buffer, ref personArray);
                buffer.Buffer.Reset();
            }
        } */

        /* [Benchmark]
        public void ILGen_SmallObjectArrayDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var p = RePacker.Unpack<Person[]>(personArrayBoxedBuffer);
                personArrayBoxedBuffer.Buffer.Reset();
            }
        } */

        /* [Benchmark]
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
        } */

        /* [Benchmark]
        public void ILGen_LargeStringSerialize()
        {
            var buffer = new BoxedBuffer(1 << 24);

            buffer.Pack(ref largeString);
        }

        [Benchmark]
        public void ILGen_LargeStringDeserialize()
        {
            var _ = largeStringBoxedBuffer.Unpack<string>();
            largeStringBoxedBuffer.Reset();
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
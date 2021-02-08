

using System.Linq;
using BenchmarkDotNet.Attributes;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Benchmarks
{
    [MemoryDiagnoser]
    public class ZeroFormatterBench
    {
        /* @commit a76f7cd3fcab7e1e78a5184a2f993e4070fd40e8
                                      Method |            Mean |         Error |        StdDev |       Gen 0 |       Gen 1 | Gen 2 |    Allocated |
        ------------------------------------ |----------------:|--------------:|--------------:|------------:|------------:|------:|-------------:|
                     SmallObjectSerialize10K |     2,383.69 us |      4.897 us |      4.581 us |    203.1250 |           - |     - |     640043 B |
               ILGen_SmallObjectSerialize10K |     1,707.55 us |      5.995 us |      5.314 us |    203.1250 |           - |     - |     640003 B |
                   SmallObjectDeserialize10K |     2,978.39 us |      7.539 us |      7.052 us |    378.9063 |           - |     - |    1200060 B |
             ILGen_SmallObjectDeserialize10K |     1,934.69 us |     12.416 us |     11.614 us |    378.9063 |           - |     - |    1200036 B |
                    ILGen_VectorSerialize10K |       365.67 us |      0.990 us |      0.878 us |           - |           - |     - |            - |
                  ILGen_VectorDeserialize10K |       388.71 us |      3.484 us |      3.259 us |           - |           - |     - |            - |
                       ILGen_IntSerialize10K |       212.50 us |      0.418 us |      0.370 us |     20.7520 |           - |     - |      65608 B |
                     ILGen_IntDeserialize10K |       199.59 us |      0.575 us |      0.538 us |           - |           - |     - |          1 B |
                             IntSerialize10K |        62.13 us |      0.375 us |      0.351 us |     20.7520 |           - |     - |      65560 B |
                           IntDeserialize10K |        45.41 us |      0.300 us |      0.281 us |      0.3052 |           - |     - |       1048 B |
                SmallObjectArraySerialize10K | 2,361,170.74 us | 10,788.875 us | 10,091.920 us | 204000.0000 |           - |     - |  640097960 B |
              SmallObjectArrayDeserialize10K | 3,058,959.39 us | 23,121.066 us | 20,496.223 us | 408000.0000 |           - |     - | 1280302168 B |
          ILGen_SmallObjectArraySerialize10K | 1,457,126.79 us |  2,696.468 us |  2,522.278 us | 204000.0000 |           - |     - |  640066960 B |
        ILGen_SmallObjectArrayDeserialize10K | 1,888,190.82 us |  3,025.772 us |  2,526.656 us | 306000.0000 | 102000.0000 |     - | 1280240000 B |
        */

        /*
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

        static byte[] backingBuffer;
        static Buffer buffer;
        static BoxedBuffer boxedBuffer;

        Person[] personArray;
        Buffer personBuffer;
        BoxedBuffer personBoxedBuffer;
        Buffer personArrayBuffer;

        BoxedBuffer personArrayBoxedBuffer;
        PersonArrayContainer personArrayContainer;

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
            boxedBuffer = new BoxedBuffer(1024);

            {
                personArray = Enumerable.Range(1000, 1000).Select(e => new Person { Age = e, FirstName = "Windows", LastName = "Server", Sex = Sex.Female }).ToArray();

                var _personBuffer = new Buffer(new byte[1024], 0);
                _personBuffer.Pack(p);
                personBuffer = new Buffer(ref _personBuffer);

                personBoxedBuffer = new BoxedBuffer(1024);
                RePacker.Pack<Person>(personBoxedBuffer, ref p);

                var _personArrayBuffer = new Buffer(new byte[1 << 16], 0);
                _personArrayBuffer.EncodeArray<Person>(personArray);
                personArrayBuffer = new Buffer(ref _personArrayBuffer);

                personArrayBoxedBuffer = new BoxedBuffer(1 << 16);
                personArrayContainer = new PersonArrayContainer
                {
                    Persons = personArray
                };
                RePacker.Pack(personArrayBoxedBuffer, ref personArrayContainer);
            }

            {
                vectorBuffer = new BoxedBuffer(1024);
                RePacker.Pack<Vector>(vectorBuffer, ref vector);
            }

            {
                int val = 123456789;
                intBuffer = new BoxedBuffer(1024);
                RePacker.Pack<int>(intBuffer, ref val);
            }
        }

        public enum Sex : sbyte
        {
            Unknown, Male, Female,
        }

        [RePacker]
        public class Person : IPacker
        {
            public int Age;
            public string FirstName;
            public string LastName;
            public Sex Sex;

            public void FromBuffer(ref Buffer buffer)
            {
                buffer.Pop<int>(out Age);
                FirstName = buffer.UnpackString();
                LastName = buffer.UnpackString();
                Sex = buffer.UnpackEnum<Sex>();
            }

            public void ToBuffer(ref Buffer buffer)
            {
                buffer.Push<int>(ref Age);
                buffer.PackString(ref FirstName);
                buffer.PackString(ref LastName);
                buffer.PackEnum<Sex>(ref Sex);
            }
        }

        [RePacker]
        public struct PersonArrayContainer
        {
            public Person[] Persons;
        }

        [RePacker]
        public struct Vector
        {
            public float X;
            public float Y;
            public float Z;
        }

        [Benchmark]
        public void SmallObjectSerialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                buffer.Pack(p);
                buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_SmallObjectSerialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack<Person>(boxedBuffer, ref p);
                boxedBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void SmallObjectDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var _ = (Person)personBuffer.Unpack(typeof(Person));
                personBuffer.Reset();
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
        }

        [Benchmark]
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
        }

        [Benchmark]
        public void SmallObjectArraySerialize10K()
        {
            Buffer buffer = new Buffer(new byte[1 << 16], 0);

            for (int i = 0; i < 10_000; i++)
            {
                buffer.EncodeArray<Person>(personArray);
                buffer.Reset();
            }
        }

        [Benchmark]
        public void SmallObjectArrayDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var p = PackerExtensions.DecodeArray<Person>(ref personArrayBuffer);
                personArrayBuffer.Reset();
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
    }
}
using System;

using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

using Refsa.RePacker;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Unsafe;

using Buffer = Refsa.RePacker.Buffers.Buffer;
using System.Linq;
using System.Collections.Generic;
using Refsa.RePacker.Generator;

namespace Refsa.RePacker.Benchmarks
{
    [MemoryDiagnoser]
    public class BufferBench
    {
        static byte[] backingBuffer;
        static Buffer buffer;

        public BufferBench()
        {
            backingBuffer = new byte[1024];
            buffer = new Buffer(backingBuffer, 0);
        }

        [Benchmark]
        public void BufferPushPopInt()
        {
            for (int j = 0; j < 400; j++)
            {
                for (int i = 0; i < 1024 / sizeof(int); i++)
                {
                    int val = i;
                    buffer.Push<int>(ref val);
                }

                for (int i = 0; i < 1024 / sizeof(int); i++)
                {
                    int val = 0;
                    buffer.Pop<int>(out val);
                }

                buffer.Reset();
            }
        }

        [Benchmark]
        public void BufferPushPopFloat()
        {
            for (int j = 0; j < 400; j++)
            {
                for (int i = 0; i < 1024 / sizeof(float); i++)
                {
                    float val = i;
                    buffer.Push<float>(ref val);
                }

                for (int i = 0; i < 1024 / sizeof(float); i++)
                {
                    float val = 0;
                    buffer.Pop<float>(out val);
                }

                buffer.Reset();
            }
        }

        [Benchmark]
        public void BufferPushPopULong()
        {
            for (int j = 0; j < 400; j++)
            {
                for (int i = 0; i < 1024 / sizeof(ulong); i++)
                {
                    ulong val = (ulong)i;
                    buffer.Push<ulong>(ref val);
                }

                for (int i = 0; i < 1024 / sizeof(ulong); i++)
                {
                    ulong val = 0;
                    buffer.Pop<ulong>(out val);
                }

                buffer.Reset();
            }
        }

        struct TestBlittableStruct
        {
            public int Int1;
            public int Int2;
            public int Int3;
            public int Int4;
            public int Int5;
            public int Int6;
            public int Int7;
            public int Int8;
        }

        [Benchmark]
        public void BufferPushPopStruct()
        {
            for (int j = 0; j < 12_500; j++)
            {
                for (int i = 0; i < 1024 / System.Runtime.InteropServices.Marshal.SizeOf<TestBlittableStruct>(); i++)
                {
                    TestBlittableStruct val = new TestBlittableStruct();
                    buffer.Push<TestBlittableStruct>(ref val);
                }

                for (int i = 0; i < 1024 / System.Runtime.InteropServices.Marshal.SizeOf<TestBlittableStruct>(); i++)
                {
                    buffer.Pop<TestBlittableStruct>(out var val);
                }

                buffer.Reset();
            }
        }
    }

    [MemoryDiagnoser]
    public class ZeroFormatterBench
    {
        /* @commit b11167496426aa69b63ea2b8bcb3cc83209a0c20
                              Method |           Mean |        Error |       StdDev |       Gen 0 |       Gen 1 | Gen 2 |    Allocated |
------------------------------------ |---------------:|-------------:|-------------:|------------:|------------:|------:|-------------:|
             SmallObjectSerialize10K | 2,632.0 us |      8.39 us |      7.44 us |    203.1250 |           - |     - |     640036 B |
       ILGen_SmallObjectSerialize10K | 2,479.2 us |     17.65 us |     16.51 us |    203.1250 |           - |     - |     640002 B |
           SmallObjectDeserialize10K | 2,711.0 us |      7.88 us |      7.37 us |    378.9063 |           - |     - |    1200064 B |
     ILGen_SmallObjectDeserialize10K | 2,589.0 us |     18.09 us |     16.92 us |    378.9063 |           - |     - |    1200002 B |
            ILGen_VectorSerialize10K | 1,168.4 us |      3.19 us |      2.98 us |           - |           - |     - |         14 B |
          ILGen_VectorDeserialize10K | 1,181.2 us |      5.69 us |      5.04 us |           - |           - |     - |          1 B |
               ILGen_IntSerialize10K |   160.7 us |      0.31 us |      0.29 us |           - |           - |     - |            - |
             ILGen_IntDeserialize10K |   141.9 us |      0.34 us |      0.31 us |           - |           - |     - |            - |
        SmallObjectArraySerialize10K | 2,932.0 ms | 12,787.97 us | 11,961.88 us | 204000.0000 |           - |     - |  640096720 B |
      SmallObjectArrayDeserialize10K | 3,202.0 ms | 17,691.45 us | 15,683.01 us | 408000.0000 |           - |     - | 1280302168 B |
  ILGen_SmallObjectArraySerialize10K | 2,077.0 ms | 13,372.43 us | 12,508.58 us | 204000.0000 |           - |     - |  640065608 B |
ILGen_SmallObjectArrayDeserialize10K | 2,421.0 ms | 19,713.62 us | 18,440.13 us | 308000.0000 | 101000.0000 |     - | 1280240000 B |
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
                _personBuffer.Encode(p);
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
        public class Person : ISerializer
        {
            public int Age;
            public string FirstName;
            public string LastName;
            public Sex Sex;

            public void FromBuffer(ref Buffer buffer)
            {
                buffer.Pop<int>(out Age);
                FirstName = buffer.DecodeString();
                LastName = buffer.DecodeString();
                Sex = buffer.DecodeEnum<Sex>();
            }

            public void ToBuffer(ref Buffer buffer)
            {
                buffer.Push<int>(ref Age);
                buffer.EncodeString(ref FirstName);
                buffer.EncodeString(ref LastName);
                buffer.EncodeEnum<Sex>(ref Sex);
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
                buffer.Encode(p);
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
                var _ = (Person)personBuffer.Decode(typeof(Person));
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
            for (int i = 0; i < 10_000; i++)
            {
                var _ = RePacker.Unpack<int>(intBuffer);
                intBuffer.Buffer.Reset();
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
                var p = Serializer.DecodeArray<Person>(ref personArrayBuffer);
                personArrayBuffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_SmallObjectArraySerialize10K()
        {
            var buffer = new BoxedBuffer(1 << 16);

            for (int i = 0; i < 10_000; i++)
            {
                RePacker.Pack(buffer, ref personArrayContainer);
                buffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void ILGen_SmallObjectArrayDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var p = RePacker.Unpack<PersonArrayContainer>(personArrayBoxedBuffer);
                personArrayBoxedBuffer.Buffer.Reset();
            }
        }
    }

    [MemoryDiagnoser]
    public class ILGenerated
    {
        static byte[] backingBuffer = new byte[1024];
        const int RUNS = 100_000;

        [Benchmark]
        public void BenchStructILGen()
        {
            var ts2 = new Program.TestStruct2
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
                RePacker.Pack<Program.TestStruct2>(boxedBuffer, ref ts2);
                var _ = RePacker.Unpack<Program.TestStruct2>(boxedBuffer);

                boxedBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void BenchStructNonILGen()
        {
            var ts2 = new Program.TestStruct2
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
                buffer.Pop<Program.TestStruct2>(out var _);

                buffer.Reset();
            }
        }

        [Benchmark]
        public void BenchClassILGen()
        {
            var ts2 = new Program.TestClass2
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
                TypeCache.Serialize<Program.TestClass2>(boxedBuffer, ref ts2);
                var _ = TypeCache.Deserialize<Program.TestClass2>(boxedBuffer);

                boxedBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void BenchNestedTypesILGen()
        {
            var p = new Program.Parent
            {
                Float = 1.337f,
                ULong = 987654321,
                Child = new Program.Child
                {
                    Float = 10f,
                    Byte = 120,
                },
            };

            var buffer = new BoxedBuffer(backingBuffer);

            for (int i = 0; i < 100_000; i++)
            {
                RePacker.Pack<Program.Parent>(buffer, ref p);
                var fromBuf = RePacker.Unpack<Program.Parent>(buffer);
                buffer.Buffer.Reset();
            }
        }
    }

    public class Program
    {
        [RePacker]
        public struct TestStruct
        {
            public float Value1;
            public int Value2;
            public byte Value3;
        }

        [RePacker]
        public struct TestStruct2
        {
            public bool Bool;
            // public char Char;
            public sbyte Sbyte;
            public byte Byte;
            public short Short;
            public ushort Ushort;
            public int Int;
            public uint Uint;
            public long Long;
            public ulong Ulong;
            public float Float;
            public double Double;
            public decimal Decimal;

            public override string ToString()
            {
                string val = "TestStruct2 {";

                foreach (var field in typeof(TestStruct2).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    val += $"{field.Name}: {field.GetValue(this)}, ";
                }

                val += "}";

                return val;
            }
        }

        [RePacker]
        public class TestClass2
        {
            public bool Bool;
            // public char Char;
            public sbyte Sbyte;
            public byte Byte;
            public short Short;
            public ushort Ushort;
            public int Int;
            public uint Uint;
            public long Long;
            public ulong Ulong;
            public float Float;
            public double Double;
            public decimal Decimal;

            public override string ToString()
            {
                string val = "TestClass2 {";

                foreach (var field in typeof(TestClass2).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    val += $"{field.Name}: {field.GetValue(this)}, ";
                }

                val += "}";

                return val;
            }
        }

        [RePacker]
        public struct StructWithString
        {
            public float Float;
            public string String1;
            public string String2;
            public int Int;
        }

        public enum ByteEnum : byte
        {
            One = 1,
            Ten = 10,
            Hundred = 100,
        }

        public enum LongEnum : long
        {
            Low = -123456789,
            High = 987654321,
        }

        [RePacker]
        public struct StructWithEnum
        {
            public float Float;
            public ByteEnum ByteEnum;
            public LongEnum LongEnum;
            public int Int;
        }

        [RePacker]
        public struct Parent
        {
            public float Float;
            public Child Child;
            public ulong ULong;
        }

        [RePacker]
        public struct Child
        {
            public double Float;
            public byte Byte;
        }

        [RePacker]
        public struct RootType
        {
            public float Float;
            public UnmanagedStruct UnmanagedStruct;
            public double Double;
        }

        public struct UnmanagedStruct
        {
            public int Int;
            public ulong ULong;

            public override string ToString()
            {
                return $"UnmanagedStruct {{ Int: {Int}, ULong: {ULong} }}";
            }
        }

        [RePacker]
        public struct StructWithArray
        {
            public int Int;
            public float[] Floats;
            public long Long;
        }

        [RePacker]
        public struct HasUnmanagedIList
        {
            public float Float;
            public IList<int> Ints;
            public double Double;
        }

        public static void Main(string[] args)
        {
            TypeCache.Init();
            Console.WriteLine("Benchmark");

            // var summary1 = BenchmarkRunner.Run<BufferBench>();
            // var summary2 = BenchmarkRunner.Run<ZeroFormatterBench>();
            // var summary2 = BenchmarkRunner.Run<ILGenerated>();

            /* var personArray = Enumerable.Range(0, 1000).Select(e => new ZeroFormatterBench.Person { Age = e, FirstName = "Windows", LastName = "Server", Sex = ZeroFormatterBench.Sex.Female }).ToArray();
            var container = new ZeroFormatterBench.PersonArrayContainer { Persons = personArray };

            var buffer = new BoxedBuffer(1 << 16);

            RePacker.Pack(buffer, ref container);
            var fromBuf = RePacker.Unpack<ZeroFormatterBench.PersonArrayContainer>(buffer);

            for (int i = 0; i < 1000; i++)
            {
                bool equal =
                    (ZeroFormatterBench.Sex.Female == fromBuf.Persons[i].Sex) &&
                    (i == fromBuf.Persons[i].Age) &&
                    ("Windows" == fromBuf.Persons[i].FirstName) &&
                    ("Server" == fromBuf.Persons[i].LastName);

                if (!equal)
                {
                    Console.WriteLine("Somethings fucked yo");
                    RePacker.Log<ZeroFormatterBench.Person>(ref fromBuf.Persons[i]);
                    break;
                }
            } */


            // object[] param = new object[] { 1.234f, 15, (byte)127 };
            // Program.TestStruct test = (Program.TestStruct)TypeCache.CreateInstance(typeof(Program.TestStruct), param);
            // Console.WriteLine($"{test.Value1} - {test.Value2} - {test.Value3}");


            /* TestClass2 ts2 = new TestClass2
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
            Buffer buffer = new Buffer(new byte[1024], 0);
            BoxedBuffer boxedBuffer = new BoxedBuffer(ref buffer);

            // Serialize
            TypeCache.Serialize<TestClass2>(boxedBuffer, ref ts2);
            Console.WriteLine($"Buffer Size: {boxedBuffer.Buffer.Length()}");

            // Deserialize
            TestClass2 des = TypeCache.Deserialize<TestClass2>(boxedBuffer);
            Console.WriteLine($"{des}");
            Console.WriteLine($"Buffer Size: {boxedBuffer.Buffer.Length()}"); */

            /* StructWithString sws = new StructWithString
            {
                Float = 1.337f,
                String1 = "Hello",
                String2 = "World",
                Int = 1337,
            };

            BoxedBuffer buffer = new BoxedBuffer(1024);

            RePacker.Pack<StructWithString>(buffer, ref sws);
            Console.WriteLine($"Buffer Size: {buffer.Buffer.Length()}");

            var fromBuf = RePacker.Unpack<StructWithString>(buffer);
            Console.WriteLine($"Buffer Size: {buffer.Buffer.Length()}"); */

            /* StructWithEnum sws = new StructWithEnum
            {
                Float = 1.337f,
                ByteEnum = ByteEnum.Ten,
                LongEnum = LongEnum.High,
                Int = 1337,
            };

            BoxedBuffer buffer = new BoxedBuffer(1024);

            RePacker.Pack<StructWithEnum>(buffer, ref sws);
            Console.WriteLine($"Buffer Size: {buffer.Buffer.Length()}");

            var fromBuf = RePacker.Unpack<StructWithEnum>(buffer);
            Console.WriteLine($"Buffer Size: {buffer.Buffer.Length()}");

            RePacker.Log<StructWithEnum>(ref fromBuf); */

            /* var p = new Parent
            {
                Float = 1.337f,
                ULong = 987654321,
                Child = new Child
                {
                    Float = 10f,
                    Byte = 120,
                },
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<Parent>(buffer, ref p);

            var fromBuf = RePacker.Unpack<Parent>(buffer);
            RePacker.Log<Parent>(ref fromBuf); */

            /* var rt = new RootType
            {
                Float = 13.37f,
                Double = 9876.54321,
                UnmanagedStruct = new UnmanagedStruct
                {
                    Int = 1337,
                    ULong = 9876543210,
                },
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<RootType>(buffer, ref rt);
            var fromBuf = RePacker.Unpack<RootType>(buffer);

            RePacker.Log<RootType>(ref fromBuf); */

            /* var swa = new StructWithArray
            {
                Int = 1337,
                Long = -123456789,
                Floats = new float[] {
                    0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f
                }
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<StructWithArray>(buffer, ref swa);
            
            var fromBuf = RePacker.Unpack<StructWithArray>(buffer);
            RePacker.Log<StructWithArray>(ref fromBuf); */

            /* var hml = new HasUnmanagedIList
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new List<int>{1,2,3,4,5,6,7,8,9,0},
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);
            var fromBuf = RePacker.Unpack<HasUnmanagedIList>(buffer);

            RePacker.Log<HasUnmanagedIList>(ref fromBuf); */

            // var generator = GeneratorLookup.Get(GeneratorType.Struct, null);
            // Console.WriteLine(generator);

            var um = new UnmanagedStruct
            {
                Int = 1337,
                ULong = 9876543210,
            };

            var buffer = new BoxedBuffer(1024);
            RePacker.Pack(buffer, ref um);
            var fromBuf = RePacker.Unpack<UnmanagedStruct>(buffer);

            RePacker.Log<UnmanagedStruct>(ref fromBuf);
        }
    }
}

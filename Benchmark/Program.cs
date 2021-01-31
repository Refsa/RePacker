﻿using System;

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
        static byte[] backingBuffer;
        static Buffer buffer;
        Person[] personArray;

        ReadOnlyBuffer personBuffer;
        ReadOnlyBuffer personArrayBuffer;

        Person p = new Person
        {
            Age = 99999,
            FirstName = "Windows",
            LastName = "Server",
            Sex = Sex.Male,
        };

        public ZeroFormatterBench()
        {
            backingBuffer = new byte[1024];
            buffer = new Buffer(backingBuffer, 0);
            personArray = Enumerable.Range(1000, 1000).Select(e => new Person { Age = e, FirstName = "Windows", LastName = "Server", Sex = Sex.Female }).ToArray();

            var _personBuffer = new Buffer(new byte[1024], 0);
            _personBuffer.Encode(p);
            personBuffer = new ReadOnlyBuffer(ref buffer);

            var _personArrayBuffer = new Buffer(new byte[1 << 16], 0);
            _personArrayBuffer.EncodeArray<Person>(personArray);
            personArrayBuffer = new ReadOnlyBuffer(ref _personArrayBuffer);
        }

        public enum Sex : sbyte
        {
            Unknown, Male, Female,
        }

        class Person : ISerializer
        {
            public int Age;
            public string FirstName;
            public string LastName;
            public Sex Sex;

            public void FromBuffer(ref ReadOnlyBuffer buffer)
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

        [Benchmark]
        public void SmallObjectSerialize10K()
        {
            byte[] backBuf = new byte[1024];
            Buffer buffer = new Buffer(backBuf, 0);

            for (int i = 0; i < 10_000; i++)
            {
                buffer.Encode(p);
                buffer.Reset();
            }
        }

        [Benchmark]
        public void SmallObjectDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var p = personBuffer.Decode(typeof(Person));
                personBuffer.Reset();
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
    }

    [MemoryDiagnoser]
    public class ILGenerated
    {
        const int RUNS = 100_000;

        [Benchmark]
        public void TestILGen()
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

            BoxedBuffer boxedBuffer = new BoxedBuffer(1024);

            for (int i = 0; i < RUNS; i++)
            {
                TypeCache.Serialize<Program.TestStruct2>(boxedBuffer, ref ts2);
                var _ = TypeCache.Deserialize<Program.TestStruct2>(boxedBuffer);

                boxedBuffer.Buffer.Reset();
            }
        }

        [Benchmark]
        public void TestNonILGen()
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

            Buffer buffer = new Buffer(new byte[1024], 0);

            for (int i = 0; i < RUNS; i++)
            {
                buffer.Push(ref ts2);
                buffer.Pop<Program.TestStruct2>(out var _);

                buffer.Reset();
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

        public static void Main(string[] args)
        {
            TypeCache.Init();
            Console.WriteLine("Benchmark");

            // var summary1 = BenchmarkRunner.Run<BufferBench>();
            // var summary2 = BenchmarkRunner.Run<ZeroFormatterBench>();
            // var summary2 = BenchmarkRunner.Run<ILGenerated>();

            // object[] param = new object[] { 1.234f, 15, (byte)127 };
            // Program.TestStruct test = (Program.TestStruct)TypeCache.CreateInstance(typeof(Program.TestStruct), param);
            // Console.WriteLine($"{test.Value1} - {test.Value2} - {test.Value3}");

            
            TestClass2 ts2 = new TestClass2
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
            Console.WriteLine($"Buffer Size: {boxedBuffer.Buffer.Length()}");
        }
    }
}

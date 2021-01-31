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
        [Benchmark]
        public void TestILGen()
        {
            object[] param = new object[] { 1.234f, 15, (byte)127 };

            for (int i = 0; i < 1_000_000; i++)
            {
                // Program.TestStruct test = (Program.TestStruct)TypeCache.CreateInstance(typeof(Program.TestStruct), param);
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
            public byte Value;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Benchmark");

            // var summary1 = BenchmarkRunner.Run<BufferBench>();
            // var summary2 = BenchmarkRunner.Run<ZeroFormatterBench>();
            // var summary2 = BenchmarkRunner.Run<ILGenerated>();

            // object[] param = new object[] { 1.234f, 15, (byte)127 };
            // Program.TestStruct test = (Program.TestStruct)TypeCache.CreateInstance(typeof(Program.TestStruct), param);
            // Console.WriteLine($"{test.Value1} - {test.Value2} - {test.Value3}");

            TypeCache.Init();
            // TypeCache.RunTestMethod<TestStruct2>();

            TestStruct2 ts2 = new TestStruct2 { Value = 128 };
            Buffer buffer = new Buffer(new byte[16], 0);
            buffer = TypeCache.Serialize<TestStruct2>(ref buffer, ref ts2);
            Console.WriteLine($"Buffer Size: {buffer.Length()}");

            BoxedBuffer boxedBuffer = new BoxedBuffer(ref buffer);
            TestStruct2 des = TypeCache.Deserialize<TestStruct2>(boxedBuffer);
            Console.WriteLine($"{des.Value}");
            Console.WriteLine($"Buffer Size: {boxedBuffer.Buffer.Length()}");
        }
    }
}

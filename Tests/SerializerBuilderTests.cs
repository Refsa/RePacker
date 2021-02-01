using Xunit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker;
using System.Runtime.InteropServices;
using System;

using Buffer = Refsa.RePacker.Buffers.Buffer;
using ReadOnlyBuffer = Refsa.RePacker.Buffers.ReadOnlyBuffer;

namespace Refsa.RePacker.Tests
{
    [RePacker]
    public struct TestStruct
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
    }

    [RePacker]
    public class TestClass
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
    }

    [RePacker]
    public struct StructWithString
    {
        public float Float;
        public string String1;
        public string String2;
        public int Int;
    }

    [RePacker]
    public class ClassWithString
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
    public struct ClassWithEnum
    {
        public float Float;
        public ByteEnum ByteEnum;
        public LongEnum LongEnum;
        public int Int;
    }

    public class SerializerBuilderTests
    {
        [Fact]
        public void can_handle_struct_with_blittable_fields()
        {
            TestStruct ts2 = new TestStruct
            {
                Bool = true,
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

            BoxedBuffer buffer = new BoxedBuffer(1024);

            RePacker.Pack<TestStruct>(buffer, ref ts2);

            TestStruct fromBuf = RePacker.Unpack<TestStruct>(buffer);

            Assert.Equal(ts2.Bool, fromBuf.Bool);
            Assert.Equal(ts2.Sbyte, fromBuf.Sbyte);
            Assert.Equal(ts2.Byte, fromBuf.Byte);
            Assert.Equal(ts2.Short, fromBuf.Short);
            Assert.Equal(ts2.Ushort, fromBuf.Ushort);
            Assert.Equal(ts2.Int, fromBuf.Int);
            Assert.Equal(ts2.Uint, fromBuf.Uint);
            Assert.Equal(ts2.Long, fromBuf.Long);
            Assert.Equal(ts2.Ulong, fromBuf.Ulong);
            Assert.Equal(ts2.Float, fromBuf.Float);
            Assert.Equal(ts2.Double, fromBuf.Double);
            Assert.Equal(ts2.Decimal, fromBuf.Decimal);

        }

        [Fact]
        public void can_handle_class_with_blittable_fields()
        {
            TestClass ts2 = new TestClass
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

            BoxedBuffer buffer = new BoxedBuffer(1024);

            RePacker.Pack<TestClass>(buffer, ref ts2);

            TestClass fromBuf = RePacker.Unpack<TestClass>(buffer);

            Assert.Equal(ts2.Bool, fromBuf.Bool);
            Assert.Equal(ts2.Sbyte, fromBuf.Sbyte);
            Assert.Equal(ts2.Byte, fromBuf.Byte);
            Assert.Equal(ts2.Short, fromBuf.Short);
            Assert.Equal(ts2.Ushort, fromBuf.Ushort);
            Assert.Equal(ts2.Int, fromBuf.Int);
            Assert.Equal(ts2.Uint, fromBuf.Uint);
            Assert.Equal(ts2.Long, fromBuf.Long);
            Assert.Equal(ts2.Ulong, fromBuf.Ulong);
            Assert.Equal(ts2.Float, fromBuf.Float);
            Assert.Equal(ts2.Double, fromBuf.Double);
            Assert.Equal(ts2.Decimal, fromBuf.Decimal);
        }

        [Fact]
        public void can_handle_struct_with_string()
        {
            StructWithString sws = new StructWithString
            {
                Float = 1.337f,
                String1 = "Hello",
                String2 = "World",
                Int = 1337,
            };

            BoxedBuffer buffer = new BoxedBuffer(1024);

            RePacker.Pack<StructWithString>(buffer, ref sws);

            var fromBuf = RePacker.Unpack<StructWithString>(buffer);

            Assert.Equal(sws.Float, fromBuf.Float);
            Assert.Equal(sws.String1, fromBuf.String1);
            Assert.Equal(sws.String2, fromBuf.String2);
            Assert.Equal(sws.Int, fromBuf.Int);
        }

        [Fact]
        public void can_handle_class_with_string()
        {
            ClassWithString sws = new ClassWithString
            {
                Float = 1.337f,
                String1 = "Hello",
                String2 = "World",
                Int = 1337,
            };

            BoxedBuffer buffer = new BoxedBuffer(1024);

            RePacker.Pack<ClassWithString>(buffer, ref sws);

            var fromBuf = RePacker.Unpack<ClassWithString>(buffer);

            Assert.Equal(sws.Float, fromBuf.Float);
            Assert.Equal(sws.String1, fromBuf.String1);
            Assert.Equal(sws.String2, fromBuf.String2);
            Assert.Equal(sws.Int, fromBuf.Int);
        }

        [Fact]
        public void can_handle_struct_with_enum()
        {
            StructWithEnum sws = new StructWithEnum
            {
                Float = 1.337f,
                ByteEnum = ByteEnum.Ten,
                LongEnum = LongEnum.High,
                Int = 1337,
            };

            BoxedBuffer buffer = new BoxedBuffer(1024);

            RePacker.Pack<StructWithEnum>(buffer, ref sws);
            var fromBuf = RePacker.Unpack<StructWithEnum>(buffer);

            Assert.Equal(sws.Float, fromBuf.Float);
            Assert.Equal(sws.ByteEnum, fromBuf.ByteEnum);
            Assert.Equal(sws.LongEnum, fromBuf.LongEnum);
            Assert.Equal(sws.Int, fromBuf.Int);
        }

        [Fact]
        public void can_handle_class_with_enum()
        {
            ClassWithEnum sws = new ClassWithEnum
            {
                Float = 1.337f,
                ByteEnum = ByteEnum.Ten,
                LongEnum = LongEnum.High,
                Int = 1337,
            };

            BoxedBuffer buffer = new BoxedBuffer(1024);

            RePacker.Pack<ClassWithEnum>(buffer, ref sws);
            var fromBuf = RePacker.Unpack<ClassWithEnum>(buffer);

            Assert.Equal(sws.Float, fromBuf.Float);
            Assert.Equal(sws.ByteEnum, fromBuf.ByteEnum);
            Assert.Equal(sws.LongEnum, fromBuf.LongEnum);
            Assert.Equal(sws.Int, fromBuf.Int);
        }

        public enum Sex : sbyte
        {
            Unknown, Male, Female,
        }
        [RePacker]
        public class Person
        {
            public int Age;
            public string FirstName;
            public string LastName;
            public Sex Sex;
        }

        [Fact]
        public void can_handle_zeroformatter_person_type()
        {
            Person p = new Person
            {
                Age = 99999,
                FirstName = "Windows",
                LastName = "Server",
                Sex = Sex.Male,
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<Person>(buffer, ref p);
            var fromBuf = RePacker.Unpack<Person>(buffer);

            Assert.Equal(p.Age, fromBuf.Age);
            Assert.Equal(p.FirstName, fromBuf.FirstName);
            Assert.Equal(p.LastName, fromBuf.LastName);
            Assert.Equal(p.Sex, fromBuf.Sex);
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
            public float Float;
            public byte Byte;
        }

        [Fact]
        public void can_handle_nested_hierarchies()
        {
            var p = new Parent
            {
                Float = 1.337f,
                ULong = 987654321,
                Child = new Child
                {
                    Float = 10f,
                    Byte = 120,
                }
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<Parent>(buffer, ref p);
            var fromBuf = RePacker.Unpack<Parent>(buffer);

            Assert.Equal(p.Float, fromBuf.Float);
            Assert.Equal(p.ULong, fromBuf.ULong);
            Assert.Equal(p.Child.Float, fromBuf.Child.Float);
            Assert.Equal(p.Child.Byte, fromBuf.Child.Byte);
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
        }

        [Fact]
        public void can_handle_unmanaged_nested_struct()
        {
            var rt = new RootType
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

            Assert.Equal(rt.Float, fromBuf.Float);
            Assert.Equal(rt.Double, fromBuf.Double);
            Assert.Equal(rt.UnmanagedStruct.Int, fromBuf.UnmanagedStruct.Int);
            Assert.Equal(rt.UnmanagedStruct.ULong, fromBuf.UnmanagedStruct.ULong);
        }

        [RePacker]
        public struct HasUnsupportedField
        {
            public int Int;
            public Type Type;
            public float Float;
        }

        [Fact]
        public void unsupported_type_does_not_break_functonality()
        {
            var data = new HasUnsupportedField
            {
                Int = 1234,
                Type = typeof(HasUnsupportedField),
                Float = 4321.1234f,
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref data);
            var fromBuf = RePacker.Unpack<HasUnsupportedField>(buffer);

            Assert.Equal(data.Int, fromBuf.Int);
            Assert.Equal(data.Float, fromBuf.Float);
        }
    }
}
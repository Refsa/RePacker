using Xunit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker;
using System.Runtime.InteropServices;
using System;

using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Tests
{
    public class SerializerTests
    {
        Buffer CreateBuffer(int size = 1024)
        {
            return new Buffer(new Memory<byte>(new byte[size]), 0);
        }

        [Fact]
        public void can_encode_and_decode_string()
        {
            Buffer buffer = CreateBuffer();
            string testString = "This is a test string";

            buffer.PackString(ref testString);

            var readBuffer = new Buffer(ref buffer);
            string fromBuf = readBuffer.UnpackString();

            Assert.Equal(testString, fromBuf);
        }

        public enum TestByteEnum : byte
        {
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
        }

        [Fact]
        public void can_encode_and_decode_byte_enum()
        {
            Buffer buffer = CreateBuffer();
            TestByteEnum testEnum = TestByteEnum.One;

            buffer.PackEnum<TestByteEnum>(ref testEnum);

            Assert.Equal(sizeof(byte), buffer.Count());

            var readBuffer = new Buffer(ref buffer);
            TestByteEnum fromBuf = readBuffer.UnpackEnum<TestByteEnum>();

            Assert.Equal(testEnum, fromBuf);
        }

        public enum TestULongEnum : ulong
        {
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
        }

        [Fact]
        public void can_encode_and_decode_ulong_enum()
        {
            Buffer buffer = CreateBuffer();
            TestULongEnum testEnum = TestULongEnum.One;

            buffer.PackEnum<TestULongEnum>(ref testEnum);

            Assert.Equal(sizeof(ulong), buffer.Count());

            var readBuffer = new Buffer(ref buffer);
            TestULongEnum fromBuf = readBuffer.UnpackEnum<TestULongEnum>();

            Assert.Equal(testEnum, fromBuf);
        }

        [Fact]
        public void can_encode_and_decode_byte_blittable_array()
        {
            Buffer buffer = CreateBuffer();

            byte[] testArray = new byte[64];
            for (int i = 0; i < 64; i++) testArray[i] = (byte)i;

            buffer.EncodeBlittableArray<byte>(testArray);
            Assert.Equal(64 + sizeof(ulong), buffer.Count());

            var readBuffer = new Buffer(ref buffer);
            byte[] fromBuf = readBuffer.DecodeBlittableArray<byte>();

            for (int i = 0; i < 64; i++)
            {
                Assert.Equal(testArray[i], fromBuf[i]);
            }
        }

        [Fact]
        public void can_encode_and_decode_int_blittable_array()
        {
            Buffer buffer = CreateBuffer();

            int[] testArray = new int[32];
            for (int i = 0; i < 32; i++) testArray[i] = (int)i;

            buffer.EncodeBlittableArray<int>(testArray);
            Assert.Equal(32 * sizeof(int) + sizeof(ulong), buffer.Count());

            var readBuffer = new Buffer(ref buffer);
            int[] fromBuf = readBuffer.DecodeBlittableArray<int>();

            for (int i = 0; i < 32; i++)
            {
                Assert.Equal(testArray[i], fromBuf[i]);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
        struct TestBlitStruct
        {
            public int Int;
            public float Float;
        }

        [Fact]
        public void can_encode_and_decode_struct_blittable_array()
        {
            Buffer buffer = CreateBuffer();

            TestBlitStruct[] testData = new TestBlitStruct[32];
            for (int i = 0; i < 32; i++)
            {
                testData[i].Float = 1.337f * (float)i;
                testData[i].Int = i;
            }

            buffer.EncodeBlittableArray<TestBlitStruct>(testData);

            Assert.Equal(32 * Marshal.SizeOf<TestBlitStruct>() + sizeof(ulong), buffer.Count());

            var readBuffer = new Buffer(ref buffer);
            TestBlitStruct[] fromBuf = readBuffer.DecodeBlittableArray<TestBlitStruct>();

            for (int i = 0; i < 32; i++)
            {
                Assert.Equal(testData[i].Float, fromBuf[i].Float);
                Assert.Equal(testData[i].Int, fromBuf[i].Int);
            }
        }

        struct TestManagedStruct : IPacker
        {
            public int Int;
            public string String;

            public void FromBuffer(ref Buffer buffer)
            {
                buffer.Pop<int>(out Int);
                String = buffer.UnpackString();
            }

            public void ToBuffer(ref Buffer buffer)
            {
                buffer.Push<int>(ref Int);
                buffer.PackString(ref String);
            }
        }

        [Fact]
        public void can_encode_decode_iserializer_struct()
        {
            Buffer buffer = CreateBuffer();

            var testData = new TestManagedStruct
            {
                Int = 1337,
                String = "1.234567f",
            };

            buffer.Pack(testData);

            var readBuffer = new Buffer(ref buffer);
            var fromBuf = (TestManagedStruct)PackerExtensions.Unpack(ref readBuffer, typeof(TestManagedStruct));
            Assert.Equal(testData.Int, fromBuf.Int);
            Assert.Equal(testData.String, fromBuf.String);
        }

        [Fact]
        public void can_encode_managed_struct_array()
        {
            Buffer buffer = CreateBuffer();

            TestManagedStruct[] testData = new TestManagedStruct[16];
            for (int i = 0; i < 16; i++)
            {
                testData[i].Int = i;
                testData[i].String = $"Some Message";
            }

            buffer.EncodeArray(testData);
            // Assert.Equal((sizeof(ulong) + 12 + sizeof(int)) * 16, buffer.Count());

            var readBuffer = new Buffer(ref buffer);
            TestManagedStruct[] fromBuf = readBuffer.DecodeArray<TestManagedStruct>();

            for (int i = 0; i < 16; i++)
            {
                Assert.Equal(testData[i].Int, fromBuf[i].Int);
                Assert.Equal(testData[i].String, fromBuf[i].String);
            }
        }

        class TestManagedClass : IPacker
        {
            public int Int;
            public string String;

            public void FromBuffer(ref Buffer buffer)
            {
                buffer.Pop<int>(out Int);
                String = buffer.UnpackString();
            }

            public void ToBuffer(ref Buffer buffer)
            {
                buffer.Push<int>(ref Int);
                buffer.PackString(ref String);
            }
        }

        [Fact]
        public void can_encode_decode_iserializer_class()
        {
            Buffer buffer = CreateBuffer();

            var testData = new TestManagedClass
            {
                Int = 1337,
                String = "1.234567f",
            };

            buffer.Pack(testData);

            var readBuffer = new Buffer(ref buffer);
            var fromBuf = (TestManagedClass)PackerExtensions.Unpack(ref readBuffer, typeof(TestManagedClass));
            Assert.Equal(testData.Int, fromBuf.Int);
            Assert.Equal(testData.String, fromBuf.String);
        }

        [Fact]
        public void can_encode_managed_class_array()
        {
            Buffer buffer = CreateBuffer();

            TestManagedClass[] testData = new TestManagedClass[16];
            for (int i = 0; i < 16; i++)
            {
                testData[i] = new TestManagedClass();
                testData[i].Int = i;
                testData[i].String = $"Some Message";
            }

            buffer.EncodeArray(testData);
            // Assert.Equal((sizeof(ulong) + 12 + sizeof(int)) * 16, buffer.Count());

            var readBuffer = new Buffer(ref buffer);
            TestManagedClass[] fromBuf = readBuffer.DecodeArray<TestManagedClass>();

            for (int i = 0; i < 16; i++)
            {
                Assert.Equal(testData[i].Int, fromBuf[i].Int);
                Assert.Equal(testData[i].String, fromBuf[i].String);
            }
        }

        #region Complex Type
        public enum Sex : sbyte
        {
            Unknown, Male, Female,
        }

        class Person : IPacker
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

        [Fact]
        public void complex_type_person_encode_decode()
        {
            var buf = new byte[1024];
            var buffer = new Buffer(buf, 0);

            Person p = new Person
            {
                Age = 99999,
                FirstName = "Windows",
                LastName = "Server",
                Sex = Sex.Male,
            };

            buffer.Pack(p);

            var readBuffer = new Buffer(ref buffer);
            var fromBuf = readBuffer.Unpack<Person>();

            Assert.Equal(p.Age, fromBuf.Age);
            Assert.Equal(p.FirstName, fromBuf.FirstName);
            Assert.Equal(p.LastName, fromBuf.LastName);
            Assert.Equal(p.Sex, fromBuf.Sex);
        }

        #endregion
    }
}
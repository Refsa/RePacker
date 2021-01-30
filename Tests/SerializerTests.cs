using Xunit;
using Refsa.Repacker.Buffers;
using Refsa.Repacker;
using System.Runtime.InteropServices;
using System;

using Buffer = Refsa.Repacker.Buffers.Buffer;
using ReadOnlyBuffer = Refsa.Repacker.Buffers.ReadOnlyBuffer;

namespace Refsa.Repacker.Tests
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

            buffer.EncodeString(ref testString);

            var readBuffer = new ReadOnlyBuffer(ref buffer);
            string fromBuf = readBuffer.DecodeString();

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

            buffer.EncodeEnum<TestByteEnum>(ref testEnum);

            Assert.Equal(sizeof(byte), buffer.Count());

            var readBuffer = new ReadOnlyBuffer(ref buffer);
            TestByteEnum fromBuf = readBuffer.DecodeEnum<TestByteEnum>();

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

            buffer.EncodeEnum<TestULongEnum>(ref testEnum);

            Assert.Equal(sizeof(ulong), buffer.Count());

            var readBuffer = new ReadOnlyBuffer(ref buffer);
            TestULongEnum fromBuf = readBuffer.DecodeEnum<TestULongEnum>();

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

            var readBuffer = new ReadOnlyBuffer(ref buffer);
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

            var readBuffer = new ReadOnlyBuffer(ref buffer);
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

            var readBuffer = new ReadOnlyBuffer(ref buffer);
            TestBlitStruct[] fromBuf = readBuffer.DecodeBlittableArray<TestBlitStruct>();

            for (int i = 0; i < 32; i++)
            {
                Assert.Equal(testData[i].Float, fromBuf[i].Float);
                Assert.Equal(testData[i].Int, fromBuf[i].Int);
            }
        }

        struct TestManagedStruct : ISerializer
        {
            public int Int;
            public string String;

            public void FromBuffer(ref ReadOnlyBuffer buffer)
            {
                buffer.Pop<int>(out Int);
                String = buffer.DecodeString();
            }

            public void ToBuffer(ref Buffer buffer)
            {
                buffer.Push<int>(ref Int);
                buffer.EncodeString(ref String);
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

            buffer.Encode(testData);

            var readBuffer = new ReadOnlyBuffer(ref buffer);
            var fromBuf = (TestManagedStruct)Serializer.Decode(ref readBuffer, typeof(TestManagedStruct));
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

            var readBuffer = new ReadOnlyBuffer(ref buffer);
            TestManagedStruct[] fromBuf = readBuffer.DecodeArray<TestManagedStruct>();

            for (int i = 0; i < 16; i++)
            {
                Assert.Equal(testData[i].Int, fromBuf[i].Int);
                Assert.Equal(testData[i].String, fromBuf[i].String);
            }
        }

        class TestManagedClass : ISerializer
        {
            public int Int;
            public string String;

            public void FromBuffer(ref ReadOnlyBuffer buffer)
            {
                buffer.Pop<int>(out Int);
                String = buffer.DecodeString();
            }

            public void ToBuffer(ref Buffer buffer)
            {
                buffer.Push<int>(ref Int);
                buffer.EncodeString(ref String);
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

            buffer.Encode(testData);

            var readBuffer = new ReadOnlyBuffer(ref buffer);
            var fromBuf = (TestManagedClass)Serializer.Decode(ref readBuffer, typeof(TestManagedClass));
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

            var readBuffer = new ReadOnlyBuffer(ref buffer);
            TestManagedClass[] fromBuf = readBuffer.DecodeArray<TestManagedClass>();

            for (int i = 0; i < 16; i++)
            {
                Assert.Equal(testData[i].Int, fromBuf[i].Int);
                Assert.Equal(testData[i].String, fromBuf[i].String);
            }
        }
    }
}
using Xunit;
using Refsa.RePacker.Builder;
using System.Runtime.InteropServices;
using System;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Tests
{
    public class SerializerTests
    {
        public SerializerTests()
        {
            RePacker.Init();
        }

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
    }
}
using Xunit;
using RePacker.Buffers;
using System.Runtime.InteropServices;
using System;
using ReBuffer = RePacker.Buffers.ReBuffer;

namespace RePacker.Tests
{
    public class SerializerTests
    {
        ReBuffer CreateBuffer(int size = 1024)
        {
            return new ReBuffer(new byte[size], 0);
        }

        [Fact]
        public void can_encode_and_decode_string()
        {
            var buffer = CreateBuffer();
            string testString = "This is a test string";

            buffer.PackString(ref testString);

            string fromBuf = buffer.UnpackString();

            Assert.Equal(testString, fromBuf);
        }

        [Fact]
        public void can_encode_and_decode_string_multiple()
        {
            var buffer = CreateBuffer(1 << 16);
            string testString = "This is a test string";

            for (int i = 0; i < 10; i++)
            {
                buffer.PackString(ref testString);
            }

            for (int i = 0; i < 10; i++)
            {
                string fromBuf = buffer.UnpackString();

                Assert.Equal(testString, fromBuf);
            }
        }

        [Fact]
        public void string_expands_auto_buffer()
        {
            string value = "aowejoianfaiwenuivaweifvnawiuefv";

            var buffer = new ReBuffer(0, true);
            buffer.PackString(ref value);

            Assert.Equal(buffer.Array.Length, 40);
        }

        [Fact]
        public void reading_string_of_wrong_size_throws()
        {
            var buffer = new ReBuffer(32);
            ulong size = 32;
            buffer.Pack(ref size);

            Assert.Throws<System.IndexOutOfRangeException>(() => buffer.UnpackString());
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
            var buffer = CreateBuffer();
            TestByteEnum testEnum = TestByteEnum.One;

            buffer.PackEnum<TestByteEnum>(ref testEnum);

            Assert.Equal(sizeof(byte), buffer.WriteCursor());

            TestByteEnum fromBuf = buffer.UnpackEnum<TestByteEnum>();

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
            var buffer = CreateBuffer();
            TestULongEnum testEnum = TestULongEnum.One;

            buffer.PackEnum<TestULongEnum>(ref testEnum);

            Assert.Equal(sizeof(ulong), buffer.WriteCursor());

            TestULongEnum fromBuf = buffer.UnpackEnum<TestULongEnum>();

            Assert.Equal(testEnum, fromBuf);
        }

        [Fact]
        public void can_encode_and_decode_byte_blittable_array()
        {
            var buffer = CreateBuffer();

            byte[] testArray = new byte[64];
            for (int i = 0; i < 64; i++) testArray[i] = (byte)i;

            buffer.PackBlittableArray<byte>(testArray);
            Assert.Equal(64 + sizeof(ulong), buffer.WriteCursor());

            byte[] fromBuf = buffer.UnpackBlittableArray<byte>();

            for (int i = 0; i < 64; i++)
            {
                Assert.Equal(testArray[i], fromBuf[i]);
            }
        }

        [Fact]
        public void can_encode_and_decode_int_blittable_array()
        {
            var buffer = CreateBuffer();

            int[] testArray = new int[32];
            for (int i = 0; i < 32; i++) testArray[i] = (int)i;

            buffer.PackBlittableArray<int>(testArray);
            Assert.Equal(32 * sizeof(int) + sizeof(ulong), buffer.WriteCursor());

            int[] fromBuf = buffer.UnpackBlittableArray<int>();

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
            var buffer = CreateBuffer();

            TestBlitStruct[] testData = new TestBlitStruct[32];
            for (int i = 0; i < 32; i++)
            {
                testData[i].Float = 1.337f * (float)i;
                testData[i].Int = i;
            }

            buffer.PackBlittableArray<TestBlitStruct>(testData);

            Assert.Equal(32 * Marshal.SizeOf<TestBlitStruct>() + sizeof(ulong), buffer.WriteCursor());

            TestBlitStruct[] fromBuf = buffer.UnpackBlittableArray<TestBlitStruct>();

            for (int i = 0; i < 32; i++)
            {
                Assert.Equal(testData[i].Float, fromBuf[i].Float);
                Assert.Equal(testData[i].Int, fromBuf[i].Int);
            }
        }
    }
}
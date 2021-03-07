using RePacker.Unsafe;
using Xunit;

namespace RePacker.Unsafe.Tests
{
    public class StringHelperTests
    {
        [Fact]
        public void get_string_ascii_gives_byte_array()
        {
            string data = "abcd";

            byte[] target = new byte[4];
            StringHelper.CopyString(data, target, 0);
            Assert.Equal(new byte[] { 0x61, 0x62, 0x63, 0x64 }, target);
        }

        [Fact]
        public void get_string_unicode_gives_byte_array()
        {
            string obj = "abcdefgəʌʜ";
            byte[] target = new byte[13];
            StringHelper.CopyString(obj, target, 0);

            Assert.Equal(new byte[] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0xC9, 0x99, 0xCA, 0x8C, 0xCA, 0x9C }, target);
        }

        [Fact]
        public void size_of_string_gives_byte_count()
        {
            string obj = "abcdefgəʌʜ";
            Assert.Equal(13, StringHelper.SizeOf(obj));
        }

        [Fact]
        public void size_of_null_string_gives_zero()
        {
            string obj = null;
            Assert.Equal(0, StringHelper.SizeOf(obj));
        }

        [Fact]
        public void get_ascii_string()
        {
            string data = "abcd";

            byte[] target = new byte[] { 0x61, 0x62, 0x63, 0x64 };
            string decoded = StringHelper.GetString(target, 0, 4);
            Assert.Equal(data, decoded);
        }

        [Fact]
        public void get_unicode_string()
        {
            string data = "abcdefgəʌʜ";

            byte[] target = new byte[] { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0xC9, 0x99, 0xCA, 0x8C, 0xCA, 0x9C };
            string decoded = StringHelper.GetString(target, 0, 13);
            Assert.Equal(data, decoded);
        }
    }
}
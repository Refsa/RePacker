using Xunit;
using Xunit.Abstractions;

namespace RePacker.Tests
{
    public class GetSizeTests
    {
        public GetSizeTests(ITestOutputHelper output)
        {
            TestBootstrap.Setup(output);
        }

        [Fact]
        public void get_size_primitive()
        {
            bool var1 = true;
            sbyte var2 = 10;
            byte var3 = 20;
            short var4 = 30;
            ushort var5 = 40;
            int var6 = 50;
            uint var7 = 60;
            long var8 = 70;
            ulong var9 = 80;
            float var10 = 90;
            double var11 = 100;
            decimal var12 = 1000;

            Assert.Equal(sizeof(bool), RePacking.SizeOf(ref var1));
            Assert.Equal(sizeof(sbyte), RePacking.SizeOf(ref var2));
            Assert.Equal(sizeof(byte), RePacking.SizeOf(ref var3));
            Assert.Equal(sizeof(short), RePacking.SizeOf(ref var4));
            Assert.Equal(sizeof(ushort), RePacking.SizeOf(ref var5));
            Assert.Equal(sizeof(int), RePacking.SizeOf(ref var6));
            Assert.Equal(sizeof(uint), RePacking.SizeOf(ref var7));
            Assert.Equal(sizeof(long), RePacking.SizeOf(ref var8));
            Assert.Equal(sizeof(ulong), RePacking.SizeOf(ref var9));
            Assert.Equal(sizeof(float), RePacking.SizeOf(ref var10));
            Assert.Equal(sizeof(double), RePacking.SizeOf(ref var11));
            Assert.Equal(sizeof(decimal), RePacking.SizeOf(ref var12));
        }

        // [Fact]
        // public void get_size_untagged_unmanaged_struct()
        // {
        // var ums = new UnmanagedStruct();
        // 
        // Assert.Equal(16, RePacking.SizeOf(ref ums));
        // }

        [Fact]
        public void get_size_tagged_unmanaged_struct()
        {
            var cs = new ChildStruct();

            Assert.Equal(5, RePacking.SizeOf(ref cs));
        }

        [Fact]
        public void get_size_struct_with_struct()
        {
            var parent = new ParentWithNestedStruct();

            Assert.Equal(17, RePacking.SizeOf(ref parent));
        }

        [Fact]
        public void get_size_has_string()
        {
            var obj = new StructWithString
            {
                String1 = "abcdefg"
            };

            Assert.Equal(31, RePacking.SizeOf(ref obj));
        }

        [Fact]
        public void get_size_has_enum()
        {
            var obj = new StructWithEnum();

            Assert.Equal(17, RePacking.SizeOf(ref obj));
        }
    }
}
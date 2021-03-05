using System.Collections.Generic;
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
        public void has_untagged_unmanaged_struct()
        {
            var rt = new RootType();

            Assert.Equal(24, RePacking.SizeOf(ref rt));
        }

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

        [Fact]
        public void get_size_has_unmanaged_array()
        {
            var hms = new HasStructArray();
            hms.ArrayOfStruct = new ChildStruct[10];

            Assert.Equal(9 + 50 + 8, RePacking.SizeOf(ref hms));
        }

        [Fact]
        public void get_size_has_managed_array()
        {
            var hms = new HasClassArray();
            hms.ArrayOfClass = new InArrayClass[10];
            for (int i = 0; i < 10; i++) hms.ArrayOfClass[i] = new InArrayClass();

            Assert.Equal(9 + 80 + 8, RePacking.SizeOf(ref hms));
        }

        [Fact]
        public void get_size_list()
        {
            var data = new List<int>();
            for (int i = 0; i < 10; i++) data.Add(i);

            Assert.Equal(8 + 40, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_ilist()
        {
            IList<int> data = new List<int>();
            for (int i = 0; i < 10; i++) data.Add(i);

            Assert.Equal(8 + 40, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_stack()
        {
            var data = new Stack<int>();
            for (int i = 0; i < 10; i++) data.Push(i);

            Assert.Equal(8 + 40, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_queue()
        {
            var data = new Queue<int>();
            for (int i = 0; i < 10; i++) data.Enqueue(i);

            Assert.Equal(8 + 40, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_hashset()
        {
            var data = new HashSet<int>();
            for (int i = 0; i < 10; i++) data.Add(i);

            Assert.Equal(8 + 40, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_dictionary()
        {
            var data = new Dictionary<int, float>();
            for (int i = 0; i < 10; i++) data.Add(i, (float)i);

            Assert.Equal(8 + 40 * 2, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_key_value_pair()
        {
            var data = new KeyValuePair<int, int>(10, 20);

            Assert.Equal(8, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_nullable()
        {
            int? data = 10;
            Assert.Equal(5, RePacking.SizeOf(ref data));

            data = null;
            Assert.Equal(1, RePacking.SizeOf(ref data));
        }
        
        [Fact]
        public void get_size_time_span()
        {
            var data = new System.TimeSpan(128);

            Assert.Equal(8, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_date_time()
        {
            var data = System.DateTime.Now;

            Assert.Equal(8, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_value_tuple_2()
        {
            var data = (10, 10);

            Assert.Equal(8, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_value_tuple_3()
        {
            var data = (10, 10, 10);

            Assert.Equal(12, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_value_tuple_4()
        {
            var data = (10, 10, 10, 10);

            Assert.Equal(16, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_value_tuple_5()
        {
            var data = (10, 10, 10, 10, 10);

            Assert.Equal(20, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_value_tuple_6()
        {
            var data = (10, 10, 10, 10, 10, 10);

            Assert.Equal(24, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_value_tuple_7()
        {
            var data = (10, 10, 10, 10, 10, 10, 10);

            Assert.Equal(28, RePacking.SizeOf(ref data));
        }

        /* [Fact]
        public void get_size_value_tuple_8()
        {
            var data = (10, 10, 10, 10, 10, 10, 10, 10ul);

            Assert.Equal(28 + 16, RePacking.SizeOf(ref data));
        } */
    }
}
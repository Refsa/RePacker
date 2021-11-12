using System.Collections.Generic;
using RePacker.Buffers;
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

        [Fact]
        public void get_size_struct_with_blittable_fields()
        {
            TestStruct ts2 = new TestStruct();

            Assert.Equal(59, RePacking.SizeOf(ref ts2));
        }

        [Fact]
        public void get_size_class_with_blittable_fields()
        {
            TestClass ts2 = new TestClass();

            Assert.Equal(59, RePacking.SizeOf(ref ts2));
        }

        [Fact]
        public void get_size_untagged_unmanaged_struct()
        {
            var ums = new UnmanagedStruct();

            Assert.Equal(12, RePacking.SizeOf(ref ums));
        }

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
        public void get_size_unmarked_properties()
        {
            var obj = new StructWithUnmarkedProperties();

            Assert.Equal(0, RePacking.SizeOf(ref obj));
        }

        [Fact]
        public void get_size_marked_properties()
        {
            var obj = new StructWithMarkedProperties();

            Assert.Equal(8, RePacking.SizeOf(ref obj));
        }

        [Fact]
        public void get_size_only_marked()
        {
            var obj = new OnlyPackSelectedFields();

            Assert.Equal(12, RePacking.SizeOf(ref obj));
        }

        [Fact]
        public void get_size_string()
        {
            string obj = "abcdefg";
            Assert.Equal(8 + 7, RePacking.SizeOf(ref obj));
        }

        [Fact]
        public void get_size_string_with_utf8_chars()
        {
            string obj = "abcdefgəʌʜ";
            Assert.Equal(8 + 13, RePacking.SizeOf(ref obj));
        }

        [Fact]
        public void get_size_array_of_strings()
        {
            string[] obj = new string[] {
                "abcdefg","abcdefg","abcdefg","abcdefg","abcdefg",
                "abcdefg","abcdefg","abcdefg","abcdefg","abcdefg",
                "abc","abc","abc","abc","abc",
                "abcdefg","abcdefg","abcdefg","abcdefg","abcdefg",
                "abcdefg","abcdefg","abcdefg","abcdefg","abcdefg",
            };
            Assert.Equal(8 + 8 * 25 + 7 * 20 + 3 * 5, RePacking.SizeOf(ref obj));
        }

#if LOCAL_TEST
        [Fact]
        public void get_size_large_string()
        {
            string largeString = System.IO.File.ReadAllText("CSharpHtml.txt");

            int size = RePacking.SizeOf(ref largeString);
            Assert.Equal(309081 + 8, size);
        }
#endif

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
        public void get_size_class_has_string()
        {
            var obj = new ClassWithString
            {
                String1 = "abcdefg"
            };

            Assert.Equal(31, RePacking.SizeOf(ref obj));
        }

        [Fact]
        public void get_size_enum()
        {
            var byteEnum = ByteEnum.Ten;
            var longEnum = LongEnum.High;

            Assert.Equal(1, RePacking.SizeOf(ref byteEnum));
            Assert.Equal(8, RePacking.SizeOf(ref longEnum));
        }

        [Fact]
        public void get_size_struct_has_enum()
        {
            var obj = new StructWithEnum();

            Assert.Equal(17, RePacking.SizeOf(ref obj));
        }

        [Fact]
        public void get_size_class_has_enum()
        {
            var obj = new ClassWithEnum();

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
        public void get_size_buffer()
        {
            var testBuffer = new ReBuffer(1024);
            for (int i = 0; i < 10; i++)
            {
                testBuffer.Pack(ref i);
            }

            Assert.Equal(8 + 8 + 4 * 10, RePacking.SizeOf(ref testBuffer));
        }

        [Fact]
        public void get_size_struct_has_buffer()
        {
            var testBuffer = new StructHasBuffer { Buffer = new ReBuffer(1024) };
            for (int i = 0; i < 10; i++)
            {
                testBuffer.Buffer.Pack(ref i);
            }

            Assert.Equal(8 + 8 + 4 * 10, RePacking.SizeOf(ref testBuffer));
        }

        [Fact]
        public void get_size_class_has_buffer()
        {
            var testBuffer = new ClassHasBuffer { Buffer = new ReBuffer(1024) };
            for (int i = 0; i < 10; i++)
            {
                testBuffer.Buffer.Pack(ref i);
            }

            Assert.Equal(8 + 8 + 4 * 10, RePacking.SizeOf(ref testBuffer));
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
        public void get_size_has_nullable()
        {
            var obj = new HasNullable();
            Assert.Equal(3, RePacking.SizeOf(ref obj));

            obj = new HasNullable
            {
                Float = 10f,
                Int = 10,
                Bool = true,
            };

            Assert.Equal(12, RePacking.SizeOf(ref obj));
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
        public void get_size_value_tuple_1()
        {
            var data = new System.ValueTuple<int>(10);

            Assert.Equal(4, RePacking.SizeOf(ref data));
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

        [Fact]
        public void get_size_value_tuple_8()
        {
            var data = (10, 10, 10, 10, 10, 10, 10, 10ul);

            Assert.Equal(28 + 8, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_has_value_tuple_1()
        {
            var data = new HasValueTuple1 { VT = new System.ValueTuple<int>(10) };

            Assert.Equal(4, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_has_value_tuple_2()
        {
            var data = new HasValueTuple2 { VT = (10, 10) };

            Assert.Equal(8, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_has_value_tuple_3()
        {
            var data = new HasValueTuple3 { VT = (10, 10, 10) };

            Assert.Equal(12, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_has_value_tuple_4()
        {
            var data = new HasValueTuple4 { VT = (10, 10, 10, 10) };

            Assert.Equal(16, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_has_value_tuple_5()
        {
            var data = new HasValueTuple5 { VT = (10, 10, 10, 10, 10) };

            Assert.Equal(20, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_has_value_tuple_6()
        {
            var data = new HasValueTuple6 { VT = (10, 10, 10, 10, 10, 10) };

            Assert.Equal(24, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_has_value_tuple_7()
        {
            var data = new HasValueTuple7 { VT = (10, 10, 10, 10, 10, 10, 10) };

            Assert.Equal(28, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_has_value_tuple_8()
        {
            var data = new HasValueTuple8 { VT = (10, 10, 10, 10, 10, 10, 10, 10) };

            Assert.Equal(28 + 4, RePacking.SizeOf(ref data));
        }

        [Fact]
        public void get_size_from_interface_cast()
        {
            var data = new ClassHasInterface(1234, 1234);
            var asInterface = (ITestInterface)data;
            Assert.Equal(16, RePacking.SizeOf(ref asInterface));
        }
    }
}
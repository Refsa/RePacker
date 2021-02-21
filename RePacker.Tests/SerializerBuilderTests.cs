using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using RePacker.Buffers;
using RePacker.Builder;
using Xunit.Abstractions;

namespace RePacker.Tests
{
    public class SerializerBuilderTests
    {
        BoxedBuffer buffer = new BoxedBuffer(1 << 24);

        public SerializerBuilderTests(ITestOutputHelper output)
        {
            TestBootstrap.Setup(output);
        }

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

            buffer.Reset();

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

            buffer.Reset();

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

            buffer.Reset();

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

            buffer.Reset();

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

            buffer.Reset();

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

            buffer.Reset();

            RePacker.Pack<ClassWithEnum>(buffer, ref sws);
            var fromBuf = RePacker.Unpack<ClassWithEnum>(buffer);

            Assert.Equal(sws.Float, fromBuf.Float);
            Assert.Equal(sws.ByteEnum, fromBuf.ByteEnum);
            Assert.Equal(sws.LongEnum, fromBuf.LongEnum);
            Assert.Equal(sws.Int, fromBuf.Int);
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

            buffer.Reset();

            RePacker.Pack<Person>(buffer, ref p);
            var fromBuf = RePacker.Unpack<Person>(buffer);

            Assert.Equal(p.Age, fromBuf.Age);
            Assert.Equal(p.FirstName, fromBuf.FirstName);
            Assert.Equal(p.LastName, fromBuf.LastName);
            Assert.Equal(p.Sex, fromBuf.Sex);
        }


        [Fact]
        public void can_handle_nested_struct_hierarchies()
        {
            var p = new ParentWithNestedStruct
            {
                Float = 1.337f,
                ULong = 987654321,
                Child = new ChildStruct
                {
                    Float = 10f,
                    Byte = 120,
                }
            };

            buffer.Reset();

            RePacker.Pack<ParentWithNestedStruct>(buffer, ref p);
            var fromBuf = RePacker.Unpack<ParentWithNestedStruct>(buffer);

            Assert.Equal(p.Float, fromBuf.Float);
            Assert.Equal(p.ULong, fromBuf.ULong);
            Assert.Equal(p.Child.Float, fromBuf.Child.Float);
            Assert.Equal(p.Child.Byte, fromBuf.Child.Byte);
        }

        [Fact]
        public void can_handle_nested_class_hierarchies()
        {
            var p = new ParentWithNestedClass
            {
                Float = 1.337f,
                ULong = 987654321,
                Child = new ChildClass
                {
                    Float = 10f,
                    Byte = 120,
                }
            };

            buffer.Reset();

            RePacker.Pack<ParentWithNestedClass>(buffer, ref p);
            var fromBuf = RePacker.Unpack<ParentWithNestedClass>(buffer);

            Assert.Equal(p.Float, fromBuf.Float);
            Assert.Equal(p.ULong, fromBuf.ULong);
            Assert.Equal(p.Child.Float, fromBuf.Child.Float);
            Assert.Equal(p.Child.Byte, fromBuf.Child.Byte);
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

            buffer.Reset();

            RePacker.Pack<RootType>(buffer, ref rt);
            var fromBuf = RePacker.Unpack<RootType>(buffer);

            Assert.Equal(rt.Float, fromBuf.Float);
            Assert.Equal(rt.Double, fromBuf.Double);
            Assert.Equal(rt.UnmanagedStruct.Int, fromBuf.UnmanagedStruct.Int);
            Assert.Equal(rt.UnmanagedStruct.ULong, fromBuf.UnmanagedStruct.ULong);
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

            buffer.Reset();

            RePacker.Pack(buffer, ref data);
            var fromBuf = RePacker.Unpack<HasUnsupportedField>(buffer);

            Assert.Equal(data.Int, fromBuf.Int);
            Assert.Equal(data.Float, fromBuf.Float);
        }


        [Fact]
        public void primitives_standalone()
        {
            buffer.Reset();

            int intValue = 10;
            RePacker.Pack<int>(buffer, ref intValue);
            Assert.Equal(intValue, RePacker.Unpack<int>(buffer));

            uint uintValue = 10;
            RePacker.Pack<uint>(buffer, ref uintValue);
            Assert.Equal(uintValue, RePacker.Unpack<uint>(buffer));

            byte byteValue = 10;
            RePacker.Pack<byte>(buffer, ref byteValue);
            Assert.Equal(byteValue, RePacker.Unpack<byte>(buffer));

            sbyte sbyteValue = 10;
            RePacker.Pack<sbyte>(buffer, ref sbyteValue);
            Assert.Equal(sbyteValue, RePacker.Unpack<sbyte>(buffer));

            long longValue = 10;
            RePacker.Pack<long>(buffer, ref longValue);
            Assert.Equal(longValue, RePacker.Unpack<long>(buffer));

            ulong ulongValue = 10;
            RePacker.Pack<ulong>(buffer, ref ulongValue);
            Assert.Equal(ulongValue, RePacker.Unpack<ulong>(buffer));

            float floatValue = 10.1234f;
            RePacker.Pack<float>(buffer, ref floatValue);
            Assert.Equal(floatValue, RePacker.Unpack<float>(buffer));

            double doubleValue = 10.1234;
            RePacker.Pack<double>(buffer, ref doubleValue);
            Assert.Equal(doubleValue, RePacker.Unpack<double>(buffer));

            decimal decimalValue = 10.1234M;
            RePacker.Pack<decimal>(buffer, ref decimalValue);
            Assert.Equal(decimalValue, RePacker.Unpack<decimal>(buffer));
        }

        #region DateTime
        [Fact]
        public void date_time_buffer_packing()
        {
            DateTime dt = DateTime.Now;

            buffer.Reset();

            buffer.PackDateTime(ref dt);
            buffer.UnpackDateTime(out DateTime fromBuf);

            Assert.Equal(dt.Ticks, fromBuf.Ticks);
        }

        [Fact]
        public void has_date_time_packing()
        {
            var hdt = new HasDateTime { Float = 1.2344534f, DateTime = DateTime.Now };

            buffer.Reset();

            RePacker.Pack(buffer, ref hdt);
            var fromBuf = RePacker.Unpack<HasDateTime>(buffer);

            Assert.Equal(hdt.Float, fromBuf.Float);
            Assert.Equal(hdt.DateTime, fromBuf.DateTime);
        }

        [Fact]
        public void standalone_date_time_repacker()
        {
            DateTime dt = DateTime.Now;

            buffer.Reset();

            RePacker.Pack<DateTime>(buffer, ref dt);
            DateTime fromBuf = RePacker.Unpack<DateTime>(buffer);

            Assert.Equal(dt.Ticks, fromBuf.Ticks);
        }
        #endregion

        #region String
        [Fact]
        public void has_regular_string()
        {
            var hs = new HasString { Float = 1212345.123451f, String = "This is some message" };

            buffer.Reset();

            RePacker.Pack(buffer, ref hs);
            var fromBuf = RePacker.Unpack<HasString>(buffer);

            Assert.Equal(hs.Float, fromBuf.Float);
            Assert.Equal(hs.String, fromBuf.String);
        }

        [Fact]
        public void standalone_string_repacker()
        {
            string value = "abrakadabra this is a magic trick";

            buffer.Reset();

            RePacker.Pack(buffer, ref value);
            string fromBuf = RePacker.Unpack<string>(buffer);

            Assert.Equal(value, fromBuf);
        }

        [Fact]
        public void standalone_large_string_repacker()
        {
            var largeString = System.IO.File.ReadAllText("CSharpHtml.txt");
            byte[] asBytes = System.Text.Encoding.UTF8.GetBytes(largeString);

            buffer.Reset();

            RePacker.Pack(buffer, ref largeString);
            Assert.Equal(asBytes.Length, buffer.Buffer.WriteCursor() - sizeof(ulong));

            string fromBuf = RePacker.Unpack<string>(buffer);

            Assert.Equal(largeString.Length, fromBuf.Length);
            Assert.Equal(largeString, fromBuf);
        }

        [Fact]
        public void null_string()
        {
            string data = null;
            buffer.Reset();

            buffer.Pack(ref data);
            string fromBuf = buffer.Unpack<string>();

            Assert.NotNull(fromBuf);
            Assert.Equal("", fromBuf);
        }
        #endregion

        [Fact]
        public void standalone_key_value_pair_unmanaged()
        {
            var kvp = new KeyValuePair<int, int>(10, 100);

            buffer.Reset();

            RePacker.Pack(buffer, ref kvp);

            var fromBuf = RePacker.Unpack<KeyValuePair<int, int>>(buffer);

            Assert.Equal(kvp.Key, fromBuf.Key);
            Assert.Equal(kvp.Value, fromBuf.Value);
        }

        [Fact]
        public void standalone_key_value_pair_managed_key()
        {
            var kvp = new KeyValuePair<SimpleClass, int>(new SimpleClass { Float = 12.34f }, 100);

            buffer.Reset();

            RePacker.Pack(buffer, ref kvp);

            var fromBuf = RePacker.Unpack<KeyValuePair<SimpleClass, int>>(buffer);

            Assert.Equal(kvp.Key.Float, fromBuf.Key.Float);
            Assert.Equal(kvp.Value, fromBuf.Value);
        }

        [Fact]
        public void standalone_key_value_pair_managed_value()
        {
            var kvp = new KeyValuePair<int, SimpleClass>(100, new SimpleClass { Float = 12.34f });

            buffer.Reset();

            RePacker.Pack(buffer, ref kvp);

            var fromBuf = RePacker.Unpack<KeyValuePair<int, SimpleClass>>(buffer);

            Assert.Equal(kvp.Key, fromBuf.Key);
            Assert.Equal(kvp.Value.Float, fromBuf.Value.Float);
        }

        [Fact]
        public void struct_with_key_value_pair()
        {
            var wanted = new StructWithKeyValuePair
            {
                Float = 1.234f,
                KeyValuePair = new KeyValuePair<int, int>(10, 100)
            };

            buffer.Reset();

            RePacker.Pack(buffer, ref wanted);

            var fromBuf = RePacker.Unpack<StructWithKeyValuePair>(buffer);

            Assert.Equal(wanted.Float, fromBuf.Float);
            Assert.Equal(wanted.KeyValuePair.Key, fromBuf.KeyValuePair.Key);
            Assert.Equal(wanted.KeyValuePair.Value, fromBuf.KeyValuePair.Value);
        }

        [Fact]
        public void repack_only_selected_fields()
        {
            var opsf = new OnlyPackSelectedFields
            {
                PackFloat = 1.234f,
                DontPackInt = 1337,
                PackLong = 1234567890123456789,
            };

            buffer.Reset();

            RePacker.Pack(buffer, ref opsf);

            var fromBuf = RePacker.Unpack<OnlyPackSelectedFields>(buffer);

            Assert.Equal(opsf.PackFloat, fromBuf.PackFloat);
            Assert.Equal(opsf.PackLong, fromBuf.PackLong);

            Assert.Equal(0, fromBuf.DontPackInt);
        }

        [Fact]
        public void dont_serialize_unmarked_properties()
        {
            var swp = new StructWithUnmarkedProperties
            {
                Float = 1.245f,
                Int = 1337,
            };

            buffer.Reset();

            RePacker.Pack(buffer, ref swp);

            var fromBuf = RePacker.Unpack<StructWithUnmarkedProperties>(buffer);

            Assert.Equal(0f, fromBuf.Float);
            Assert.Equal(0, fromBuf.Int);
        }

        [Fact]
        public void serialize_marked_properties()
        {
            var swp = new StructWithMarkedProperties
            {
                Float = 1.245f,
                Int = 1337,
                Long = 123456789
            };

            buffer.Reset();

            RePacker.Pack(buffer, ref swp);

            var fromBuf = RePacker.Unpack<StructWithMarkedProperties>(buffer);

            Assert.Equal(swp.Float, fromBuf.Float);
            Assert.Equal(swp.Int, fromBuf.Int);

            Assert.Equal(0, fromBuf.Long);
        }

        [Fact]
        public void serialize_internal_type()
        {
            var t = new IHavePrivateType.IAmPrivate
            {
                Float = 1.24f,
                Int = 1337,
            };

            buffer.Reset();

            RePacker.Pack(buffer, ref t);

            var fromBuf = RePacker.Unpack<IHavePrivateType.IAmPrivate>(buffer);

            Assert.Equal(t.Float, fromBuf.Float);
            Assert.Equal(t.Int, fromBuf.Int);
        }

        [Fact]
        public void struct_with_nullable()
        {
            buffer.Reset();

            var hn = new HasNullable
            {
                Float = 1.234f,
                Int = null,
                Bool = true,
            };

            buffer.Pack(ref hn);

            var fromBuf = buffer.Unpack<HasNullable>();

            Assert.Equal(hn.Float, fromBuf.Float);
            Assert.Equal(hn.Int, fromBuf.Int);
            Assert.Equal(hn.Bool, fromBuf.Bool);
        }

        [Fact]
        public void nullable_direct()
        {
            buffer.Reset();

            float? val = 10f;
            float? val2 = null;

            buffer.Pack(ref val);
            buffer.Pack(ref val2);

            var fromBuf = buffer.Unpack<float?>();
            Assert.Equal(val, fromBuf);

            var fromBuf2 = buffer.Unpack<float?>();
            Assert.Equal(val2, fromBuf2);
        }

#if NETCOREAPP30 || NETCOREAPP31 || NET473
        [Fact]
        public void nullable_list_direct()
        {
            buffer.Reset();

            List<int>? test = new List<int>();
        }
#endif
    }
}
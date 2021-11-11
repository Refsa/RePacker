using Xunit;
using System.Collections.Generic;
using System.Linq;
using RePacker.Buffers;
using RePacker.Builder;
using Xunit.Abstractions;

namespace RePacker.Tests
{
    public class SerializerBuilderTests
    {
        ReBuffer buffer = new ReBuffer(1 << 24);

        public SerializerBuilderTests(ITestOutputHelper output)
        {
            TestBootstrap.Setup(output);
        }

        [Fact]
        public void unmarked_and_unmanaged_struct()
        {
            var data = new UnmanagedStruct
            {
                Int = 12345,
                ULong = 4523426435,
            };

            buffer.Reset();

            buffer.Pack(ref data);
            var fromBuf = RePacking.Unpack<UnmanagedStruct>(buffer);

            Assert.Equal(data.Int, fromBuf.Int);
            Assert.Equal(data.ULong, fromBuf.ULong);
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

            RePacking.Pack<TestStruct>(buffer, ref ts2);

            TestStruct fromBuf = RePacking.Unpack<TestStruct>(buffer);

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

            RePacking.Pack<TestClass>(buffer, ref ts2);

            TestClass fromBuf = RePacking.Unpack<TestClass>(buffer);

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

            RePacking.Pack<StructWithString>(buffer, ref sws);

            var fromBuf = RePacking.Unpack<StructWithString>(buffer);

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

            RePacking.Pack<ClassWithString>(buffer, ref sws);

            var fromBuf = RePacking.Unpack<ClassWithString>(buffer);

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

            RePacking.Pack<StructWithEnum>(buffer, ref sws);
            var fromBuf = RePacking.Unpack<StructWithEnum>(buffer);

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

            RePacking.Pack<ClassWithEnum>(buffer, ref sws);
            var fromBuf = RePacking.Unpack<ClassWithEnum>(buffer);

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

            RePacking.Pack<Person>(buffer, ref p);
            var fromBuf = RePacking.Unpack<Person>(buffer);

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

            RePacking.Pack<ParentWithNestedStruct>(buffer, ref p);
            var fromBuf = RePacking.Unpack<ParentWithNestedStruct>(buffer);

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

            RePacking.Pack<ParentWithNestedClass>(buffer, ref p);
            var fromBuf = RePacking.Unpack<ParentWithNestedClass>(buffer);

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

            RePacking.Pack<RootType>(buffer, ref rt);
            var fromBuf = RePacking.Unpack<RootType>(buffer);

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

            RePacking.Pack(buffer, ref data);
            var fromBuf = RePacking.Unpack<HasUnsupportedField>(buffer);

            Assert.Equal(data.Int, fromBuf.Int);
            Assert.Equal(data.Float, fromBuf.Float);
        }


        [Fact]
        public void primitives_standalone()
        {
            buffer.Reset();

            int intValue = 10;
            RePacking.Pack<int>(buffer, ref intValue);
            Assert.Equal(intValue, RePacking.Unpack<int>(buffer));

            uint uintValue = 10;
            RePacking.Pack<uint>(buffer, ref uintValue);
            Assert.Equal(uintValue, RePacking.Unpack<uint>(buffer));

            byte byteValue = 10;
            RePacking.Pack<byte>(buffer, ref byteValue);
            Assert.Equal(byteValue, RePacking.Unpack<byte>(buffer));

            sbyte sbyteValue = 10;
            RePacking.Pack<sbyte>(buffer, ref sbyteValue);
            Assert.Equal(sbyteValue, RePacking.Unpack<sbyte>(buffer));

            long longValue = 10;
            RePacking.Pack<long>(buffer, ref longValue);
            Assert.Equal(longValue, RePacking.Unpack<long>(buffer));

            ulong ulongValue = 10;
            RePacking.Pack<ulong>(buffer, ref ulongValue);
            Assert.Equal(ulongValue, RePacking.Unpack<ulong>(buffer));

            float floatValue = 10.1234f;
            RePacking.Pack<float>(buffer, ref floatValue);
            Assert.Equal(floatValue, RePacking.Unpack<float>(buffer));

            double doubleValue = 10.1234;
            RePacking.Pack<double>(buffer, ref doubleValue);
            Assert.Equal(doubleValue, RePacking.Unpack<double>(buffer));

            decimal decimalValue = 10.1234M;
            RePacking.Pack<decimal>(buffer, ref decimalValue);
            Assert.Equal(decimalValue, RePacking.Unpack<decimal>(buffer));
        }

        #region DateTime
        [Fact]
        public void date_time_buffer_packing()
        {
            System.DateTime dt = System.DateTime.Now;

            buffer.Reset();

            buffer.PackDateTime(ref dt);
            buffer.UnpackDateTime(out System.DateTime fromBuf);

            Assert.Equal(dt, fromBuf);
        }

        [Fact]
        public void has_date_time_packing()
        {
            var hdt = new HasDateTime { Float = 1.2344534f, DateTime = System.DateTime.Now };

            buffer.Reset();

            RePacking.Pack(buffer, ref hdt);
            var fromBuf = RePacking.Unpack<HasDateTime>(buffer);

            Assert.Equal(hdt.Float, fromBuf.Float);
            Assert.Equal(hdt.DateTime, fromBuf.DateTime);
        }

        [Fact]
        public void standalone_date_time_repacker()
        {
            System.DateTime dt = System.DateTime.Now;

            buffer.Reset();

            RePacking.Pack<System.DateTime>(buffer, ref dt);
            System.DateTime fromBuf = RePacking.Unpack<System.DateTime>(buffer);

            Assert.Equal(dt, fromBuf);
        }

        [Fact]
        public void time_span_buffer_packing()
        {
            buffer.Reset();

            System.TimeSpan ts = new System.TimeSpan(512);

            buffer.PackTimeSpan(ref ts);
            buffer.UnpackTimeSpan(out var fromBuf);

            Assert.Equal(ts, fromBuf);
        }

        [Fact]
        public void has_time_span()
        {
            buffer.Reset();

            var hts = new HasTimespan
            {
                Float = 1.234f,
                TimeSpan = new System.TimeSpan(512)
            };

            RePacking.Pack(buffer, ref hts);
            var fromBuf = RePacking.Unpack<HasTimespan>(buffer);

            Assert.Equal(hts, fromBuf);
        }

        [Fact]
        public void standalone_time_span_repacker()
        {
            buffer.Reset();

            System.TimeSpan ts = new System.TimeSpan(512);

            RePacking.Pack(buffer, ref ts);
            var fromBuf = RePacking.Unpack<System.TimeSpan>(buffer);

            Assert.Equal(ts, fromBuf);
        }
        #endregion

        #region String
        [Fact]
        public void has_regular_string()
        {
            var hs = new HasString { Float = 1212345.123451f, String = "This is some message" };

            buffer.Reset();

            RePacking.Pack(buffer, ref hs);
            var fromBuf = RePacking.Unpack<HasString>(buffer);

            Assert.Equal(hs.Float, fromBuf.Float);
            Assert.Equal(hs.String, fromBuf.String);
        }

        [Fact]
        public void standalone_string_repacker()
        {
            string value = "abrakadabra this is a magic trick";

            buffer.Reset();

            RePacking.Pack(buffer, ref value);
            string fromBuf = RePacking.Unpack<string>(buffer);

            Assert.Equal(value, fromBuf);
        }

        [Fact]
        public void standalone_large_string_repacker()
        {
            var largeString = System.IO.File.ReadAllText("CSharpHtml.txt");
            byte[] asBytes = System.Text.Encoding.UTF8.GetBytes(largeString);

            buffer.Reset();

            RePacking.Pack(buffer, ref largeString);
            Assert.Equal(asBytes.Length, buffer.WriteCursor() - sizeof(ulong));

            string fromBuf = RePacking.Unpack<string>(buffer);

            Assert.Equal(largeString.Length, fromBuf.Length);
            Assert.Equal(largeString, fromBuf);
        }

        [Fact]
        public void null_string()
        {
            string data = null;
            buffer.Reset();

            RePacking.Pack(buffer, ref data);
            string fromBuf = RePacking.Unpack<string>(buffer);

            Assert.NotNull(fromBuf);
            Assert.Equal("", fromBuf);
        }
        #endregion

        [Fact]
        public void standalone_key_value_pair_unmanaged()
        {
            var kvp = new KeyValuePair<int, int>(10, 100);

            buffer.Reset();

            RePacking.Pack(buffer, ref kvp);

            var fromBuf = RePacking.Unpack<KeyValuePair<int, int>>(buffer);

            Assert.Equal(kvp.Key, fromBuf.Key);
            Assert.Equal(kvp.Value, fromBuf.Value);
        }

        [Fact]
        public void standalone_key_value_pair_managed_key()
        {
            var kvp = new KeyValuePair<SimpleClass, int>(new SimpleClass { Float = 12.34f }, 100);

            buffer.Reset();

            RePacking.Pack(buffer, ref kvp);

            var fromBuf = RePacking.Unpack<KeyValuePair<SimpleClass, int>>(buffer);

            Assert.Equal(kvp.Key.Float, fromBuf.Key.Float);
            Assert.Equal(kvp.Value, fromBuf.Value);
        }

        [Fact]
        public void standalone_key_value_pair_managed_value()
        {
            var kvp = new KeyValuePair<int, SimpleClass>(100, new SimpleClass { Float = 12.34f });

            buffer.Reset();

            RePacking.Pack(buffer, ref kvp);

            var fromBuf = RePacking.Unpack<KeyValuePair<int, SimpleClass>>(buffer);

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

            RePacking.Pack(buffer, ref wanted);

            var fromBuf = RePacking.Unpack<StructWithKeyValuePair>(buffer);

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

            RePacking.Pack(buffer, ref opsf);

            var fromBuf = RePacking.Unpack<OnlyPackSelectedFields>(buffer);

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

            RePacking.Pack(buffer, ref swp);

            var fromBuf = RePacking.Unpack<StructWithUnmarkedProperties>(buffer);

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

            RePacking.Pack(buffer, ref swp);

            var fromBuf = RePacking.Unpack<StructWithMarkedProperties>(buffer);

            Assert.Equal(swp.Float, fromBuf.Float);
            Assert.Equal(swp.Int, fromBuf.Int);

            Assert.Equal(0, fromBuf.Long);
        }

        [Fact]
        public void serialize_marked_private_fields()
        {
            var swmpf = new StructWithMarkedPrivateField(1.234f, 43452435, 10);

            buffer.Reset();

            RePacking.Pack(buffer, ref swmpf);
            var fromBuf = RePacking.Unpack<StructWithMarkedPrivateField>(buffer);

            Assert.Equal(swmpf, fromBuf);
        }

        [Fact]
        public void dont_serialize_unmarked_private_fields()
        {
            var swmpf = new StructWithPrivateField(1.234f, 43452435, 10);

            buffer.Reset();

            RePacking.Pack(buffer, ref swmpf);
            var fromBuf = RePacking.Unpack<StructWithPrivateField>(buffer);

            Assert.NotEqual(swmpf, fromBuf);
            Assert.Equal(swmpf.Float, fromBuf.Float);
            Assert.Equal(swmpf.Long, fromBuf.Long);
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

            RePacking.Pack(buffer, ref t);

            var fromBuf = RePacking.Unpack<IHavePrivateType.IAmPrivate>(buffer);

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

            RePacking.Pack(buffer, ref hn);

            var fromBuf = RePacking.Unpack<HasNullable>(buffer);

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

            RePacking.Pack(buffer, ref val);
            RePacking.Pack(buffer, ref val2);

            var fromBuf = RePacking.Unpack<float?>(buffer);
            Assert.Equal(val, fromBuf);

            var fromBuf2 = RePacking.Unpack<float?>(buffer);
            Assert.Equal(val2, fromBuf2);
        }

        [Fact]
        public void sealed_class()
        {
            var data = new SealedClass
            {
                Int = 10,
                Float = 1.23456f,
                Class = new NestedSealedClass
                {
                    Double = 234.345234f,
                    Long = 2345645,
                }
            };

            buffer.Reset();

            RePacking.Pack(buffer, ref data);
            var fromBuf = RePacking.Unpack<SealedClass>(buffer);

            Assert.Equal(data.Int, fromBuf.Int);
            Assert.Equal(data.Float, fromBuf.Float);

            Assert.Equal(data.Class.Double, fromBuf.Class.Double);
            Assert.Equal(data.Class.Long, fromBuf.Class.Long);
        }

        [Fact]
        public void internal_sealed_class()
        {
            var data = new InternalSealedClass
            {
                Long = 12352345234,
                Float = 12.234235f,
            };

            buffer.Reset();

            RePacking.Pack(buffer, ref data);
            var fromBuf = RePacking.Unpack<InternalSealedClass>(buffer);

            Assert.Equal(data.Long, fromBuf.Long);
            Assert.Equal(data.Float, fromBuf.Float);
        }

        [Fact]
        public void cant_fit_object_should_unroll()
        {
            var obj = new SomeManagedObject();

            var buffer = new ReBuffer(4);

            Assert.Throws<System.IndexOutOfRangeException>(() => RePacking.Pack(buffer, ref obj));

            Assert.Equal(0, buffer.WriteCursor());
        }

        [Fact]
        public void cant_fit_nested_object_should_unroll()
        {
            var obj = new ParentWithNestedClass();
            obj.Child = new ChildClass();

            var buffer = new ReBuffer(8);

            Assert.Throws<System.IndexOutOfRangeException>(() => RePacking.Pack(buffer, ref obj));

            Assert.Equal(0, buffer.WriteCursor());
        }

        [Fact]
        public void cant_fit_multiple_object_should_unroll()
        {
            var obj = new SomeManagedObject();

            var buffer = new ReBuffer(20);

            RePacking.Pack(buffer, ref obj);
            RePacking.Pack(buffer, ref obj);
            Assert.Throws<System.IndexOutOfRangeException>(() => RePacking.Pack(buffer, ref obj));

            Assert.Equal(16, buffer.WriteCursor());
        }

        [Fact]
        public void cant_fit_multiple_nested_object_should_unroll()
        {
            var obj = new ParentWithNestedClass();
            obj.Child = new ChildClass();

            var buffer = new ReBuffer(38);

            RePacking.Pack(buffer, ref obj);
            RePacking.Pack(buffer, ref obj);
            Assert.Throws<System.IndexOutOfRangeException>(() => RePacking.Pack(buffer, ref obj));

            Assert.Equal(34, buffer.WriteCursor());
        }

        [Fact]
        public void serialize_class_that_has_interface()
        {
            StructHasInterface obj = new StructHasInterface(1234, 1.234f);
            ITestInterface asInterface = (ITestInterface)obj;
            buffer.Reset();

            RePacking.Pack(buffer, ref asInterface);
            string msg = buffer.Array.Take(buffer.WriteCursor()).Aggregate("", (m, b) => m + b.ToString() + " ");
            TestBootstrap.GlobalData.Logger.Log("Buffer: " + msg);

            var fromBuf = RePacking.Unpack<StructHasInterface>(buffer);

            Assert.Equal(obj.Int, fromBuf.Int);
            Assert.Equal(obj.Float, fromBuf.Float);
        }

        [Fact]
        public void serialize_class_that_has_two_of_same_interface()
        {
            StructHasInterface t1 = new StructHasInterface(1234, 1.234f);
            ClassHasInterface t2 = new ClassHasInterface(892345890.23458902345, 9023458902345890);

            ITestInterface asInterface1 = (ITestInterface)t1;
            ITestInterface asInterface2 = (ITestInterface)t2;

            buffer.Reset();

            RePacking.Pack(buffer, ref asInterface1);
            RePacking.Pack(buffer, ref asInterface2);

            StructHasInterface t1FromBuf = RePacking.Unpack<StructHasInterface>(buffer);
            Assert.Equal(t1.Int, t1FromBuf.Int);
            Assert.Equal(t1.Float, t1FromBuf.Float);

            ClassHasInterface t2FromBuf = RePacking.Unpack<ClassHasInterface>(buffer);
            Assert.Equal(t2.Double, t2FromBuf.Double);
            Assert.Equal(t2.Long, t2FromBuf.Long);
        }
    }
}
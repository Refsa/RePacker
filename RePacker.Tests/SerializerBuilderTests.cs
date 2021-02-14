using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Builder;


namespace Refsa.RePacker.Tests
{
    public class SerializerBuilderTests
    {
        public SerializerBuilderTests()
        {
            RePacker.Init();
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

            BoxedBuffer buffer = new BoxedBuffer(1024);

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

            BoxedBuffer buffer = new BoxedBuffer(1024);

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

            BoxedBuffer buffer = new BoxedBuffer(1024);

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

            BoxedBuffer buffer = new BoxedBuffer(1024);

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

            BoxedBuffer buffer = new BoxedBuffer(1024);

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

            BoxedBuffer buffer = new BoxedBuffer(1024);

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

            var buffer = new BoxedBuffer(1024);

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

            var buffer = new BoxedBuffer(1024);

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

            var buffer = new BoxedBuffer(1024);

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

            var buffer = new BoxedBuffer(1024);

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

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref data);
            var fromBuf = RePacker.Unpack<HasUnsupportedField>(buffer);

            Assert.Equal(data.Int, fromBuf.Int);
            Assert.Equal(data.Float, fromBuf.Float);
        }

        [Fact]
        public void struct_with_array_of_unmanaged_types()
        {
            var swa = new StructWithUnmanagedArray
            {
                Int = 1337,
                Long = -123456789,
                Floats = new float[] {
                    0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f
                }
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<StructWithUnmanagedArray>(buffer, ref swa);
            var fromBuf = RePacker.Unpack<StructWithUnmanagedArray>(buffer);

            Assert.Equal(swa.Int, fromBuf.Int);
            Assert.Equal(swa.Long, fromBuf.Long);

            for (int i = 0; i < swa.Floats.Length; i++)
            {
                Assert.Equal(swa.Floats[i], fromBuf.Floats[i]);
            }
        }

        [Fact]
        public void struct_with_array_of_blittable_types()
        {
            var swa = new StructWithBlittableArray
            {
                Int = 1337,
                Long = -123456789,
                Blittable = new ChildStruct[] {
                    new ChildStruct
                    {
                        Float = 123f,
                        Byte = 120,
                    },
                    new ChildStruct
                    {
                        Float = 63456f,
                        Byte = 14,
                    }
                    ,
                    new ChildStruct
                    {
                        Float = 465f,
                        Byte = 2,
                    },
                    new ChildStruct
                    {
                        Float = 6345f,
                        Byte = 239,
                    }
                }
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<StructWithBlittableArray>(buffer, ref swa);
            var fromBuf = RePacker.Unpack<StructWithBlittableArray>(buffer);

            Assert.Equal(swa.Int, fromBuf.Int);
            Assert.Equal(swa.Long, fromBuf.Long);

            for (int i = 0; i < swa.Blittable.Length; i++)
            {
                Assert.Equal(swa.Blittable[i].Float, fromBuf.Blittable[i].Float);
                Assert.Equal(swa.Blittable[i].Byte, fromBuf.Blittable[i].Byte);
            }
        }

        [Fact]
        public void type_with_array_of_supported_class()
        {
            var iac = new HasClassArray
            {
                Long = 123412341234,
                Byte = 23,
                ArrayOfClass = new InArrayClass[]
                {
                    new InArrayClass{
                        Float = 1234f,
                        Int = 82345,
                    },
                    new InArrayClass{
                        Float = 7645f,
                        Int = 1234356,
                    },
                    new InArrayClass{
                        Float = 74f,
                        Int = 43,
                    },
                    new InArrayClass{
                        Float = 4567f,
                        Int = 46723,
                    }
                }
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<HasClassArray>(buffer, ref iac);
            var fromBuf = RePacker.Unpack<HasClassArray>(buffer);

            Assert.Equal(iac.Byte, fromBuf.Byte);
            Assert.Equal(iac.Long, fromBuf.Long);

            Assert.Equal(iac.ArrayOfClass.Length, fromBuf.ArrayOfClass.Length);

            for (int i = 0; i < iac.ArrayOfClass.Length; i++)
            {
                Assert.Equal(iac.ArrayOfClass[i].Float, fromBuf.ArrayOfClass[i].Float);
                Assert.Equal(iac.ArrayOfClass[i].Int, fromBuf.ArrayOfClass[i].Int);
            }
        }

        [Fact]
        public void ilist_with_unmanaged_type()
        {
            var hml = new HasUnmanagedIList
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 },
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HasUnmanagedIList>(buffer);

            Assert.Equal(hml.Float, fromBuf.Float);
            Assert.Equal(hml.Double, fromBuf.Double);

            Assert.False(fromBuf.Ints == null);

            for (int i = 0; i < hml.Ints.Count; i++)
            {
                Assert.Equal(hml.Ints[i], fromBuf.Ints[i]);
            }
        }

        /* [Fact]
        public void ilist_with_unmanaged_type_direct()
        {
            var hml = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<IList<int>>(buffer);

            Assert.False(fromBuf == null);

            for (int i = 0; i < hml.Count; i++)
            {
                Assert.Equal(hml[i], fromBuf[i]);
            }
        } */

        [Fact]
        public void list_with_unmanaged_type()
        {
            var hml = new HasUnmanagedList
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 },
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HasUnmanagedList>(buffer);

            Assert.Equal(hml.Float, fromBuf.Float);
            Assert.Equal(hml.Double, fromBuf.Double);

            Assert.False(fromBuf.Ints == null);

            for (int i = 0; i < hml.Ints.Count; i++)
            {
                Assert.Equal(hml.Ints[i], fromBuf.Ints[i]);
            }
        }

        [Fact]
        public void list_with_unmanaged_type_direct()
        {
            var hml = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<List<int>>(buffer);

            Assert.False(fromBuf == null);

            for (int i = 0; i < hml.Count; i++)
            {
                Assert.Equal(hml[i], fromBuf[i]);
            }
        }

        [Fact]
        public void queue_with_unmanaged_type()
        {
            var hml = new HasUnmanagedQueue
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new Queue<int>(System.Linq.Enumerable.Range(1, 10)),
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HasUnmanagedQueue>(buffer);

            Assert.Equal(hml.Float, fromBuf.Float);
            Assert.Equal(hml.Double, fromBuf.Double);

            Assert.False(fromBuf.Ints == null);

            for (int i = 0; i < hml.Ints.Count; i++)
            {
                Assert.Equal(hml.Ints.Dequeue(), fromBuf.Ints.Dequeue());
            }
        }

        [Fact]
        public void queue_with_unmanaged_type_direct()
        {
            var hml = new Queue<int>(System.Linq.Enumerable.Range(1, 10));

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<Queue<int>>(buffer);

            Assert.False(fromBuf == null);

            for (int i = 0; i < hml.Count; i++)
            {
                Assert.Equal(hml.Dequeue(), fromBuf.Dequeue());
            }
        }

        [Fact]
        public void stack_with_unmanaged_type()
        {
            var hml = new HasUnmanagedStack
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new Stack<int>(System.Linq.Enumerable.Range(1, 10)),
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HasUnmanagedStack>(buffer);

            Assert.Equal(hml.Float, fromBuf.Float);
            Assert.Equal(hml.Double, fromBuf.Double);

            Assert.False(fromBuf.Ints == null);

            for (int i = 0; i < hml.Ints.Count; i++)
            {
                Assert.Equal(hml.Ints.Pop(), fromBuf.Ints.Pop());
            }
        }

        [Fact]
        public void stack_with_unmanaged_type_direct()
        {
            var hml = new Stack<int>(System.Linq.Enumerable.Range(1, 10));

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<Stack<int>>(buffer);

            Assert.False(fromBuf == null);

            for (int i = 0; i < hml.Count; i++)
            {
                Assert.Equal(hml.Pop(), fromBuf.Pop());
            }
        }

        [Fact]
        public void hashset_with_unmanaged_type()
        {
            var hml = new HasUnmanagedHashSet
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new HashSet<int>(System.Linq.Enumerable.Range(1, 10)),
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HasUnmanagedHashSet>(buffer);

            Assert.Equal(hml.Float, fromBuf.Float);
            Assert.Equal(hml.Double, fromBuf.Double);

            Assert.False(fromBuf.Ints == null);

            for (int i = 0; i < hml.Ints.Count; i++)
            {
                Assert.Equal(hml.Ints.ElementAt(i), fromBuf.Ints.ElementAt(i));
            }
        }

        [Fact]
        public void hashset_with_unmanaged_type_direct()
        {
            var hml = new HashSet<int>(System.Linq.Enumerable.Range(1, 10));

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HashSet<int>>(buffer);

            Assert.False(fromBuf == null);

            for (int i = 0; i < hml.Count; i++)
            {
                Assert.Equal(hml.ElementAt(i), fromBuf.ElementAt(i));
            }
        }

        #region ManagedContainers

        [Fact]
        public void ilist_with_managed_type()
        {
            var hml = new HasManagedIList
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new List<SomeManagedObject> {
                    new SomeManagedObject{Float = 12.34f, Int = 901234},
                    new SomeManagedObject{Float = 567.25f, Int = 4562},
                    new SomeManagedObject{Float = 1.3245f, Int = 24535},
                    new SomeManagedObject{Float = 56.2435f, Int = 56724},
                },
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HasManagedIList>(buffer);

            Assert.Equal(hml.Float, fromBuf.Float);
            Assert.Equal(hml.Double, fromBuf.Double);

            Assert.False(fromBuf.Ints == null);

            for (int i = 0; i < hml.Ints.Count; i++)
            {
                Assert.Equal(hml.Ints[i].Int, fromBuf.Ints[i].Int);
                Assert.Equal(hml.Ints[i].Float, fromBuf.Ints[i].Float);
            }
        }

        /* [Fact]
        public void ilist_with_managed_type_direct()
        {
            var hml = new List<SomeManagedObject> {
                new SomeManagedObject{Float = 12.34f, Int = 901234},
                new SomeManagedObject{Float = 567.25f, Int = 4562},
                new SomeManagedObject{Float = 1.3245f, Int = 24535},
                new SomeManagedObject{Float = 56.2435f, Int = 56724},
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<IList<SomeManagedObject>>(buffer);

            Assert.False(fromBuf == null);

            for (int i = 0; i < hml.Count; i++)
            {
                Assert.Equal(hml[i].Int, fromBuf[i].Int);
                Assert.Equal(hml[i].Float, fromBuf[i].Float);
            }
        } */

        [Fact]
        public void list_with_managed_type()
        {
            var hml = new HasManagedList
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new List<SomeManagedObject> {
                    new SomeManagedObject{Float = 12.34f, Int = 901234},
                    new SomeManagedObject{Float = 567.25f, Int = 4562},
                    new SomeManagedObject{Float = 1.3245f, Int = 24535},
                    new SomeManagedObject{Float = 56.2435f, Int = 56724},
                },
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HasManagedList>(buffer);

            Assert.Equal(hml.Float, fromBuf.Float);
            Assert.Equal(hml.Double, fromBuf.Double);

            Assert.False(fromBuf.Ints == null);

            for (int i = 0; i < hml.Ints.Count; i++)
            {
                Assert.Equal(hml.Ints[i].Int, fromBuf.Ints[i].Int);
                Assert.Equal(hml.Ints[i].Float, fromBuf.Ints[i].Float);
            }
        }

        [Fact]
        public void list_with_managed_type_direct()
        {
            var hml = new List<SomeManagedObject> {
                new SomeManagedObject{Float = 12.34f, Int = 901234},
                new SomeManagedObject{Float = 567.25f, Int = 4562},
                new SomeManagedObject{Float = 1.3245f, Int = 24535},
                new SomeManagedObject{Float = 56.2435f, Int = 56724},
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<List<SomeManagedObject>>(buffer);

            Assert.False(fromBuf == null);

            for (int i = 0; i < hml.Count; i++)
            {
                Assert.Equal(hml[i].Int, fromBuf[i].Int);
                Assert.Equal(hml[i].Float, fromBuf[i].Float);
            }
        }

        [Fact]
        public void queue_with_managed_type()
        {
            var hml = new HasManagedQueue
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new Queue<SomeManagedObject>(
                    new List<SomeManagedObject>() {
                        new SomeManagedObject{Float = 12.34f, Int = 901234},
                        new SomeManagedObject{Float = 567.25f, Int = 4562},
                        new SomeManagedObject{Float = 1.3245f, Int = 24535},
                        new SomeManagedObject{Float = 56.2435f, Int = 56724},
                    }
                ),
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HasManagedQueue>(buffer);

            Assert.Equal(hml.Float, fromBuf.Float);
            Assert.Equal(hml.Double, fromBuf.Double);

            Assert.False(fromBuf.Ints == null);

            for (int i = 0; i < hml.Ints.Count; i++)
            {
                var elementA = hml.Ints.Dequeue();
                var elementB = fromBuf.Ints.Dequeue();
                Assert.Equal(elementA.Int, elementB.Int);
                Assert.Equal(elementA.Float, elementB.Float);
            }
        }

        [Fact]
        public void queue_with_managed_type_direct()
        {
            var hml = new Queue<SomeManagedObject>(
                new List<SomeManagedObject>() {
                    new SomeManagedObject{Float = 12.34f, Int = 901234},
                    new SomeManagedObject{Float = 567.25f, Int = 4562},
                    new SomeManagedObject{Float = 1.3245f, Int = 24535},
                    new SomeManagedObject{Float = 56.2435f, Int = 56724},
                }
            );

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<Queue<SomeManagedObject>>(buffer);

            Assert.False(fromBuf == null);

            for (int i = 0; i < hml.Count; i++)
            {
                var elementA = hml.Dequeue();
                var elementB = fromBuf.Dequeue();
                Assert.Equal(elementA.Int, elementB.Int);
                Assert.Equal(elementA.Float, elementB.Float);
            }
        }

        [Fact]
        public void stack_with_managed_type()
        {
            var hml = new HasManagedStack
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new Stack<SomeManagedObject>(
                    new List<SomeManagedObject>() {
                        new SomeManagedObject{Float = 12.34f, Int = 901234},
                        new SomeManagedObject{Float = 567.25f, Int = 4562},
                        new SomeManagedObject{Float = 1.3245f, Int = 24535},
                        new SomeManagedObject{Float = 56.2435f, Int = 56724},
                    }
                ),
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HasManagedStack>(buffer);

            Assert.Equal(hml.Float, fromBuf.Float);
            Assert.Equal(hml.Double, fromBuf.Double);

            Assert.False(fromBuf.Ints == null);

            for (int i = 0; i < hml.Ints.Count; i++)
            {
                var elementA = hml.Ints.Pop();
                var elementB = fromBuf.Ints.Pop();
                Assert.Equal(elementA.Int, elementB.Int);
                Assert.Equal(elementA.Float, elementB.Float);
            }
        }

        [Fact]
        public void stack_with_managed_type_direct()
        {
            var hml = new Stack<SomeManagedObject>(
                new List<SomeManagedObject>() {
                    new SomeManagedObject{Float = 12.34f, Int = 901234},
                    new SomeManagedObject{Float = 567.25f, Int = 4562},
                    new SomeManagedObject{Float = 1.3245f, Int = 24535},
                    new SomeManagedObject{Float = 56.2435f, Int = 56724},
                }
            );

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<Stack<SomeManagedObject>>(buffer);

            Assert.False(fromBuf == null);

            for (int i = 0; i < hml.Count; i++)
            {
                var elementA = hml.Pop();
                var elementB = fromBuf.Pop();
                Assert.Equal(elementA.Int, elementB.Int);
                Assert.Equal(elementA.Float, elementB.Float);
            }
        }

        [Fact]
        public void hashset_with_managed_type()
        {
            var hml = new HasManagedHashSet
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new HashSet<SomeManagedObject>(
                    new List<SomeManagedObject>() {
                        new SomeManagedObject{Float = 12.34f, Int = 901234},
                        new SomeManagedObject{Float = 567.25f, Int = 4562},
                        new SomeManagedObject{Float = 1.3245f, Int = 24535},
                        new SomeManagedObject{Float = 56.2435f, Int = 56724},
                    }
                ),
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HasManagedHashSet>(buffer);

            Assert.Equal(hml.Float, fromBuf.Float);
            Assert.Equal(hml.Double, fromBuf.Double);

            Assert.False(fromBuf.Ints == null);

            for (int i = 0; i < hml.Ints.Count; i++)
            {
                var elementA = hml.Ints.ElementAt(i);
                var elementB = fromBuf.Ints.ElementAt(i);
                Assert.Equal(elementA.Int, elementB.Int);
                Assert.Equal(elementA.Float, elementB.Float);
            }
        }

        [Fact]
        public void hashset_with_managed_type_direct()
        {
            var hml = new HashSet<SomeManagedObject>(
                new List<SomeManagedObject>() {
                    new SomeManagedObject{Float = 12.34f, Int = 901234},
                    new SomeManagedObject{Float = 567.25f, Int = 4562},
                    new SomeManagedObject{Float = 1.3245f, Int = 24535},
                    new SomeManagedObject{Float = 56.2435f, Int = 56724},
                }
            );

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<HashSet<SomeManagedObject>>(buffer);

            Assert.False(fromBuf == null);

            for (int i = 0; i < hml.Count; i++)
            {
                var elementA = hml.ElementAt(i);
                var elementB = fromBuf.ElementAt(i);
                Assert.Equal(elementA.Int, elementB.Int);
                Assert.Equal(elementA.Float, elementB.Float);
            }
        }

        [Fact]
        public void dictionary_with_unmanaged_key_and_value()
        {
            var dictCont = new HasUnmanagedDictionary
            {
                Float = 1234.4562345f,
                Long = 23568913457,
                Dict = new Dictionary<int, float> {
                    {123, 12.5f},
                    {4567456, 125.2345f},
                    {23454, 3456.235f},
                    {345643, 3456234.1341f},
                    {89678, 1234.3523f},
                    {452, .654567f},
                }
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref dictCont);
            var fromBuf = RePacker.Unpack<HasUnmanagedDictionary>(buffer);

            foreach (var key in dictCont.Dict.Keys)
            {
                var wantedValue = dictCont.Dict[key];

                Assert.True(fromBuf.Dict.TryGetValue(key, out float value));
                Assert.Equal(wantedValue, value);
            }
        }

        [Fact]
        public void dictionary_with_unmanaged_key_and_value_direct()
        {
            var dictCont = new Dictionary<int, float> {
                {123, 12.5f},
                {4567456, 125.2345f},
                {23454, 3456.235f},
                {345643, 3456234.1341f},
                {89678, 1234.3523f},
                {452, .654567f},
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref dictCont);
            var fromBuf = RePacker.Unpack<Dictionary<int, float>>(buffer);

            foreach (var key in dictCont.Keys)
            {
                var wantedValue = dictCont[key];

                Assert.True(fromBuf.TryGetValue(key, out float value));
                Assert.Equal(wantedValue, value);
            }
        }

        [Fact]
        public void dictionary_with_managed_key_and_unmanaged_value()
        {
            var dictCont = new HasManagedKeyUnmanagedValueDict
            {
                Float = 1234.4562345f,
                Long = 23568913457,
                Dict = new Dictionary<SomeManagedObject, float> {
                    {new SomeManagedObject{Float = 12.34f, Int = 901234}, 12.5f},
                    {new SomeManagedObject{Float = 567.25f, Int = 4562}, 125.2345f},
                    {new SomeManagedObject{Float = 1.3245f, Int = 24535}, 3456.235f},
                    {new SomeManagedObject{Float = 56.2435f, Int = 56724}, 3456234.1341f},
                }
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref dictCont);
            var fromBuf = RePacker.Unpack<HasManagedKeyUnmanagedValueDict>(buffer);

            for (int i = 0; i < dictCont.Dict.Count; i++)
            {
                var wantedKey = dictCont.Dict.Keys.ElementAt(i);
                var key = fromBuf.Dict.Keys.ElementAt(i);

                Assert.Equal(wantedKey.Float, key.Float);
                Assert.Equal(wantedKey.Int, key.Int);

                var wantedValue = dictCont.Dict.Values.ElementAt(i);
                var value = fromBuf.Dict.Values.ElementAt(i);

                Assert.Equal(wantedValue, value);
            }
        }

        [Fact]
        public void dictionary_with_managed_key_and_unmanaged_value_direct()
        {
            var dictCont = new Dictionary<SomeManagedObject, float> {
                {new SomeManagedObject{Float = 12.34f, Int = 901234}, 12.5f},
                {new SomeManagedObject{Float = 567.25f, Int = 4562}, 125.2345f},
                {new SomeManagedObject{Float = 1.3245f, Int = 24535}, 3456.235f},
                {new SomeManagedObject{Float = 56.2435f, Int = 56724}, 3456234.1341f},
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref dictCont);
            var fromBuf = RePacker.Unpack<Dictionary<SomeManagedObject, float>>(buffer);

            for (int i = 0; i < dictCont.Count; i++)
            {
                var wantedKey = dictCont.Keys.ElementAt(i);
                var key = fromBuf.Keys.ElementAt(i);

                Assert.Equal(wantedKey.Float, key.Float);
                Assert.Equal(wantedKey.Int, key.Int);

                var wantedValue = dictCont.Values.ElementAt(i);
                var value = fromBuf.Values.ElementAt(i);

                Assert.Equal(wantedValue, value);
            }
        }

        [Fact]
        public void dictionary_with_unmanaged_key_and_managed_value()
        {
            var dictCont = new HasUnmanagedKeyManagedValueDict
            {
                Float = 1234.4562345f,
                Long = 23568913457,
                Dict = new Dictionary<int, SomeManagedObject> {
                    {1, new SomeManagedObject{Float = 12.34f, Int = 901234}},
                    {10, new SomeManagedObject{Float = 567.25f, Int = 4562}},
                    {100, new SomeManagedObject{Float = 1.3245f, Int = 24535}},
                    {1000, new SomeManagedObject{Float = 56.2435f, Int = 56724}},
                }
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref dictCont);
            var fromBuf = RePacker.Unpack<HasUnmanagedKeyManagedValueDict>(buffer);

            for (int i = 0; i < dictCont.Dict.Count; i++)
            {
                var wantedKey = dictCont.Dict.Keys.ElementAt(i);
                var key = fromBuf.Dict.Keys.ElementAt(i);

                Assert.Equal(wantedKey, key);

                var wantedValue = dictCont.Dict.Values.ElementAt(i);
                var value = fromBuf.Dict.Values.ElementAt(i);

                Assert.Equal(wantedValue.Float, value.Float);
                Assert.Equal(wantedValue.Int, value.Int);
            }
        }

        [Fact]
        public void dictionary_with_unmanaged_key_and_managed_value_direct()
        {
            var dictCont = new Dictionary<int, SomeManagedObject> {
                {1, new SomeManagedObject{Float = 12.34f, Int = 901234}},
                {10, new SomeManagedObject{Float = 567.25f, Int = 4562}},
                {100, new SomeManagedObject{Float = 1.3245f, Int = 24535}},
                {1000, new SomeManagedObject{Float = 56.2435f, Int = 56724}}
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref dictCont);
            var fromBuf = RePacker.Unpack<Dictionary<int, SomeManagedObject>>(buffer);

            for (int i = 0; i < dictCont.Count; i++)
            {
                var wantedKey = dictCont.Keys.ElementAt(i);
                var key = fromBuf.Keys.ElementAt(i);

                Assert.Equal(wantedKey, key);

                var wantedValue = dictCont.Values.ElementAt(i);
                var value = fromBuf.Values.ElementAt(i);

                Assert.Equal(wantedValue.Float, value.Float);
                Assert.Equal(wantedValue.Int, value.Int);
            }
        }

        [Fact]
        public void dictionary_with_managed_key_and_managed_value()
        {
            var dictCont = new HasManagedKeyManagedValueDict
            {
                Float = 1234.4562345f,
                Long = 23568913457,
                Dict = new Dictionary<SomeManagedObject, SomeManagedObject> {
                    {new SomeManagedObject{Float = 12.34f, Int = 901234}, new SomeManagedObject{Float = 12.34f, Int = 901234}},
                    {new SomeManagedObject{Float = 567.25f, Int = 4562}, new SomeManagedObject{Float = 567.25f, Int = 4562}},
                    {new SomeManagedObject{Float = 1.3245f, Int = 24535}, new SomeManagedObject{Float = 1.3245f, Int = 24535}},
                    {new SomeManagedObject{Float = 56.2435f, Int = 56724}, new SomeManagedObject{Float = 56.2435f, Int = 56724}},
                }
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref dictCont);
            var fromBuf = RePacker.Unpack<HasManagedKeyManagedValueDict>(buffer);

            for (int i = 0; i < dictCont.Dict.Count; i++)
            {
                var wantedKey = dictCont.Dict.Keys.ElementAt(i);
                var key = fromBuf.Dict.Keys.ElementAt(i);

                Assert.Equal(wantedKey.Float, key.Float);
                Assert.Equal(wantedKey.Int, key.Int);

                var wantedValue = dictCont.Dict.Values.ElementAt(i);
                var value = fromBuf.Dict.Values.ElementAt(i);

                Assert.Equal(wantedValue.Float, value.Float);
                Assert.Equal(wantedValue.Int, value.Int);
            }
        }

        [Fact]
        public void primitives_standalone()
        {
            var buffer = new BoxedBuffer(1024);

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

        [Fact]
        public void date_time_buffer_packing()
        {
            DateTime dt = DateTime.Now;

            var buffer = new BoxedBuffer(1024);

            buffer.PackDateTime(ref dt);
            buffer.UnpackDateTime(out DateTime fromBuf);

            Assert.Equal(dt.Ticks, fromBuf.Ticks);
        }

        [Fact]
        public void has_date_time_packing()
        {
            var hdt = new HasDateTime { Float = 1.2344534f, DateTime = DateTime.Now };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hdt);
            var fromBuf = RePacker.Unpack<HasDateTime>(buffer);

            Assert.Equal(hdt.Float, fromBuf.Float);
            Assert.Equal(hdt.DateTime, fromBuf.DateTime);
        }

        [Fact]
        public void standalone_date_time_repacker()
        {
            DateTime dt = DateTime.Now;

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<DateTime>(buffer, ref dt);
            DateTime fromBuf = RePacker.Unpack<DateTime>(buffer);

            Assert.Equal(dt.Ticks, fromBuf.Ticks);
        }

        [Fact]
        public void has_regular_string()
        {
            var hs = new HasString { Float = 1212345.123451f, String = "This is some message" };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hs);
            var fromBuf = RePacker.Unpack<HasString>(buffer);

            Assert.Equal(hs.Float, fromBuf.Float);
            Assert.Equal(hs.String, fromBuf.String);
        }

        [Fact]
        public void standalone_string_repacker()
        {
            string value = "abrakadabra this is a magic trick";

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref value);
            string fromBuf = RePacker.Unpack<string>(buffer);

            Assert.Equal(value, fromBuf);
        }

        [Fact]
        public void standalone_key_value_pair_unmanaged()
        {
            var kvp = new KeyValuePair<int, int>(10, 100);

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref kvp);

            var fromBuf = RePacker.Unpack<KeyValuePair<int, int>>(buffer);

            Assert.Equal(kvp.Key, fromBuf.Key);
            Assert.Equal(kvp.Value, fromBuf.Value);
        }

        [Fact]
        public void standalone_key_value_pair_managed_key()
        {
            var kvp = new KeyValuePair<SimpleClass, int>(new SimpleClass { Float = 12.34f }, 100);

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref kvp);

            var fromBuf = RePacker.Unpack<KeyValuePair<SimpleClass, int>>(buffer);

            Assert.Equal(kvp.Key.Float, fromBuf.Key.Float);
            Assert.Equal(kvp.Value, fromBuf.Value);
        }

        [Fact]
        public void standalone_key_value_pair_managed_value()
        {
            var kvp = new KeyValuePair<int, SimpleClass>(100, new SimpleClass { Float = 12.34f });

            var buffer = new BoxedBuffer(1024);

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

            var buffer = new BoxedBuffer(1024);

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

            var buffer = new BoxedBuffer(1024);

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

            var buffer = new BoxedBuffer(1024);

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

            var buffer = new BoxedBuffer(1024);

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

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref t);

            var fromBuf = RePacker.Unpack<IHavePrivateType.IAmPrivate>(buffer);

            Assert.Equal(t.Float, fromBuf.Float);
            Assert.Equal(t.Int, fromBuf.Int);
        }
    }
    #endregion
}
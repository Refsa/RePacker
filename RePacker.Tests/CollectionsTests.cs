using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using RePacker.Buffers;
using RePacker.Builder;
using Xunit.Abstractions;

namespace RePacker.Tests
{
    public class CollectionsTests
    {
        [Fact]
        public void unmanaged_null_array()
        {
            int[] array = null;

            var buffer = new BoxedBuffer(1024);

            buffer.Pack(ref array);

            var fromBuf = buffer.Unpack<int[]>();

            Assert.NotNull(fromBuf);
            Assert.Equal(0, fromBuf.Length);
        }

        [Fact]
        public void managed_null_array()
        {
            SomeManagedObject[] array = null;

            var buffer = new BoxedBuffer(1024);

            buffer.Pack(ref array);

            var fromBuf = buffer.Unpack<SomeManagedObject[]>();

            Assert.NotNull(fromBuf);
            Assert.Equal(0, fromBuf.Length);
        }

        [Fact]
        public void unmanaged_with_unmanaged_array()
        {
            StructWithUnmanagedArray data = new StructWithUnmanagedArray
            {
                Int = 1234,
                Floats = null,
                Long = 1234567890
            };

            var buffer = new BoxedBuffer(1024);

            buffer.Pack(ref data);

            var fromBuf = buffer.Unpack<StructWithUnmanagedArray>();

            Assert.Equal(data.Int, fromBuf.Int);
            Assert.Equal(data.Long, fromBuf.Long);

            Assert.NotNull(fromBuf.Floats);
            Assert.Equal(0, fromBuf.Floats.Length);
        }

        [Fact]
        public void unmanaged_with_managed_array()
        {
            HasClassArray data = new HasClassArray
            {
                Byte = 123,
                ArrayOfClass = null,
                Long = 1234567890
            };

            var buffer = new BoxedBuffer(1024);

            buffer.Pack(ref data);

            var fromBuf = buffer.Unpack<HasClassArray>();

            Assert.Equal(data.Byte, fromBuf.Byte);
            Assert.Equal(data.Long, fromBuf.Long);

            Assert.NotNull(fromBuf.ArrayOfClass);
            Assert.Equal(0, fromBuf.ArrayOfClass.Length);
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

        [Fact]
        public void ilist_with_unmanaged_type_direct()
        {
            IList<int> hml = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);

            var fromBuf = RePacker.Unpack<IList<int>>(buffer);

            Assert.False(fromBuf == null);

            for (int i = 0; i < hml.Count; i++)
            {
                Assert.Equal(hml[i], fromBuf[i]);
            }
        }

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

        [Fact]
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
        }

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

            Assert.Equal(dictCont.Dict.Count, fromBuf.Dict.Count);

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

            Assert.Equal(dictCont.Dict.Count, fromBuf.Dict.Count);

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

            Assert.Equal(dictCont.Dict.Count, fromBuf.Dict.Count);

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
    }
}
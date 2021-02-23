using Xunit;
using RePacker.Buffers;
using RePacker;
using System.Runtime.InteropServices;
using System;
using Buffer = RePacker.Buffers.Buffer;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using System.Reflection;

namespace RePacker.Tests
{

    public class ValueTupleTests
    {
        public ValueTupleTests(ITestOutputHelper output)
        {
            TestBootstrap.Setup(output);
        }

        [Fact]
        public void ValueTuple_2_unmanaged()
        {
            var vt2 = new ValueTuple<int, int>(10, 100);

            var buffer = new BoxedBuffer(1024);

            RePacking.Pack(buffer, ref vt2);
            var fromBuf = RePacking.Unpack<ValueTuple<int, int>>(buffer);

            Assert.Equal(vt2.Item1, fromBuf.Item1);
            Assert.Equal(vt2.Item2, fromBuf.Item2);
        }

        [Fact]
        public void ValueTuple_3_unmanaged()
        {
            var vt2 = new ValueTuple<int, int, int>(10, 100, 1000);

            var buffer = new BoxedBuffer(1024);

            RePacking.Pack(buffer, ref vt2);
            var fromBuf = RePacking.Unpack<ValueTuple<int, int, int>>(buffer);

            Assert.Equal(vt2.Item1, fromBuf.Item1);
            Assert.Equal(vt2.Item2, fromBuf.Item2);
            Assert.Equal(vt2.Item3, fromBuf.Item3);
        }

        [Fact]
        public void ValueTuple_4_unmanaged()
        {
            var vt2 = new ValueTuple<int, int, int, int>(10, 100, 1000, 10000);

            var buffer = new BoxedBuffer(1024);

            RePacking.Pack(buffer, ref vt2);
            var fromBuf = RePacking.Unpack<ValueTuple<int, int, int, int>>(buffer);

            Assert.Equal(vt2.Item1, fromBuf.Item1);
            Assert.Equal(vt2.Item2, fromBuf.Item2);
            Assert.Equal(vt2.Item3, fromBuf.Item3);
            Assert.Equal(vt2.Item4, fromBuf.Item4);
        }

        [Fact]
        public void ValueTuple_5_unmanaged()
        {
            var vt2 = new ValueTuple<int, int, int, int, int>(10, 100, 1000, 10000, 100000);

            var buffer = new BoxedBuffer(1024);

            RePacking.Pack(buffer, ref vt2);
            var fromBuf = RePacking.Unpack<ValueTuple<int, int, int, int, int>>(buffer);

            Assert.Equal(vt2.Item1, fromBuf.Item1);
            Assert.Equal(vt2.Item2, fromBuf.Item2);
            Assert.Equal(vt2.Item3, fromBuf.Item3);
            Assert.Equal(vt2.Item4, fromBuf.Item4);
            Assert.Equal(vt2.Item5, fromBuf.Item5);
        }

        [Fact]
        public void ValueTuple_6_unmanaged()
        {
            var vt2 = new ValueTuple<int, int, int, int, int, int>(10, 100, 1000, 10000, 100000, 1000000);

            var buffer = new BoxedBuffer(1024);

            RePacking.Pack(buffer, ref vt2);
            var fromBuf = RePacking.Unpack<ValueTuple<int, int, int, int, int, int>>(buffer);

            Assert.Equal(vt2.Item1, fromBuf.Item1);
            Assert.Equal(vt2.Item2, fromBuf.Item2);
            Assert.Equal(vt2.Item3, fromBuf.Item3);
            Assert.Equal(vt2.Item4, fromBuf.Item4);
            Assert.Equal(vt2.Item5, fromBuf.Item5);
            Assert.Equal(vt2.Item6, fromBuf.Item6);
        }

        [Fact]
        public void ValueTuple_2_managed()
        {
            var vt2 = new ValueTuple<SimpleClass, SimpleClass>(new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f });

            var buffer = new BoxedBuffer(1024);

            RePacking.Pack(buffer, ref vt2);
            var fromBuf = RePacking.Unpack<ValueTuple<SimpleClass, SimpleClass>>(buffer);

            Assert.Equal(vt2.Item1.Float, fromBuf.Item1.Float);
            Assert.Equal(vt2.Item2.Float, fromBuf.Item2.Float);
        }

        [Fact]
        public void ValueTuple_3_managed()
        {
            var vt2 = new ValueTuple<SimpleClass, SimpleClass, SimpleClass>(new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f });

            var buffer = new BoxedBuffer(1024);

            RePacking.Pack(buffer, ref vt2);
            var fromBuf = RePacking.Unpack<ValueTuple<SimpleClass, SimpleClass, SimpleClass>>(buffer);

            Assert.Equal(vt2.Item1.Float, fromBuf.Item1.Float);
            Assert.Equal(vt2.Item2.Float, fromBuf.Item2.Float);
            Assert.Equal(vt2.Item3.Float, fromBuf.Item3.Float);
        }

        [Fact]
        public void ValueTuple_4_managed()
        {
            var vt2 = new ValueTuple<SimpleClass, SimpleClass, SimpleClass, SimpleClass>(new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f });

            var buffer = new BoxedBuffer(1024);

            RePacking.Pack(buffer, ref vt2);
            var fromBuf = RePacking.Unpack<ValueTuple<SimpleClass, SimpleClass, SimpleClass, SimpleClass>>(buffer);

            Assert.Equal(vt2.Item1.Float, fromBuf.Item1.Float);
            Assert.Equal(vt2.Item2.Float, fromBuf.Item2.Float);
            Assert.Equal(vt2.Item3.Float, fromBuf.Item3.Float);
            Assert.Equal(vt2.Item4.Float, fromBuf.Item4.Float);
        }

        [Fact]
        public void ValueTuple_5_managed()
        {
            var vt2 = new ValueTuple<SimpleClass, SimpleClass, SimpleClass, SimpleClass, SimpleClass>(new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f });

            var buffer = new BoxedBuffer(1024);

            RePacking.Pack(buffer, ref vt2);
            var fromBuf = RePacking.Unpack<ValueTuple<SimpleClass, SimpleClass, SimpleClass, SimpleClass, SimpleClass>>(buffer);

            Assert.Equal(vt2.Item1.Float, fromBuf.Item1.Float);
            Assert.Equal(vt2.Item2.Float, fromBuf.Item2.Float);
            Assert.Equal(vt2.Item3.Float, fromBuf.Item3.Float);
            Assert.Equal(vt2.Item4.Float, fromBuf.Item4.Float);
            Assert.Equal(vt2.Item5.Float, fromBuf.Item5.Float);
        }

        [Fact]
        public void ValueTuple_6_managed()
        {
            var vt2 = new ValueTuple<SimpleClass, SimpleClass, SimpleClass, SimpleClass, SimpleClass, SimpleClass>(new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f }, new SimpleClass { Float = 12.34f });

            var buffer = new BoxedBuffer(1024);

            RePacking.Pack(buffer, ref vt2);
            var fromBuf = RePacking.Unpack<ValueTuple<SimpleClass, SimpleClass, SimpleClass, SimpleClass, SimpleClass, SimpleClass>>(buffer);

            Assert.Equal(vt2.Item1.Float, fromBuf.Item1.Float);
            Assert.Equal(vt2.Item2.Float, fromBuf.Item2.Float);
            Assert.Equal(vt2.Item3.Float, fromBuf.Item3.Float);
            Assert.Equal(vt2.Item4.Float, fromBuf.Item4.Float);
            Assert.Equal(vt2.Item5.Float, fromBuf.Item5.Float);
            Assert.Equal(vt2.Item6.Float, fromBuf.Item6.Float);
        }

        [RePacker]
        public struct StructWithValueTuple
        {
            public ValueTuple<int, int> VT2;
            public ValueTuple<int, int, int> VT3;
            public ValueTuple<int, int, int, int> VT4;
            public ValueTuple<int, int, int, int, int> VT5;
            public ValueTuple<int, int, int, int, int, int> VT6;
        }

        [Fact]
        public void struct_with_value_tuples()
        {
            var valuetuples = new StructWithValueTuple
            {
                VT2 = (10, 100),
                VT3 = (10, 100, 1000),
                VT4 = (10, 100, 1000, 10000),
                VT5 = (10, 100, 1000, 10000, 100000),
                VT6 = (10, 100, 1000, 10000, 100000, 1000000),
            };

            var buffer = new BoxedBuffer(1024);
            RePacking.Pack(buffer, ref valuetuples);

            var frombuf = RePacking.Unpack<StructWithValueTuple>(buffer);

            Assert.Equal(valuetuples.VT2.Item1, frombuf.VT2.Item1);
            Assert.Equal(valuetuples.VT2.Item2, frombuf.VT2.Item2);

            Assert.Equal(valuetuples.VT3.Item1, frombuf.VT3.Item1);
            Assert.Equal(valuetuples.VT3.Item2, frombuf.VT3.Item2);
            Assert.Equal(valuetuples.VT3.Item3, frombuf.VT3.Item3);

            Assert.Equal(valuetuples.VT4.Item1, frombuf.VT4.Item1);
            Assert.Equal(valuetuples.VT4.Item2, frombuf.VT4.Item2);
            Assert.Equal(valuetuples.VT4.Item3, frombuf.VT4.Item3);
            Assert.Equal(valuetuples.VT4.Item4, frombuf.VT4.Item4);

            Assert.Equal(valuetuples.VT5.Item1, frombuf.VT5.Item1);
            Assert.Equal(valuetuples.VT5.Item2, frombuf.VT5.Item2);
            Assert.Equal(valuetuples.VT5.Item3, frombuf.VT5.Item3);
            Assert.Equal(valuetuples.VT5.Item4, frombuf.VT5.Item4);
            Assert.Equal(valuetuples.VT5.Item5, frombuf.VT5.Item5);

            Assert.Equal(valuetuples.VT6.Item1, frombuf.VT6.Item1);
            Assert.Equal(valuetuples.VT6.Item2, frombuf.VT6.Item2);
            Assert.Equal(valuetuples.VT6.Item3, frombuf.VT6.Item3);
            Assert.Equal(valuetuples.VT6.Item4, frombuf.VT6.Item4);
            Assert.Equal(valuetuples.VT6.Item5, frombuf.VT6.Item5);
            Assert.Equal(valuetuples.VT6.Item6, frombuf.VT6.Item6);
        }

        [RePacker]
        public struct StructWithManagedValueTuple
        {
            public ValueTuple<SimpleClass, SimpleClass> VT2;
        }

        [Fact]
        public void struct_with_managed_value_tuple()
        {
            var wanted = new StructWithManagedValueTuple
            {
                VT2 = (new SimpleClass { Float = 1.2345f }, new SimpleClass { Float = 1.2345f })
            };

            var buffer = new BoxedBuffer(1024);

            RePacking.Pack(buffer, ref wanted);

            var fromBuf = RePacking.Unpack<StructWithManagedValueTuple>(buffer);

            Assert.Equal(wanted.VT2.Item1.Float, fromBuf.VT2.Item1.Float);
            Assert.Equal(wanted.VT2.Item2.Float, fromBuf.VT2.Item2.Float);
        }
    }
}
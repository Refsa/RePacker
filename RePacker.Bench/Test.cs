using System;
using System.Reflection;
using Refsa.RePacker;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Builder;

public class TestClass
{
    [RePacker]
    public struct StructWithMarkedProperties
    {
        [RePack]
        public float Float { get; set; }
        [RePack]
        public int Int { get; set; }

        public float Long { get; set; }
    }

    public TestClass()
    {
        var swp = new StructWithMarkedProperties
        {
            Float = 1.245f,
            Int = 1337,
            Long = 123456789
        };

        var buffer = new BoxedBuffer(1024);
        RePacker.Pack(buffer, ref swp);
        // var fromBuf = RePacker.Unpack<StructWithMarkedProperties>(buffer);

        // GetPropertyBackingFieldInfo(typeof(StructWithMarkedProperties), "Float").SetValue(swp, 5f);
        // Console.WriteLine(swp.Float);

        var intstruct = new InternalStruct
        {
            Thing = 1.337f,
        };
        RePacker.Pack(buffer, ref intstruct);

        int val = 10;
        RePacker.Pack<int>(buffer, ref val);
    }

    static FieldInfo GetPropertyBackingFieldInfo(Type target, string propertyName)
    {
        string backingFieldName = $"<{propertyName}>k__BackingField";
        FieldInfo fi = target.GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        return fi;
    }
}

[RePacker]
struct InternalStruct
{
    public float Thing;
}

struct BaseStruct<A, B, C>
{
    public A AValue;
    public B BValue;
    public C CValue;
}

class BaseStructPacker<A, B, C> : RePackerWrapper<BaseStruct<A, B, C>>
{
    public override void Pack(BoxedBuffer buffer, ref BaseStruct<A, B, C> value)
    {
        RePacker.Pack(buffer, ref value.AValue);
        RePacker.Pack(buffer, ref value.BValue);
        RePacker.Pack(buffer, ref value.CValue);
    }
}

class BaseStructPackerProducer : GenericProducer
{
    public override Type ProducerFor => typeof(BaseStruct<,,>);

    public override ITypePacker GetProducer(Type type)
    {
        var producer = Activator
            .CreateInstance(typeof(BaseStructPacker<,,>)
            .MakeGenericType(type.GetGenericArguments()));

        return (ITypePacker)producer;
    }
}
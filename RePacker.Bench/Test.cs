using System;
using System.Reflection;
using Refsa.RePacker;
using Refsa.RePacker.Buffers;

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

        var intstruct = new InternalStruct{
            Thing = 1.337f,
        };
        RePacker.Pack(buffer, ref intstruct);
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
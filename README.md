# RePacker - Flexible and Fast C# Binary Packer

The aim for this project is to create a flexible and fast serializer/binary packer that works with both standalone C# projects and within the Unity .Net ecosystem.

Current Features:
- Single entry serialization of any supported type
- Easily auto-generate serializers, with nested type support
- Wrap types you can't directly modify to support serialization
- Fast Buffer type that can be used by itself to pack and unpack data
- No GC footprint on unmanaged types
- Unpack directly into existing objects
- C style packing without padding, meaning it can be unpacked in languages that supports C style structs

Issues/Downsides:
- No AOT(IL2CPP) support for Unity
- Currently auto-generated packers needs the base type to be public
- Unity reference types nested inside other types can be iffy
- Currently you need to use Buffer and BoxedBuffer that is supplied with this package to Pack and Unpack. It's a wrapper around Memory\<byte\> to more easily optimize the internal workings.
- No proper IEEE handling of floating point types
- Only Big-Endian support for now
  
Long Term Goals:
- Unity AOT(IL2CPP) Support
- Speed
- Flexibility
- Tested cross-language support
- Control over stride/padding

## General Use

Packing and Unpacking requires the use of **BoxedBuffer** that is a class that wraps around the Buffer struct. Buffer internally has a **Memory\<byte\>** that is pointing to a byte array. __You are responsible for pooling the byte array and/or the BoxedBuffer for now.__

#### Auto-Generate packer:
By default it generates for all public fields:
```cs
[RePacker]
public struct SupportMe
{
    public float Float;
    public int Int;
}
```

You can also specify the fields to pack:
```cs
[RePacker(false)]
public struct SupportMe
{
    [RePack] public float Float;
    public int Int;
}
```

#### Pack and Unpack:
```cs
SupportMe packMe = new SupportMe{Float = 1.337f, Int = 1337};
BoxedBuffer buffer = new BoxedBuffer(1024);

RePacker.Pack(buffer, ref packMe);
SupportMe unpacked = RePacker.Unpack<SupportMe>(buffer);
```

#### Pack and Unpack into existing instance
```cs
[RePacker]
public class IntoInstance
{
    public float Float;
}

IntoInstance packMe = new IntoInstance{Float = 13.37f};
BoxedBuffer buffer = new BoxedBuffer(1024);

RePacker.Pack(buffer, ref packMe);

IntoInstance unpackIntoMe = new IntoInstance();
RePacker.UnpackInto(buffer, ref unpackIntoMe);
```

#### Wrap an existing type
```cs
public class CantModifyMe
{
    public float Float;
}

[RePackerWrapper(typeof(CantModifyMe))]
public class CantModifyMeWrapper : RePackerWrapper<CantModifyMe>
{
    public override void Pack(BoxedBuffer buffer, ref CantModifyMe value)
    {
        buffer.Buffer.PushFloat(ref value.Float);
    }

    public override void Unpack(BoxedBuffer buffer, out CantModifyMe value)
    {
        value = new CantModifyMe();
        buffer.Buffer.PopFloat(out value.Float);
    }

    public override void UnpackInto(BoxedBuffer buffer, ref Vector2 value)
    {
        buffer.Buffer.PopFloat(out value.Float);
    }
}

// Pack/Unpack as shown above
```

## Supported Types

### C# Types
Any unmanaged type/struct is automatically supported by the use of MemoryMarshal. 

To allow for faster serialization the following unmanaged types have direct support:

Bool, Char, Byte, SByte, Short, UShort, Int, Uint, Long, ULong, Float, Double, Decimal

Collections:
- ICollection\<T\>
- IList\<T\>
- IEnumerable\<T\>
- Dictionary<TKey, TValue>
- List\<T\>
- Stack\<T\>
- Queue\<T\>
- HashSet\<T\>
- Array

Additionally:
- String
- Enum (the built in generator reflects directly on the underlying type, meaning no overhead)
- DateTime
- TimeSpan
- ValueTuple\<T1, T2, T3, T4, T5, T6, T7, TRest\> (or any of the versions with fewer params)
- KeyValuePair\<TKey, TValue\>
- Nullable\<T\>

**Any generic argument shown above needs to be supported by a packer/unpacker as well**

### Unity Types:
Transform, Color, Color32, Vector2, Vector3, Vector4, Quaternion, Vector3Int, Vector2Int
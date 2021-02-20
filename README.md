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
- Versioning and dirty bits

## General Use
Currently both bootstrapping and logging is enabled by default, but can be toggled with the NO_BOOTSTRAP and NO_LOGGING compiler defines. In .NET the bootstrap is initialized statically, but for Unity this happens with the use of `RuntimeInitializeOnLoad`.

### BoxedBuffer
Packing and Unpacking requires the use of **BoxedBuffer** that is a class that wraps around the Buffer struct. Buffer internally has a **Memory\<byte\>** that is pointing to a byte array. __You are responsible for pooling the byte array and/or the BoxedBuffer for now.__ 

Because of how Buffer works internally it's important that you call `buffer.Reset()` after you're finished unpacking data from it. It stores both a read and a write index internally to facilitate reads and writes.

### Auto-Generate packer:
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

Alternatively, any struct with only unmanaged fields can be directly packed/unpacked. This is all handled internally though, but best practice is probably to have the [RePackerAttribute] present.
```cs
public struct ImUnmanaged
{
    public float Float;
    public int Int;
}
```

### Pack and Unpack:
```cs
SupportMe packMe = new SupportMe{Float = 1.337f, Int = 1337};
BoxedBuffer buffer = new BoxedBuffer(1024);

RePacker.Pack(buffer, ref packMe);
SupportMe unpacked = RePacker.Unpack<SupportMe>(buffer);
```

### Pack and Unpack into existing instance
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

### Wrap an existing type
```cs
public class CantModifyMe
{
    public float Float;
}

[RePackerWrapper(typeof(CantModifyMe))]
public class CantModifyMePacker : RePackerWrapper<CantModifyMe>
{
    public override void Pack(BoxedBuffer buffer, ref CantModifyMe value)
    {
        buffer.PushFloat(ref value.Float);
    }

    public override void Unpack(BoxedBuffer buffer, out CantModifyMe value)
    {
        value = new CantModifyMe();
        buffer.PopFloat(out value.Float);
    }

    public override void UnpackInto(BoxedBuffer buffer, ref Vector2 value)
    {
        buffer.PopFloat(out value.Float);
    }
}

// Pack/Unpack as shown above
```

### Handling generic types
Generic types is a special case that requires some additional work to support. They need an additional class that is responsible for producing a packer/unpacker for a specific set of generic arguments. There is currently no pre-generating for generic types but they only need to run once when the first call to packing/unpacking is done. This is hopefully something that can be reworked in the future to reduce the amount of boilerplate.

```cs
public struct MyGenericType<T1, T2>
{
    public T1 Value1;
    public T2 Value2
}

public class MyGenericTypePacker<T1, T2> : RePackerWrapper<MyGenericType<T1, T2>>
{
    public override void Pack(BoxedBuffer buffer, ref MyGenericType<T1, T2> value)
    {
        RePacker.Pack(buffer, ref value.Value1);
        RePacker.Pack(buffer, ref value.Value2);
    }

    public override void Unpack(BoxedBuffer buffer, out MyGenericType<T1, T2> value)
    {
        value = new MyGenericType<T1, T2>();
        value.Value1 = RePacker.Unpack<T1>(buffer);
        value.Value2 = RePacker.Unpack<T2>(buffer);
    }
}

public class MyGenericTypeProducer : GenericProducer
{
    public override Type ProducerFor => typeof(MyGenericType<,>);

    public override ITypePacker GetProducer(Type type)
    {
        var elementTypes = type.GetGenericArguments();
        var instance = Activator.CreateInstance(typeof(MyGenericTypeWrapper<,>).MakeGenericType(elementTypes));
        return (ITypePacker)instance;
    }
}
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

**Any generic argument shown above needs to be supported by a packer/unpacker as well, but nesting of types is supported**

### Unity Types:
Transform, Color, Color32, Vector2, Vector3, Vector4, Quaternion, Vector3Int, Vector2Int

## Buffer and BoxedBuffer

This library uses a struct known as `Buffer` in order to to read and writes into a byte array. Main reason for this is to avoid having to handle pooling of the arrays internally and rather leave that to the end user. Buffer also has a wrapper class known as BoxedBuffer in order to make it easier to synchronize the state of the Buffer when it's passed around the internals.

You can use the Buffer and BoxedBuffer alone in order to do packing and unpacking without the rest of the framework. These are contained inside the RePacker.Unsafe project. This is also how you implement custom packers for types you might not have control over.

### Buffer Extensions
Buffer itself has access to a bunch of utility methods to pack different types. Some of them have direct support, but any type wrapped by RePacker needs to use the framework itself. 

```md
Push<T> where T : unmanaged
    Supports any unmanaged type by using MemoryManager. This is a bit slower than the more optimized packing methods but can be useful.

All of these methods are packing in Big Endian form
PushBool/PopBool
PushByte/PopByte
PushSByte/PopSByte
PushShort/PopShort
PushUShort/PopUShort
PushInt/PopInt
PushUInt/PopUInt
PushLong/PopLong
PushULong/PopULong

Floating point numbers are directly copied by memory and as such is bound to the form the runtime is using
PushFloat/PopFloat
PushDouble/PopDouble
PushDecimal/PopDecimal

Extensions
PackString/UnpackString 
    UTF8 Encoding
PackDateTime/UnpackDateTime
    Packs the Ticks as a ulong
PackEnum<TEnum>/UnpackEnum<TEnum>
    All unmanaged underlying types are supported
PackBlittableArray/PackBlittableArray
    Uses MemoryMarshal to convert an unmanaged/blittable type into a byte array
EncodeArray/DecodeArray
    Only able to support types that are supported by the RePacker framework
```

### BoxedBuffer Extensions
BoxedBuffer contains a field, Buffer, that points to a Buffer instance. On top of this it has some additional utility extensions.

```md
PackDateTime/UnpackDateTime
PackKeyValuePair/UnpackKeyValuePair
PackValueTuple<T1,T2,...>/UnpackValueTuple<T1,T2,...>

PackArray/UnpackArray

PackIList/UnpackIList
PackIListBlittable/UnpackIListBlittable
    Internall this is unpacked as List
    Blittable version utilizes `stackalloc` meaning it avoids heap allocations

PackIEnumerable/UnpackIEnumerable
PackIEnumerableBlittable/UnpackIEnumerableBlittable
    Internally it supports Stack, Queue and HashSet but defaults to List
    Blittable version utilizes `stackalloc` meaning it avoids heap allocations

PackDictionary/UnpackDictionary
```

## Logging
Package provides an ILogger interface and you can set the logger when initializing it at runtime. A default logger using Console exists, while the Unity version has one for it's Debug logging. You can also turn logging completely off if you want.

## Performance:
Benchmarks are performed on an i5-4670k@4.3GHz and uses similar test format to ZeroFormatter. All benchmark code can be found under the RePacker.Bench project. 

```cs
/* netcoreapp3.1
                              Method |            Mean
------------------------------------ |----------------
       ILGen_SmallObjectSerialize10K |       727.38 us
     ILGen_SmallObjectDeserialize10K |     1,128.23 us
            ILGen_VectorSerialize10K |        96.32 us
          ILGen_VectorDeserialize10K |       155.38 us
               ILGen_IntSerialize10K |        51.53 us
             ILGen_IntDeserialize10K |       103.11 us
                     IntSerialize10K |        21.49 us
                   IntDeserialize10K |        18.28 us
                 PackIntSerialize10K |        21.48 us
               PackIntDeserialize10K |        18.28 us
  ILGen_SmallObjectArraySerialize10K |   691,455.41 us
ILGen_SmallObjectArrayDeserialize10K | 1,089,353.67 us
       ILGen_VectorArraySerialize10K |       677.41 us
     ILGen_VectorArrayDeserialize10K |     1,548.71 us
          ILGen_IntArraySerialize10K |     1,018.70 us
        ILGen_IntArrayDeserialize10K |     1,044.99 us
          ILGen_LargeStringSerialize |   342,457.98 us
        ILGen_LargeStringDeserialize | 2,004,193.04 us
*/

/* net4.6.1 - latest
                              Method |            Mean
------------------------------------ |----------------
       ILGen_SmallObjectSerialize10K |     1,082.88 us
     ILGen_SmallObjectDeserialize10K |     1,396.81 us
            ILGen_VectorSerialize10K |       217.88 us
          ILGen_VectorDeserialize10K |       235.53 us
               ILGen_IntSerialize10K |       105.54 us
             ILGen_IntDeserialize10K |       105.72 us
                     IntSerialize10K |        21.82 us
                   IntDeserialize10K |        17.69 us
                 PackIntSerialize10K |        21.95 us
               PackIntDeserialize10K |        17.67 us
  ILGen_SmallObjectArraySerialize10K | 1,002,928.50 us
ILGen_SmallObjectArrayDeserialize10K | 1,395,526.24 us
       ILGen_VectorArraySerialize10K |       928.49 us
     ILGen_VectorArrayDeserialize10K |     1,580.59 us
          ILGen_IntArraySerialize10K |     1,412.97 us
        ILGen_IntArrayDeserialize10K |     1,006.31 us
          ILGen_LargeStringSerialize | 1,510,192.95 us
        ILGen_LargeStringDeserialize | 3,074,888.27 us
*/
```

### Unity:

For Unity 2048 packs and unpacks of a Transform takes about 3ms on a 4670k @ 4.3GHz.
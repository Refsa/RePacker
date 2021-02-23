# RePacker - Flexible and Fast C# Non-Persistent Binary Packer

The aim for this project is to create a flexible and fast serializer/binary packer that works with both standalone C# projects and within the Unity Engine ecosystem.

What you get:
- Blazingly fast binary packing
- Single entry point to serialization
- Ability to mark specific fields/properties for serialization
- Ability to wrap types outside of your control for serialization
- Supports NET4.6.1 through NET4.8 and Net Core 3.0/3.1

What you dont get:
- Versioning
- No Unity AOT(IL2CPP) Support (for now)
- Endianess is based on platform it runs on (for now)

Long Term Goals:
- Unity AOT(IL2CPP) Support
- Tested cross-language support
- Control over stride/padding and endianess

## How
Using IL generated at runtime and a healthy does of unsafe code we can achieve fast and stable serialization speeds. This is done by sacrificing control over stride and endianess and directly copying what the .NET runtime itself creates. 

From the benchmarks I've done this package provides the fastest way to serialize to a binary format across all supported .NET versions. Even if it is the fastest there are still many packages that provides more features for versioning and smaller binary size if that is a requirement.

You can find benchmarks under the [performance](#performance) section.

## General Use

Currently both bootstrapping and logging is enabled by default, but can be toggled with the NO_BOOTSTRAP and NO_LOGGING compiler defines. In .NET the bootstrap is initialized statically, but for Unity this happens with the use of `RuntimeInitializeOnLoad`.

### BoxedBuffer
Packing and Unpacking requires the use of **BoxedBuffer** that is a class that wraps around the **Buffer** struct. **Buffer** again is a simple wrapper around a byte array and contains most functionality for handling unmanaged data. This means that the user is responsible for pooling the arrays that you work on. More information on this can be found in the [Buffer & BoxedBuffer](#buffer-and-boxedbuffer) section below.

### Auto-Generate packer:

By default it generates for all public fields:

```cs
[RePacker]
public struct SupportMe
{
    public float Float; // Packed
    public int Int; // Packed
    long _long; // Not Packed
}
```

You can also specify the fields to pack:

```cs
[RePacker(false)]
public struct SupportMe
{
    [RePack] public float Float; // Serialized
    public int Int; // Not serialized
}
```

Any properties needs to be explicitly marked for serialization.

```cs
[RePacker]
public struct SupportMe
{
    [RePack] public float Float { get; set; } // Serialized
    public int Int; // Serialized
}
```

Alternatively, any struct with only unmanaged fields can be directly packed/unpacked.

```cs
public struct ImUnmanaged
{
    public float Float;
    public int Int;
}
```

Another benefit is that any struct where all fields are serialized can be directly copied. This means that an array of "ImUnmanaged" structs will be copied with MemoryCopy and as such provide no overhead.

### Pack and Unpack:
As long as you are not using the Untiy version you need to initialize the library with `RePacker.RePacker.Init();` before any of the packing will work.

```cs
SupportMe packMe = new SupportMe{Float = 1.337f, Int = 1337};
BoxedBuffer buffer = new BoxedBuffer(1024);

RePacker.Pack(buffer, ref packMe);
SupportMe unpacked = RePacker.Unpack<SupportMe>(buffer);
```

### Pack and Unpack into existing instance

You can also unpack types that has the "UnpackInto" method defined on them. Any internal type has this defined, but any custom wrappers you make needs this defined

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

Any type outside of your control needs a custom wrapper defined to enable serialization. In this case you can make use of all the internally used packing/unpacking methods for Buffer and BoxedBuffer. An overview of the supported packing helpers can be found in the [Buffer Extensions](#buffer-extensions) and [BoxedBuffer Extensions](#boxedbuffer-extensions) section below.

Additionally any type that has a custom packer can also be packed/unpacked with `RePacker.Pack`/`RePacker.Unpack`.

```cs
public class CantModifyMe
{
    public float Float;
    public SupportMe SupportMe;
}

[RePackerWrapper(typeof(CantModifyMe))]
public class CantModifyMeWrapper : RePackerWrapper<CantModifyMe>
{
    public override void Pack(BoxedBuffer buffer, ref CantModifyMe value)
    {
        buffer.PushFloat(ref value.Float);
        RePacker.Pack(buffer, ref value.SupportMe);
    }

    public override void Unpack(BoxedBuffer buffer, out CantModifyMe value)
    {
        value = new CantModifyMe();
        buffer.PopFloat(out value.Float);
        RePacker.Unpack(buffer, ref value.SupportMe);
    }

    public override void UnpackInto(BoxedBuffer buffer, ref Vector2 value)
    {
        buffer.PopFloat(out value.Float);
        RePacker.Unpack(buffer, ref value.SupportMe);
    }
}
// Pack/Unpack as shown above
```

You can also add the `public static new bool IsCopyable = true;` field to your `RePackerWrapper<T>` to enable direct copying of the type inside of arrays/collections. This is only applicable to unmanaged types, such as primitives or structs with only primitives. If that is not the case the field is ignored.

### Handling generic types

Generic types is a special case that requires some additional work to support. They need an additional class that is responsible for producing a packer/unpacker for a specific set of generic arguments. There is currently no pre-generating for generic types but they only need to run once when the first call to packing/unpacking is done. This is hopefully something that can be reworked in the future to reduce the amount of boilerplate, but is worth knowing if such needs exists.

```cs
public struct MyGenericType<T1, T2> where T1 : unmanaged
{
    public T1 Value1;
    public T2 Value2
}

public class MyGenericTypePacker<T1, T2> : RePackerWrapper<MyGenericType<T1, T2>>
{
    public override void Pack(BoxedBuffer buffer, ref MyGenericType<T1, T2> value)
    {
        buffer.Buffer.Pack<T1>(ref value.Value1);
        RePacker.Pack(buffer, ref value.Value2);
    }

    public override void Unpack(BoxedBuffer buffer, out MyGenericType<T1, T2> value)
    {
        value = new MyGenericType<T1, T2>();
        buffer.Buffer.Unpack<T1>(out value.Value1);
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

Primitives:

```
Bool, Char, Byte, SByte, Short, UShort, Int, Uint, Long, ULong, Float, Double, Decimal
```

Collections:

```
- ICollection<T>
- IList<T>
- IEnumerable<T>
- Dictionary<TKey, TValue>
- List<T>
- Stack<T>
- Queue<T>
- HashSet<T>
- Array (up to rank 4)
```

Additionally:

```
- String
- Enum (the built in generator reflects directly on the underlying type, meaning no overhead)
- DateTime
- TimeSpan
- ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> (or any of the versions with fewer params)
- KeyValuePair<TKey, TValue>
- Nullable<T>
```

### Unity Types:

RePacker.Unity projects contains support needed for Unity types.

```
Transform, Color, Color32, Vector2, Vector3, Vector4, Quaternion, Vector3Int, Vector2Int
```

## Buffer and BoxedBuffer
This library uses a struct known as `Buffer` in order to to read and writes into a byte array. Main reason for this is to avoid having to handle pooling of the arrays internally and rather leave that to the end user. Buffer also has a wrapper class known as BoxedBuffer in order to make it easier to synchronize the state of the Buffer when it's passed around the internals.

You can use the Buffer and BoxedBuffer alone in order to do packing and unpacking without the rest of the framework. These are contained inside the RePacker.Unsafe project. This is also how you implement custom packers for types you might not have control over.

Since no byte array is allocated internally it also means that you need to assign a byte array that can fit the data you want. It will not resize the byte array in order to avoid losing the reference externally and as such requires some more careful planning around use. If you push anything that can't fit it will throw an exception you need to catch. This is mostly a design decision around the specific use case for this library and you could alter it to expand the byte array by using the Buffer::Expand method when appropriate.

### Buffer Extensions

Buffer itself has access to a bunch of utility methods to pack different unmanaged types. This type is only ment for unmanaged data and does not support any managed types. It's a simple wrapper around a byte array and is the core functionality of the entire library.

```
Pack<T> where T : unmanaged
Unpack<T> where T : unmanaged
  Supports any unmanaged type by direct memory copy.

All of these methods are directly copied by memory:
PushBool/PopBool
PushByte/PopByte
PushSByte/PopSByte
PushShort/PopShort
PushUShort/PopUShort
PushInt/PopInt
PushUInt/PopUInt
PushLong/PopLong
PushULong/PopULong
PushFloat/PopFloat
PushDouble/PopDouble
PushDecimal/PopDecimal

MemoryCopyFromUnsafe<T> where T : unmanaged
  Copies an array by memory into buffer
MemoryCopyToUnsafe<T> where T : unmanaged
  Copes an array by memory from buffer

CanFit<T>(int count) where T : unmanaged
  Checks if buffer can fit x amount of T

Extensions:
PackString/UnpackString
  UTF8 Encoding
PackDateTime/UnpackDateTime
  Packs the Ticks as a ulong
PackEnum<TEnum>/UnpackEnum<TEnum>
  All unmanaged underlying types are supported
PackBlittableArray/UnpackBlittableArray
  Any blittable/unmanaged types such as primitives and structs with only primitives
```

### BoxedBuffer Extensions

BoxedBuffer contains a field, Buffer, that points to a Buffer instance. On top of this it has some additional utility extensions. This is the main class for handling managed types.

```
Pack/Unpack - Redirects to RePacker::Pack/RePacker::Unpack
Push/Pop - Redirects to contained buffers Buffer::Pack/Buffer::Unpack

PackKeyValuePair/UnpackKeyValuePair
PackValueTuple<T1,T2,...>/UnpackValueTuple<T1,T2,...>
PackString/UnpackString
PackDateTime/UnpackDateTime
PackTimeSpan/UnpackTimeSpan
PackNullable/UnpackNullable

PackArray/UnpackArray
PackArray2D/UnpackArray2D
PackArray3D/UnpackArray3D
PackArray4D/UnpackArray4D

PackIList/UnpackIList
PackIListBlittable/UnpackIListBlittable - Do not this returns an array as IList

PackIEnumerable/UnpackIEnumerable
PackIEnumerableBlittable/UnpackIEnumerableBlittable

PackICollection/UnpackICollection
PackICollectionBlittable/UnpackICollectionBlittable

PackQueue/UnpackQueue
PackStack/UnpackStack
PackHashSet/UnpackHashSet

PackDictionary/UnpackDictionary
```

## Performance:

Benchmarks are performed on an i5-4670k@4.3GHz with Windows 10. All benchmark code can be found under the RePacker.Bench project.

```cs
// Benches found in ZeroFormatterBench.cs

/* netcoreapp3.1
                              Method |            Mean
------------------------------------ |----------------
       ILGen_SmallObjectSerialize10K |       463.22 us
     ILGen_SmallObjectDeserialize10K |       870.49 us
            ILGen_VectorSerialize10K |       119.37 us
          ILGen_VectorDeserialize10K |       135.90 us
               ILGen_IntSerialize10K |        51.54 us
             ILGen_IntDeserialize10K |        51.49 us
                     IntSerialize10K |        30.93 us
                   IntDeserialize10K |        28.42 us
                 PackIntSerialize10K |        63.23 us
               PackIntDeserialize10K |        30.40 us
  ILGen_SmallObjectArraySerialize10K |   420,579.11 us
ILGen_SmallObjectArrayDeserialize10K |   773,566.65 us
       ILGen_VectorArraySerialize10K |       659.67 us
     ILGen_VectorArrayDeserialize10K |     1,220.17 us
          ILGen_IntArraySerialize10K |       948.97 us
        ILGen_IntArrayDeserialize10K |       654.16 us
          ILGen_LargeStringSerialize |   339,452.53 us
        ILGen_LargeStringDeserialize | 1,996,564.20 us

/* net4.6.1
                              Method |            Mean
------------------------------------ |----------------
       ILGen_SmallObjectSerialize10K |       573.19 us
     ILGen_SmallObjectDeserialize10K |       977.45 us
            ILGen_VectorSerialize10K |       119.48 us
          ILGen_VectorDeserialize10K |       142.76 us
               ILGen_IntSerialize10K |        49.17 us
             ILGen_IntDeserialize10K |        51.57 us
                     IntSerialize10K |        27.43 us
                   IntDeserialize10K |        26.57 us
                 PackIntSerialize10K |        30.79 us
               PackIntDeserialize10K |        28.23 us
  ILGen_SmallObjectArraySerialize10K |   529,452.41 us
ILGen_SmallObjectArrayDeserialize10K |   962,823.00 us
       ILGen_VectorArraySerialize10K |       858.31 us
     ILGen_VectorArrayDeserialize10K |     1,287.13 us
          ILGen_IntArraySerialize10K |     1,389.04 us
        ILGen_IntArrayDeserialize10K |       735.67 us
          ILGen_LargeStringSerialize | 1,506,050.15 us
        ILGen_LargeStringDeserialize | 3,069,118.35 us
*/
```

ZeroFormatter benchmark results can be found [here](RePacker.Bench/ExternalBenchResults/ZeroFormatterBench.md) (NET 4.6.1).  
StructPacker benchmark results can be found [here](RePacker.Bench/ExternalBenchResults/StructPacker.md) (Net Core 3.1).  

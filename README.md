# RePacker - Flexible and Fast C# Non-Persistent Binary Packer

[![.NET](https://github.com/Refsa/RePacker/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Refsa/RePacker/actions/workflows/dotnet.yml)

![Nuget](https://img.shields.io/nuget/v/RePacker?label=nuget%3A%20RePacker)
![Nuget](https://img.shields.io/nuget/v/RePacker.Unsafe?label=nuget%3A%20RePacker.Unsafe)

[Changelog](CHANGELOG.md)

The aim for this project is to create a flexible and fast serializer/binary packer that works with both standalone C# projects and within the Unity Engine ecosystem.

What you get:
- Blazingly fast binary packing
- Single entry point to serialization
- Ability to mark specific fields/properties for serialization
- Ability to wrap types outside of your control for serialization
- Supports NET4.6.1 through NET5.0 and Net Core 3.0/3.1

What you dont get:
- Versioning
- No Unity AOT(IL2CPP) Support (for now)
- Endianess is based on platform it runs on (for now)

Long Term Goals:
- Unity AOT(IL2CPP) Support
- Tested cross-language support
- Control over stride/padding and endianess

## How
Using IL generated at runtime and a healthy does of unsafe code we can achieve fast and stable serialization speeds.

From the benchmarks I've done this package provides the fastest way to serialize to a binary format across all supported .NET versions. Even if it is the fastest there are still many packages that provides more features for versioning and smaller binary size if that is a requirement.

You can find benchmarks under the [performance](#performance) section.

## Where
Library is hosted on [nuget](https://www.nuget.org/packages/RePacker/).

You can also find the Unsafe portion, with just the ReBuffer and utilities at [nuget](https://www.nuget.org/packages/RePacker.Unsafe/), but these are included in the main package.

You can also directly build the project under "RePackage" with `dotnet build -c Release` or "RePackage.Unity" with `dotnet build -c Unity`.

## General Use

Currently both bootstrapping is enabled by default, but can be toggled with the NO_BOOTSTRAP compiler defines. In .NET the bootstrap is initialized statically, but for Unity this happens with the use of `RuntimeInitializeOnLoad`.

### Endianness
Endianness is supported, but will default to the CLR endianness from BitConverter.IsLittleEndian. You can specify endianness on the buffer itself with `ReBuffer::SetEndianness` using the `Endianness` enum. You can also specify a default with RePackerSettings if you run your own bootstrap, or by calling `RePacking::SetDefaultEndianness`.

### ReBuffer
This library primarily works on the ReBuffer class that is a wrapper around a byte array with some additonal utility for reading/writing data. By default the buffer doesnt grow in size to accomodate data, but there is support for this as well. Internally no pooling is done so this is something that has to be handled externally for now.

### Auto-Generate packer:

By default it generates for all public fields:

```cs
[RePacker]
public struct SupportMe
{
    public float Float; // Packed
    [RePack] float _float; // Packed
    long _long; // Not Packed
    public short Short {get; set;} // Not Packed
}
```

You can also specify the fields to pack:

```cs
[RePacker(false)]
public struct SupportMe
{
    [RePack] public float Float; // Packed
    public int Int; // Not Packed
    public short Short {get; set;} // Not Packed
}
```

Any properties needs to be explicitly marked for serialization.

```cs
[RePacker]
public struct SupportMe
{
    [RePack] public float Float { get; set; } // Packed
    [RePack] double Double { get; set; } // Packed
    public int Int; // Packed
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

You should add a `[StructLayout(LayoutKind.Sequential, Pack = 1)]` over your untagged and unmanaged struct to control the packing of those structs. Directly copying these by memory means that any data type inside is expanded to fit the largest one. i.e. a struct with a long and a short will give a size of 16 instead of 10 if you dont have the attribute on it. `Pack = 1` is what this library packs with internally too, so it's good to keep it consistent that way.

### Pack and Unpack:
As long as the NO_BOOTSTRAP flag is set you need to initialize the library with `RePacker.RePacker.Init();` before any of the packing will work.

```cs
SupportMe packMe = new SupportMe{Float = 1.337f, Int = 1337};
ReBuffer buffer = new ReBuffer(1024);

RePacking.Pack(buffer, ref packMe);
SupportMe unpacked = RePacking.Unpack<SupportMe>(buffer);
```

### Pack with auto expanding buffer
You can also choose to pack by fitting the size of the buffer to the exact size of the data you are pushing into it. Be aware that this incurs some performance cost as we need to calculate size and resize the internal byte array when new data is pushed into it.

```cs
SupportMe packMe = new SupportMe{Float = 1.337f, Int = 1337};

var buffer = RePacking.Pack(ref packMe);
SupportMe unpacked = RePacking.Unpack<SupportMe>(buffer);
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
ReBuffer buffer = new ReBuffer(1024);

RePacking.Pack(buffer, ref packMe);

IntoInstance unpackIntoMe = new IntoInstance();
RePacking.UnpackInto(buffer, ref unpackIntoMe);
```

### Wrap an existing type

Any type outside of your control needs a custom wrapper defined to enable serialization. In this case you can make use of all the internally used packing/unpacking methods for ReBuffer. An overview of the supported packing helpers can be found in the [ReBuffer Extensions](#buffer-extensions) section below.

When a type is wrapped this way you can continue to use `RePacking.Pack/RePacking.Unpack` as normal. Do note that you need to implement the `SizeOf` method in order to make use of `RePacking.SizeOf` on this object.

```cs
public class CantModifyMe
{
    public float Float;
}

[RePackerWrapper(typeof(CantModifyMe))]
public class CantModifyMeWrapper : RePackerWrapper<CantModifyMe>
{
    public override void Pack(ReBuffer buffer, ref CantModifyMe value)
    {
        buffer.Pack(ref value.Float);
    }

    public override void Unpack(ReBuffer buffer, out CantModifyMe value)
    {
        value = new CantModifyMe();
        buffer.Unpack(out value.Float);
    }

    public override void UnpackInto(ReBuffer buffer, ref CantModifyMe value)
    {
        buffer.Pack(out value.Float);
    }

    public override int SizeOf(ref CantModifyMe value)
    {
        return 4;
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
    public T2 Value2;
}

public class MyGenericTypePacker<T1, T2> : RePackerWrapper<MyGenericType<T1, T2>> where T1 : unmanaged
{
    public override void Pack(ReBuffer buffer, ref MyGenericType<T1, T2> value)
    {
        buffer.Pack<T1>(ref value.Value1);
        RePacking.Pack(buffer, ref value.Value2);
    }

    public override void Unpack(ReBuffer buffer, out MyGenericType<T1, T2> value)
    {
        value = new MyGenericType<T1, T2>();
        buffer.Unpack<T1>(out value.Value1);
        value.Value2 = RePacking.Unpack<T2>(buffer);
    }

    public override int SizeOf(ref MyGenericType<T1, T2> value)
    {
        return RePacking.SizeOf(ref value.Value1) + RePacking.SizeOf(ref value.Value2);
    }
}

public class MyGenericTypeProducer : GenericProducer
{
    public override Type ProducerFor => typeof(MyGenericType<,>);

    public override ITypePacker GetProducer(Type type)
    {
        var elementTypes = type.GetGenericArguments();
        var instance = Activator.CreateInstance(typeof(MyGenericTypePacker<,>).MakeGenericType(elementTypes));
        return (ITypePacker)instance;
    }
}
```

### Getting size of an object
You can also check/calculate the size of any supported/unmanaged type via `RePacking.SizeOf`. This requires an instance of the object you want to check as managed types dont have a constant size. Currently this computation can be quite expensive, but is well worth it.

```cs
SupportMe packMe = new SupportMe{Float = 1.337f, Int = 1337};

int size = RePacking.SizeOf(ref packMe);
```

### Modifying a value in the buffer
To modify a value that exists in the buffer a method exists to get a reference to it. `ReBuffer::GetRef<T>()` will return a reference value that you can then modify, without having to repack the value into the buffer. Do note that this is only possible for unmanage value types such as primitives and structs of primitives.  

Additionally you need to use the specific syntax below in order to interface with the reference value.

```cs
ref int value = ref buffer.GetRef<int>();
value *= 10;
// Value in buffer is now X*10

// You can also give an offset in bytes
ref int value = ref buffer.GetRef<int>(8);
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
  Although it supports dimensionality up to rank 4 it's much more optimal to split it into 4 seperate arrays.
  Only 1 dimensional arrays support direct copying
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
// Components - these needs to be used with UnpackInto to work
Transform, Rigidbody, Rigidbody2D   

// These can be directly packed into the buffer, but using RePacking.Pack/Unpack is more stable
Vector2, Vector3, Vector4, Vector3Int, Vector2Int, Quaternion, Matrix4x4
RectInt, Rect, BoundsInt, Bounds
Color, Color32
```

## ReBuffer
This library uses a struct known as `ReBuffer` in order to to read and writes into a byte array. Main reason for this is to avoid having to handle pooling of the arrays internally and rather leave that to the end user.

You can use the ReBuffer alone in order to do packing and unpacking without the rest of the framework. These are contained inside the RePacker.Unsafe project under the RePacker.Buffers namespace. This is also how you implement custom packers for types you might not have control over.

Two modes exists for the ReBuffer class, either static or dynamic. The default mode is static, which means that you need to ensure it has enough capacity to fit the object you want to pack. If you try to write an object that is too large it will throw an exception and unwind to the position before the object was pushed. Utilities exists to calculate the size of supported objects though, so you can strategize around this.

In dynamic mode it will first check the size of the object you push and then resize the internal byte array if it needs to. This might be the easier mode to make use of but there is currently no pooling done internally. This means that you might end up with a lot of GC pressure if you push a lot of smaller values over time. 

### ReBuffer Extensions

ReBuffer itself has access to a bunch of utility methods to pack different types. All unmanaged types are directly supported, but any unmanaged types requires support through `RePacking.Pack/RePacking.Unpack`

```
Pack<T> where T : unmanaged
Unpack<T> where T : unmanaged
  Supports any unmanaged type by direct memory copy.

All of these methods are directly copied by memory:
PackBool/UnpackBool
PackByte/UnpackByte
PackSByte/UnpackSByte
PackShort/UnpackShort
PackUShort/UnpackUShort
PackInt/UnpackInt
PackUInt/UnpackUInt
PackLong/UnpackLong
PackULong/UnpackULong
PackFloat/UnpackFloat
PackDouble/UnpackDouble
PackDecimal/UnpackDecimal

PackArray<T> where T : unmanaged
  Copies an array by memory into buffer
UnpackArray<T> where T : unmanaged
  Copes an array by memory from buffer

CanWrite<T>(int count) where T : unmanaged
  Checks if buffer can fit x amount of T
CanRead<T>(int count) where T : unmanaged
  Checks if buffer can read x amount of T

Extensions:
PackString/UnpackString
  UTF8 Encoding
PackDateTime/UnpackDateTime
  Packs the Ticks as a ulong
PackEnum<TEnum>/UnpackEnum<TEnum>
  All unmanaged underlying types are supported
PackBlittableArray/UnpackBlittableArray
  Any blittable/unmanaged types such as primitives and structs with only primitives

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
// Benches found in ZeroFormatterBench.cs + some additional tests

/* netcoreapp3.1
                                 Method |            Mean
--------------------------------------- |----------------
          ILGen_SmallObjectSerialize10K |       712.57 us
        ILGen_SmallObjectDeserialize10K |       892.95 us
     ILGen_Auto_SmallObjectSerialize10K |     1,175.49 us
               ILGen_VectorSerialize10K |       164.57 us
             ILGen_VectorDeserialize10K |       159.60 us
          ILGen_Auto_VectorSerialize10K |       301.68 us
                  ILGen_IntSerialize10K |        69.69 us
                ILGen_IntDeserialize10K |        62.62 us
             ILGen_Auto_IntSerialize10K |       178.50 us
                    PackIntSerialize10K |        49.13 us
                  PackIntDeserialize10K |        35.61 us
     ILGen_SmallObjectArraySerialize10K |   652,137.50 us
   ILGen_SmallObjectArrayDeserialize10K |   856,358.45 us
ILGen_Auto_SmallObjectArraySerialize10K | 1,075,709.22 us
          ILGen_VectorArraySerialize10K |       597.87 us
        ILGen_VectorArrayDeserialize10K |     1,037.95 us
     ILGen_Auto_VectorArraySerialize10K |     1,169.85 us
             ILGen_IntArraySerialize10K |       963.48 us
           ILGen_IntArrayDeserialize10K |       452.13 us
        ILGen_Auto_IntArraySerialize10K |     2,907.45 us
             ILGen_LargeStringSerialize |   771,316.78 us
           ILGen_LargeStringDeserialize | 2,728,520.45 us
        ILGen_Auto_LargeStringSerialize | 2,290,445.83 us
*/

/* net4.6.1
                                 Method |            Mean
--------------------------------------- |----------------
          ILGen_SmallObjectSerialize10K |       610.88 us
        ILGen_SmallObjectDeserialize10K |     1,043.60 us
     ILGen_Auto_SmallObjectSerialize10K |     1,411.37 us
               ILGen_VectorSerialize10K |       145.52 us
             ILGen_VectorDeserialize10K |       159.96 us
          ILGen_Auto_VectorSerialize10K |       276.04 us
                  ILGen_IntSerialize10K |        62.07 us
                ILGen_IntDeserialize10K |        56.58 us
             ILGen_Auto_IntSerialize10K |       165.58 us
                    PackIntSerialize10K |        34.85 us
                  PackIntDeserialize10K |        30.32 us
     ILGen_SmallObjectArraySerialize10K |   547,066.33 us
   ILGen_SmallObjectArrayDeserialize10K |   948,959.85 us
ILGen_Auto_SmallObjectArraySerialize10K | 1,385,727.33 us
          ILGen_VectorArraySerialize10K |       375.05 us
        ILGen_VectorArrayDeserialize10K |       987.88 us
     ILGen_Auto_VectorArraySerialize10K |     1,214.24 us
             ILGen_IntArraySerialize10K |       717.80 us
           ILGen_IntArrayDeserialize10K |       482.71 us
        ILGen_Auto_IntArraySerialize10K |     2,929.60 us
             ILGen_LargeStringSerialize | 1,518,598.64 us
           ILGen_LargeStringDeserialize | 3,454,324.89 us
        ILGen_Auto_LargeStringSerialize | 3,887,686.05 us

                                 Method |            Mean
--------------------------------------- |----------------
          ILGen_SmallObjectSerialize10K |       892.65 us
        ILGen_SmallObjectDeserialize10K |     1,008.63 us
     ILGen_Auto_SmallObjectSerialize10K |     1,433.67 us
               ILGen_VectorSerialize10K |       161.80 us
             ILGen_VectorDeserialize10K |       171.16 us
          ILGen_Auto_VectorSerialize10K |       294.30 us
                  ILGen_IntSerialize10K |        63.43 us
                ILGen_IntDeserialize10K |        60.31 us
             ILGen_Auto_IntSerialize10K |       167.86 us
                    PackIntSerialize10K |        38.17 us
                  PackIntDeserialize10K |        34.75 us
     ILGen_SmallObjectArraySerialize10K |   827,117.35 us
   ILGen_SmallObjectArrayDeserialize10K |   973,007.00 us
ILGen_Auto_SmallObjectArraySerialize10K | 1,368,944.08 us
          ILGen_VectorArraySerialize10K |       392.66 us
        ILGen_VectorArrayDeserialize10K |       984.11 us
     ILGen_Auto_VectorArraySerialize10K |     1,193.45 us
             ILGen_IntArraySerialize10K |       772.44 us
           ILGen_IntArrayDeserialize10K |       483.35 us
        ILGen_Auto_IntArraySerialize10K |     2,904.86 us
             ILGen_LargeStringSerialize | 3,049,761.95 us
           ILGen_LargeStringDeserialize | 3,644,474.06 us
        ILGen_Auto_LargeStringSerialize | 5,427,890.90 us
*/
```

ZeroFormatter benchmark results can be found [here](RePacker.Bench/ExternalBenchResults/ZeroFormatterBench.md) (NET 4.6.1).  
StructPacker benchmark results can be found [here](RePacker.Bench/ExternalBenchResults/StructPacker.md) (Net Core 3.1).  

## Building and Testing Unity

### Building
Building the Unity project requires the UnityEngine.dll. This is not included with this project and as such you need to provide that yourself.  

The DLL should be placed under "RePacker.Unity/UnityDlls/" and can be found in "[unity install directory]\Editor\Data\Managed\".

### Testing
A Unity project exists under RePacker.Unity.Tests to run tests within the Unity environment.

Tests can be ran with the `run.sh` bash script in that folder, it will default to `C:\Program Files\Unity\Hub\Editor\2019.4.22f1\Editor\Unity.exe` as the Unity version to use.  
To run with another version/location you can use the `-c` flag when running the script, supplying the full `Unity.exe` path.
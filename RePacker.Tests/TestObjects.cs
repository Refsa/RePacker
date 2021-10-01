using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RePacker.Tests
{
    [RePacker]
    public struct TestStruct
    {
        public bool Bool;
        // public char Char;
        public sbyte Sbyte;
        public byte Byte;
        public short Short;
        public ushort Ushort;
        public int Int;
        public uint Uint;
        public long Long;
        public ulong Ulong;
        public float Float;
        public double Double;
        public decimal Decimal;
    }

    [RePacker]
    public class TestClass
    {
        public bool Bool;
        // public char Char;
        public sbyte Sbyte;
        public byte Byte;
        public short Short;
        public ushort Ushort;
        public int Int;
        public uint Uint;
        public long Long;
        public ulong Ulong;
        public float Float;
        public double Double;
        public decimal Decimal;
    }

    [RePacker]
    public struct StructWithString
    {
        public float Float;
        public string String1;
        public string String2;
        public int Int;
    }

    [RePacker]
    public class ClassWithString
    {
        public float Float;
        public string String1;
        public string String2;
        public int Int;
    }

    public enum ByteEnum : byte
    {
        One = 1,
        Ten = 10,
        Hundred = 100,
    }

    public enum LongEnum : long
    {
        Low = -123456789,
        High = 987654321,
    }

    [RePacker]
    public struct StructWithEnum
    {
        public float Float;
        public ByteEnum ByteEnum;
        public LongEnum LongEnum;
        public int Int;
    }

    [RePacker]
    public struct ClassWithEnum
    {
        public float Float;
        public ByteEnum ByteEnum;
        public LongEnum LongEnum;
        public int Int;
    }

    [RePacker]
    public class SimpleClass
    {
        public float Float;
    }

    [RePacker]
    public class SimpleStruct
    {
        public float Float;
    }

    public enum Sex : sbyte
    {
        Unknown, Male, Female,
    }

    [RePacker]
    public class Person
    {
        public int Age;
        public string FirstName;
        public string LastName;
        public Sex Sex;
    }

    [RePacker]
    public struct ParentWithNestedStruct
    {
        public float Float;
        public ChildStruct Child;
        public ulong ULong;
    }

    [RePacker]
    public struct ChildStruct
    {
        public float Float;
        public byte Byte;
    }

    [RePacker]
    public struct ParentWithNestedClass
    {
        public float Float;
        public ChildClass Child;
        public ulong ULong;
    }

    [RePacker]
    public class ChildClass
    {
        public float Float;
        public byte Byte;
    }

    [RePacker]
    public struct RootType
    {
        public float Float;
        public UnmanagedStruct UnmanagedStruct;
        public double Double;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UnmanagedStruct
    {
        public int Int;
        public ulong ULong;
    }

    [RePacker]
    public struct HasUnsupportedField
    {
        public int Int;
        public Type Type;
        public float Float;
    }

    [RePacker]
    public struct StructWithUnmanagedArray
    {
        public int Int;
        public float[] Floats;
        public long Long;
    }

    [RePacker]
    public struct StructWithBlittableArray
    {
        public int Int;
        public ChildStruct[] Blittable;
        public long Long;
    }

    [RePacker]
    public class InArrayClass
    {
        public float Float;
        public int Int;
    }

    [RePacker]
    public struct HasClassArray
    {
        public long Long;
        public InArrayClass[] ArrayOfClass;
        public byte Byte;
    }

    [RePacker]
    public struct HasStructArray
    {
        public long Long;
        public ChildStruct[] ArrayOfStruct;
        public byte Byte;
    }

    [RePacker]
    public struct Has2DArray
    {
        public long Long;
        public int[,] TwoDimArray;
        public int Int;
    }

    [RePacker]
    public struct Has3DArray
    {
        public long Long;
        public int[,,] ThreeDimArray;
        public int Int;
    }

    [RePacker]
    public struct Has4DArray
    {
        public long Long;
        public int[,,,] FourDimArray;
        public int Int;
    }

    [RePacker]
    public struct HasUnmanagedIList
    {
        public float Float;
        public IList<int> Ints;
        public double Double;
    }

    [RePacker]
    public struct HasUnmanagedList
    {
        public float Float;
        public List<int> Ints;
        public double Double;
    }

    [RePacker]
    public struct HasUnmanagedQueue
    {
        public float Float;
        public Queue<int> Ints;
        public double Double;
    }

    [RePacker]
    public struct HasUnmanagedStack
    {
        public float Float;
        public Stack<int> Ints;
        public double Double;
    }

    [RePacker]
    public struct HasUnmanagedHashSet
    {
        public float Float;
        public HashSet<int> Ints;
        public double Double;
    }

    [RePacker]
    public class SomeManagedObject
    {
        public float Float;
        public int Int;
    }

    [RePacker]
    public struct HasManagedIList
    {
        public float Float;
        public IList<SomeManagedObject> Ints;
        public double Double;
    }

    [RePacker]
    public struct HasManagedList
    {
        public float Float;
        public List<SomeManagedObject> Ints;
        public double Double;
    }

    [RePacker]
    public struct HasManagedQueue
    {
        public float Float;
        public Queue<SomeManagedObject> Ints;
        public double Double;
    }

    [RePacker]
    public struct HasManagedStack
    {
        public float Float;
        public Stack<SomeManagedObject> Ints;
        public double Double;
    }

    [RePacker]
    public struct HasManagedHashSet
    {
        public float Float;
        public HashSet<SomeManagedObject> Ints;
        public double Double;
    }

    [RePacker]
    public struct HasUnmanagedDictionary
    {
        public float Float;
        public Dictionary<int, float> Dict;
        public long Long;
    }

    [RePacker]
    public struct HasManagedKeyUnmanagedValueDict
    {
        public float Float;
        public Dictionary<SomeManagedObject, float> Dict;
        public long Long;
    }

    [RePacker]
    public struct HasUnmanagedKeyManagedValueDict
    {
        public float Float;
        public Dictionary<int, SomeManagedObject> Dict;
        public long Long;
    }

    [RePacker]
    public struct HasManagedKeyManagedValueDict
    {
        public float Float;
        public Dictionary<SomeManagedObject, SomeManagedObject> Dict;
        public long Long;
    }

    [RePacker]
    public struct HasDateTime
    {
        public float Float;
        public DateTime DateTime;
    }

    [RePacker]
    public struct HasTimespan : IEquatable<HasTimespan>
    {
        public float Float;
        public TimeSpan TimeSpan;

        public bool Equals(HasTimespan other)
        {
            return Float == other.Float && TimeSpan == other.TimeSpan;
        }
    }

    [RePacker]
    public struct HasString
    {
        public float Float;
        public string String;
    }

    [RePacker]
    public struct StructWithKeyValuePair
    {
        public float Float;
        public KeyValuePair<int, int> KeyValuePair;
    }

    [RePacker(false)]
    public struct OnlyPackSelectedFields
    {
        [RePack] public float PackFloat;
        public int DontPackInt;
        [RePack] public long PackLong;
    }

    [RePacker]
    public struct StructWithUnmarkedProperties
    {
        public float Float { get; set; }
        public float Int { get; set; }
    }

    [RePacker]
    public struct StructWithMarkedProperties
    {
        [RePack] public float Float { get; set; }
        [RePack] public int Int { get; set; }

        public float Long { get; set; }
    }

    [RePacker]
    public struct StructWithMarkedPrivateField : IEquatable<StructWithMarkedPrivateField>
    {
        public float Float;
        [RePack] int _int;
        public long Long;

        public StructWithMarkedPrivateField(float f, int i, long l)
        {
            Float = f;
            _int = i;
            Long = l;
        }

        public bool Equals(StructWithMarkedPrivateField other)
        {
            return Float == other.Float && _int == other._int && Long == other.Long;
        }
    }

    [RePacker]
    public struct StructWithPrivateField
    {
        public float Float;
        int _int;
        public long Long;

        public StructWithPrivateField(float f, int i, long l)
        {
            Float = f;
            _int = i;
            Long = l;
        }

        public bool Equals(StructWithPrivateField other)
        {
            return Float == other.Float && _int == other._int && Long == other.Long;
        }
    }

    internal class IHavePrivateType
    {
        [RePacker]
        internal struct IAmPrivate
        {
            public float Float;
            public int Int;
        }
    }

    [RePacker]
    public struct HasNullable
    {
        public float? Float;
        public int? Int;
        public bool? Bool;
    }

    [RePacker]
    public sealed class SealedClass
    {
        public int Int;
        public float Float;
        [RePack]
        public NestedSealedClass Class { get; set; }
    }

    [RePacker(false)]
    public sealed class NestedSealedClass
    {
        [RePack]
        public long Long { get; set; }
        [RePack]
        public double Double { get; set; }
    }

    [RePacker]
    internal sealed class InternalSealedClass
    {
        public long Long;
        public float Float;
    }

    [RePacker]
    public struct StructHasBuffer
    {
        public RePacker.Buffers.ReBuffer Buffer;
    }

    [RePacker]
    public class ClassHasBuffer
    {
        public RePacker.Buffers.ReBuffer Buffer;
    }

    public interface ITestInterface { }
    [RePacker]
    public class ClassHasInterface : ITestInterface
    {
        public int Int;
        public float Float;
    }

    [RePacker]
    public class ClassWithInterface
    {
        public ITestInterface Interface;
    }

    [RePacker]
    public struct HasValueTuple1
    {
        public ValueTuple<int> VT;
    }

    [RePacker]
    public struct HasValueTuple2
    {
        public ValueTuple<int, int> VT;
    }

    [RePacker]
    public struct HasValueTuple3
    {
        public ValueTuple<int, int, int> VT;
    }

    [RePacker]
    public struct HasValueTuple4
    {
        public ValueTuple<int, int, int, int> VT;
    }

    [RePacker]
    public struct HasValueTuple5
    {
        public ValueTuple<int, int, int, int, int> VT;
    }

    [RePacker]
    public struct HasValueTuple6
    {
        public ValueTuple<int, int, int, int, int, int> VT;
    }

    [RePacker]
    public struct HasValueTuple7
    {
        public ValueTuple<int, int, int, int, int, int, int> VT;
    }

    [RePacker]
    public struct HasValueTuple8
    {
        public ValueTuple<int, int, int, int, int, int, int, ValueTuple<int>> VT;
    }

#if NETCOREAPP3_0 || NETCOREAPP3_1
#nullable enable

    [RePacker(false)]
    public struct HasNullableList
    {
        [RePack]
        public float Float;
        [RePack]
        public List<float>? Floats {get; set;}
    }
#endif
}
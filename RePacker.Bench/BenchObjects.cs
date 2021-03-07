using System;
using System.Collections.Generic;
using RePacker.Buffers;
using RePacker.Builder;

using ReBuffer = RePacker.Buffers.ReBuffer;

namespace RePacker.Benchmarks
{
    [RePacker]
    public struct TestStruct
    {
        public float Value1;
        public int Value2;
        public byte Value3;
    }

    [RePacker]
    public struct TestStruct2
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

        public override string ToString()
        {
            string val = "TestStruct2 {";

            foreach (var field in typeof(TestStruct2).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                val += $"{field.Name}: {field.GetValue(this)}, ";
            }

            val += "}";

            return val;
        }
    }

    [RePacker]
    public class TestClass2
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

        public override string ToString()
        {
            string val = "TestClass2 {";

            foreach (var field in typeof(TestClass2).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                val += $"{field.Name}: {field.GetValue(this)}, ";
            }

            val += "}";

            return val;
        }
    }

    [RePacker]
    public struct StructWithString
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
    public struct Parent
    {
        public float Float;
        public Child Child;
        public ulong ULong;
    }

    [RePacker]
    public struct Child
    {
        public double Float;
        public byte Byte;
    }

    [RePacker]
    public struct RootType
    {
        public float Float;
        public UnmanagedStruct UnmanagedStruct;
        public double Double;
    }

    public struct UnmanagedStruct
    {
        public int Int;
        public ulong ULong;

        public override string ToString()
        {
            return $"UnmanagedStruct {{ Int: {Int}, ULong: {ULong} }}";
        }
    }

    [RePacker]
    public struct UnmanagedStruct2
    {
        public int Int;
        public ulong ULong;
    }

    [RePacker]
    public struct StructWithArray
    {
        public int Int;
        public float[] Floats;
        public long Long;
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
    public struct HasUnmanagedDictionary
    {
        public float Float;
        public Dictionary<int, float> Dict;
        public long Long;
    }

    [RePacker]
    public struct HasDateTime
    {
        public float Float;
        public DateTime DateTime;
    }

    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;
    }

    public class Transform
    {
        Vector3 position;
        Vector3 rotation;
        Vector3 scale;

        public Vector3 Position { get => position; set => position = value; }
        public Vector3 Rotation { get => rotation; set => rotation = value; }
        public Vector3 Scale { get => scale; set => scale = value; }
    }

    [RePackerWrapper(typeof(Vector3))]
    public class Vector3Packer : RePackerWrapper<Vector3>
    {
        public override void Pack(ReBuffer buffer, ref Vector3 value)
        {
            buffer.Pack(ref value.X);
            buffer.Pack(ref value.Y);
            buffer.Pack(ref value.Z);
        }

        public override void Unpack(ReBuffer buffer, out Vector3 value)
        {
            value = new Vector3();
            UnpackInto(buffer, ref value);
        }

        public override void UnpackInto(ReBuffer buffer, ref Vector3 value)
        {
            buffer.Unpack(out value.X);
            buffer.Unpack(out value.Y);
            buffer.Unpack(out value.Z);
        }
    }

    [RePackerWrapper(typeof(Transform))]
    public class TransformPacker : RePackerWrapper<Transform>
    {
        public override void Pack(ReBuffer buffer, ref Transform value)
        {
            Vector3 pos = value.Position;
            RePacking.Pack<Vector3>(buffer, ref pos);

            Vector3 rot = value.Rotation;
            RePacking.Pack<Vector3>(buffer, ref rot);

            Vector3 scale = value.Scale;
            RePacking.Pack<Vector3>(buffer, ref scale);
        }

        public override void Unpack(ReBuffer buffer, out Transform value)
        {
            throw new NotImplementedException();
        }

        public override void UnpackInto(ReBuffer buffer, ref Transform value)
        {
            value.Position = RePacking.Unpack<Vector3>(buffer);
            value.Rotation = RePacking.Unpack<Vector3>(buffer);
            value.Scale = RePacking.Unpack<Vector3>(buffer);
        }
    }

    [RePacker]
    public struct StructWithKeyValuePair
    {
        public float Float;
        public KeyValuePair<int, int> KeyValuePair;
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
}
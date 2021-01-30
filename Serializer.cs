using System;
using System.Runtime.InteropServices;
using Refsa.Repacker.Buffers;

using Buffer = Refsa.Repacker.Buffers.Buffer;
using ReadOnlyBuffer = Refsa.Repacker.Buffers.ReadOnlyBuffer;

namespace Refsa.Repacker
{
    public static class Serializer
    {
        public static void Encode(this ref Buffer buffer, ISerializer target)
        {
            target.ToBuffer(ref buffer);
        }

        public static object Decode(ref ReadOnlyBuffer buffer, Type type)
        {
            ISerializer instance = Activator.CreateInstance(type) as ISerializer;

            instance.FromBuffer(ref buffer);

            return instance;
        }

        public static void EncodeString(this ref Buffer buffer, ref string str)
        {
            ulong length = (ulong)str.Length;
            buffer.Push<ulong>(ref length);

            var bytes = System.Text.Encoding.UTF8.GetBytes(str);
            buffer.Write(new ReadOnlySpan<byte>(bytes));
        }

        public static string DecodeString(this ref ReadOnlyBuffer buffer)
        {
            buffer.Pop<ulong>(out ulong length);
            int start = buffer.Cursor();

            if (MemoryMarshal.TryGetArray(buffer.Read((int)length), out var seg))
            {
                return System.Text.Encoding.UTF8.GetString(seg.Array, start, (int)length);
            }

            return "";
        }

        public static void EncodeEnum<TEnum>(this ref Buffer buffer, ref TEnum target) where TEnum : unmanaged, Enum
        {
            TypeCode enumType = System.Type.GetTypeCode(System.Enum.GetUnderlyingType(typeof(TEnum)));

            switch (enumType)
            {
                case TypeCode.Byte:
                    byte bval = (byte)Convert.ChangeType(target, enumType);
                    buffer.Push<byte>(ref bval);
                    break;
                case TypeCode.Int16:
                    short i16 = (short)Convert.ChangeType(target, enumType);
                    buffer.Push<short>(ref i16);
                    break;
                case TypeCode.Int32:
                    int i32 = (int)Convert.ChangeType(target, enumType);
                    buffer.Push<int>(ref i32);
                    break;
                case TypeCode.Int64:
                    long i64 = (long)Convert.ChangeType(target, enumType);
                    buffer.Push<long>(ref i64);
                    break;
                case TypeCode.UInt16:
                    ushort u16 = (ushort)Convert.ChangeType(target, enumType);
                    buffer.Push<ushort>(ref u16);
                    break;
                case TypeCode.UInt32:
                    uint u32 = (uint)Convert.ChangeType(target, enumType);
                    buffer.Push<uint>(ref u32);
                    break;
                case TypeCode.UInt64:
                    ulong u64 = (ulong)Convert.ChangeType(target, enumType);
                    buffer.Push<ulong>(ref u64);
                    break;
            }
        }

        public static TEnum DecodeEnum<TEnum>(this ref ReadOnlyBuffer buffer) where TEnum : unmanaged, Enum
        {
            TypeCode enumType = System.Type.GetTypeCode(System.Enum.GetUnderlyingType(typeof(TEnum)));

            switch (enumType)
            {
                case TypeCode.Byte:
                    buffer.Pop<byte>(out byte bval);
                    return (TEnum)(object)(bval);
                case TypeCode.Int16:
                    buffer.Pop<short>(out short i16);
                    return (TEnum)(object)(i16);
                case TypeCode.Int32:
                    buffer.Pop<int>(out int i32);
                    return (TEnum)(object)(i32);
                case TypeCode.Int64:
                    buffer.Pop<long>(out long i64);
                    return (TEnum)(object)(i64);
                case TypeCode.UInt16:
                    buffer.Pop<ushort>(out ushort u16);
                    return (TEnum)(object)(u16);
                case TypeCode.UInt32:
                    buffer.Pop<uint>(out uint u32);
                    return (TEnum)(object)(u32);
                case TypeCode.UInt64:
                    buffer.Pop<ulong>(out ulong u64);
                    return (TEnum)(object)(u64);
            }

            return default(TEnum);
        }

        public static void EncodeBlittableArray<T>(this ref Buffer buffer, T[] data) where T : unmanaged
        {
            ulong dataLen = (ulong)(data.Length * Marshal.SizeOf<T>());
            buffer.Push<ulong>(ref dataLen);
            for (int i = 0; i < data.Length; i++)
            {
                buffer.Push<T>(ref data[i]);
            }
        }

        public static T[] DecodeBlittableArray<T>(this ref ReadOnlyBuffer buffer) where T : unmanaged
        {
            buffer.Pop<ulong>(out ulong length);

            var span = MemoryMarshal.Cast<byte, T>(buffer.Read((int)length).Span);

            return span.ToArray();
        }

        public static void EncodeArray<T>(this ref Buffer buffer, T[] data) where T : ISerializer
        {
            ulong dataLen = (ulong)data.Length;
            buffer.Push<ulong>(ref dataLen);

            for (int i = 0; i < data.Length; i++)
            {
                Encode(ref buffer, data[i]);
            }
        }

        public static T[] DecodeArray<T>(this ref ReadOnlyBuffer buffer) where T : ISerializer
        {
            buffer.Pop<ulong>(out ulong length);

            T[] data = new T[(int)length];
            for (int i = 0; i < (int)length; i++)
            {
                data[i] = (T)Decode(ref buffer, typeof(T));
            }

            return data;
        }
    }
}
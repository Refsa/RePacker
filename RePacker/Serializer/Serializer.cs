using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Unsafe;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker
{
    public static class Serializer
    {
        public static void Encode(this ref Buffer buffer, ISerializer target)
        {
            target.ToBuffer(ref buffer);
        }

        public static object Decode(this ref Buffer buffer, Type type)
        {
            ISerializer instance = Activator.CreateInstance(type) as ISerializer;

            instance.FromBuffer(ref buffer);

            return instance;
        }

        public static T Decode<T>(this ref Buffer buffer)
        {
            ISerializer instance = Activator.CreateInstance(typeof(T)) as ISerializer;

            instance.FromBuffer(ref buffer);

            return (T)instance;
        }

        public static void EncodeString(this ref Buffer buffer, ref string str)
        {
            ulong length = (ulong)str.Length;
            buffer.Push<ulong>(ref length);

            var bytes = System.Text.Encoding.UTF8.GetBytes(str);
            buffer.Write(new ReadOnlySpan<byte>(bytes));
        }

        public static string DecodeString(this ref Buffer buffer)
        {
            buffer.Pop<ulong>(out ulong length);
            int start = buffer.Cursor();

            if (MemoryMarshal.TryGetArray(buffer.Read((int)length), out var seg))
            {
                return System.Text.Encoding.UTF8.GetString(seg.Array, start, (int)length);
            }

            return "";
        }

        public static void UnpackString(this ref Buffer buffer, out string value)
        {
            buffer.Pop<ulong>(out ulong length);
            int start = buffer.Cursor();

            if (MemoryMarshal.TryGetArray(buffer.Read((int)length), out var seg))
            {
                value = System.Text.Encoding.UTF8.GetString(seg.Array, start, (int)length);
                return;
            }

            value = "";
        }

        public static void EncodeEnum<TEnum>(this ref Buffer buffer, ref TEnum target) where TEnum : unmanaged, Enum
        {
            TypeCode enumType = System.Type.GetTypeCode(System.Enum.GetUnderlyingType(typeof(TEnum)));

            switch (enumType)
            {
                case TypeCode.Byte:
                    byte bval = target.ToUnderlyingType<TEnum, byte>();
                    buffer.Push<byte>(ref bval);
                    break;
                case TypeCode.SByte:
                    sbyte sbval = target.ToUnderlyingType<TEnum, sbyte>();
                    buffer.Push<sbyte>(ref sbval);
                    break;
                case TypeCode.Int16:
                    short i16 = target.ToUnderlyingType<TEnum, short>();
                    buffer.Push<short>(ref i16);
                    break;
                case TypeCode.Int32:
                    int i32 = target.ToUnderlyingType<TEnum, int>();
                    buffer.Push<int>(ref i32);
                    break;
                case TypeCode.Int64:
                    long i64 = target.ToUnderlyingType<TEnum, long>();
                    buffer.Push<long>(ref i64);
                    break;
                case TypeCode.UInt16:
                    ushort u16 = target.ToUnderlyingType<TEnum, ushort>();
                    buffer.Push<ushort>(ref u16);
                    break;
                case TypeCode.UInt32:
                    uint u32 = target.ToUnderlyingType<TEnum, uint>();
                    buffer.Push<uint>(ref u32);
                    break;
                case TypeCode.UInt64:
                    ulong u64 = target.ToUnderlyingType<TEnum, ulong>();
                    buffer.Push<ulong>(ref u64);
                    break;
            }
        }

        public static TEnum DecodeEnum<TEnum>(this ref Buffer buffer) where TEnum : unmanaged, Enum
        {
            TypeCode enumType = System.Type.GetTypeCode(System.Enum.GetUnderlyingType(typeof(TEnum)));

            switch (enumType)
            {
                case TypeCode.Byte:
                    buffer.Pop<byte>(out byte bval);
                    return EnumHelper.ToEnum<byte, TEnum>(ref bval);
                case TypeCode.SByte:
                    buffer.Pop<sbyte>(out sbyte sbval);
                    return EnumHelper.ToEnum<sbyte, TEnum>(ref sbval);
                case TypeCode.Int16:
                    buffer.Pop<short>(out short i16);
                    return EnumHelper.ToEnum<short, TEnum>(ref i16);
                case TypeCode.Int32:
                    buffer.Pop<int>(out int i32);
                    return EnumHelper.ToEnum<int, TEnum>(ref i32);
                case TypeCode.Int64:
                    buffer.Pop<long>(out long i64);
                    return EnumHelper.ToEnum<long, TEnum>(ref i64);
                case TypeCode.UInt16:
                    buffer.Pop<ushort>(out ushort u16);
                    return EnumHelper.ToEnum<ushort, TEnum>(ref u16);
                case TypeCode.UInt32:
                    buffer.Pop<uint>(out uint u32);
                    return EnumHelper.ToEnum<uint, TEnum>(ref u32);
                case TypeCode.UInt64:
                    buffer.Pop<ulong>(out ulong u64);
                    return EnumHelper.ToEnum<ulong, TEnum>(ref u64);
            }

            return default(TEnum);
        }

        public static void EncodeBlittableArray<T>(this ref Buffer buffer, T[] data) where T : unmanaged
        {
            ulong dataLen = (ulong)(data.Length * Marshal.SizeOf<T>());
            buffer.Push<ulong>(ref dataLen);

            var span = MemoryMarshal.Cast<T, byte>(new Span<T>(data));
            buffer.Write(span);
        }

        public static T[] DecodeBlittableArray<T>(this ref Buffer buffer) where T : unmanaged
        {
            buffer.Pop<ulong>(out ulong length);
            var span = MemoryMarshal.Cast<byte, T>(buffer.Read((int)length).Span);
            return span.ToArray();
        }

        public static void UnpackBlittableArray<T>(this ref Buffer buffer, out T[] data) where T : unmanaged
        {
            buffer.Pop<ulong>(out ulong length);
            var span = MemoryMarshal.Cast<byte, T>(buffer.Read((int)length).Span);
            data = span.ToArray();
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

        public static T[] DecodeArray<T>(this ref Buffer buffer) where T : ISerializer
        {
            buffer.Pop<ulong>(out ulong length);

            T[] data = new T[(int)length];
            for (int i = 0; i < (int)length; i++)
            {
                data[i] = (T)Decode(ref buffer, typeof(T));
            }

            return data;
        }

        public static void PackArray<T>(this BoxedBuffer buffer, T[] data)
        {
            if (TypeCache.TryGetTypePacker(typeof(T), out var packer))
            {
                ulong dataLen = (ulong)data.Length;
                buffer.Buffer.Push<ulong>(ref dataLen);

                for (int i = 0; i < data.Length; i++)
                {
                    packer.Pack<T>(buffer, ref data[i]);
                }
            }
        }

        public static void UnpackArray<T>(this BoxedBuffer buffer, out T[] data)
        {
            if (TypeCache.TryGetTypePacker(typeof(T), out var packer))
            {
                buffer.Buffer.Pop<ulong>(out ulong len);
                data = new T[(int)len];

                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = packer.Unpack<T>(buffer);
                }
            }
            else
            {
                data = null;
            }
        }

        public static void PackIList<T>(this BoxedBuffer buffer, IList<T> data)
        {
            if (TypeCache.TryGetTypePacker(typeof(T), out var packer))
            {
                ulong dataLen = (ulong)data.Count;
                buffer.Buffer.Push<ulong>(ref dataLen);

                for (int i = 0; i < data.Count; i++)
                {
                    var ele = data[i];
                    packer.Pack<T>(buffer, ref ele);
                }
            }
        }

        public static void UnpackIList<T>(this BoxedBuffer buffer, out IList<T> data)
        {
            if (TypeCache.TryGetTypePacker(typeof(T), out var packer))
            {
                buffer.Buffer.Pop<ulong>(out ulong len);

                data = new List<T>();
                for (int i = 0; i < (int)len; i++)
                {
                    data.Add(RePacker.Unpack<T>(buffer));
                }
            }
            else
            {
                data = null;
            }
        }

        public static void PackIListBlittable<T>(this BoxedBuffer buffer, IList<T> data) where T : unmanaged
        {
            ulong dataLen = (ulong)data.Count;
            buffer.Buffer.Push<ulong>(ref dataLen);

            for (int i = 0; i < data.Count; i++)
            {
                var ele = data[i];
                buffer.Buffer.Push<T>(ref ele);
            }
        }

        public static void UnpackIListBlittable<T>(this BoxedBuffer buffer, out IList<T> data) where T : unmanaged
        {
            buffer.Buffer.Pop<ulong>(out ulong len);

            data = new List<T>();
            for (int i = 0; i < (int)len; i++)
            {
                buffer.Buffer.Pop<T>(out T value);
                data.Add(value);
            }
        }
    }
}
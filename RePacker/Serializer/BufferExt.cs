using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RePacker.Buffers;
using RePacker.Unsafe;
using Buffer = RePacker.Buffers.Buffer;

namespace RePacker.Builder
{
    public static class BufferExt
    {
        static uint ZERO_UINT = 0;
        static ulong ZERO_ULONG = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackString(this ref Buffer buffer, ref string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                buffer.Pack(ref ZERO_ULONG);
                return;
            }

            int count = 0;
            if (buffer.Array != null)
            {
                count = StringHelper.CopyString(str, buffer.Array, buffer.WriteCursor() + sizeof(ulong));
                
                ulong c = (ulong)count;
                buffer.Pack(ref c);

                buffer.MoveWriteCursor(count);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string UnpackString(this ref Buffer buffer)
        {
            buffer.Unpack(out ulong length);

            if (length == 0)
            {
                return "";
            }

            int start = buffer.ReadCursor();

            if (buffer.Array != null)
            {
                string s = StringHelper.GetString(buffer.Array, start, (int)length);
                buffer.MoveReadCursor((int)length);
                return s;
            }

            return "";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackString(this ref Buffer buffer, out string value)
        {
            value = UnpackString(ref buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackDateTime(this ref Buffer buffer, ref DateTime value)
        {
            long ticks = value.Ticks;
            buffer.PushLong(ref ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackDateTime(this ref Buffer buffer, out DateTime value)
        {
            buffer.PopLong(out long ticks);
            value = new DateTime(ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackEnum<TEnum>(this ref Buffer buffer, ref TEnum target) where TEnum : unmanaged, Enum
        {
            TypeCode enumType = target.GetTypeCode();

            switch (enumType)
            {
                case TypeCode.Byte:
                    byte bval = target.ToUnderlyingType<TEnum, byte>();
                    buffer.PushByte(ref bval);
                    break;
                case TypeCode.SByte:
                    sbyte sbval = target.ToUnderlyingType<TEnum, sbyte>();
                    buffer.PushSByte(ref sbval);
                    break;
                case TypeCode.Int16:
                    short i16 = target.ToUnderlyingType<TEnum, short>();
                    buffer.PushShort(ref i16);
                    break;
                case TypeCode.Int32:
                    int i32 = target.ToUnderlyingType<TEnum, int>();
                    buffer.PushInt(ref i32);
                    break;
                case TypeCode.Int64:
                    long i64 = target.ToUnderlyingType<TEnum, long>();
                    buffer.PushLong(ref i64);
                    break;
                case TypeCode.UInt16:
                    ushort u16 = target.ToUnderlyingType<TEnum, ushort>();
                    buffer.PushUShort(ref u16);
                    break;
                case TypeCode.UInt32:
                    uint u32 = target.ToUnderlyingType<TEnum, uint>();
                    buffer.PushUInt(ref u32);
                    break;
                case TypeCode.UInt64:
                    ulong u64 = target.ToUnderlyingType<TEnum, ulong>();
                    buffer.PushULong(ref u64);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum UnpackEnum<TEnum>(this ref Buffer buffer) where TEnum : unmanaged, Enum
        {
            TypeCode enumType = System.Type.GetTypeCode(System.Enum.GetUnderlyingType(typeof(TEnum)));

            switch (enumType)
            {
                case TypeCode.Byte:
                    buffer.PopByte(out byte bval);
                    return EnumHelper.ToEnum<byte, TEnum>(ref bval);
                case TypeCode.SByte:
                    buffer.PopSByte(out sbyte sbval);
                    return EnumHelper.ToEnum<sbyte, TEnum>(ref sbval);
                case TypeCode.Int16:
                    buffer.PopShort(out short i16);
                    return EnumHelper.ToEnum<short, TEnum>(ref i16);
                case TypeCode.Int32:
                    buffer.PopInt(out int i32);
                    return EnumHelper.ToEnum<int, TEnum>(ref i32);
                case TypeCode.Int64:
                    buffer.PopLong(out long i64);
                    return EnumHelper.ToEnum<long, TEnum>(ref i64);
                case TypeCode.UInt16:
                    buffer.PopUShort(out ushort u16);
                    return EnumHelper.ToEnum<ushort, TEnum>(ref u16);
                case TypeCode.UInt32:
                    buffer.PopUInt(out uint u32);
                    return EnumHelper.ToEnum<uint, TEnum>(ref u32);
                case TypeCode.UInt64:
                    buffer.PopULong(out ulong u64);
                    return EnumHelper.ToEnum<ulong, TEnum>(ref u64);
            }

            return default(TEnum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackBlittableArray<T>(this ref Buffer buffer, T[] data) where T : unmanaged
        {
            buffer.MemoryCopyFromUnsafe(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] UnpackBlittableArray<T>(this ref Buffer buffer) where T : unmanaged
        {
            return buffer.MemoryCopyToUnsafe<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackUnmanagedArrayOut<T>(this ref Buffer buffer, out T[] data) where T : unmanaged
        {
            data = buffer.MemoryCopyToUnsafe<T>();
        }
    }
}


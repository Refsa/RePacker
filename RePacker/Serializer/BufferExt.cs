using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RePacker.Buffers;
using RePacker.Unsafe;
using Buffer = RePacker.Buffers.Buffer;

namespace RePacker.Buffers
{
    public static class BufferExt
    {
        static uint ZERO_UINT = 0;
        static ulong ZERO_ULONG = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackString(this Buffer buffer, ref string str)
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
        public static string UnpackString(this Buffer buffer)
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
        public static void UnpackString(this Buffer buffer, out string value)
        {
            value = UnpackString(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackDateTime(this Buffer buffer, ref DateTime value)
        {
            long ticks = value.Ticks;
            buffer.Pack(ref ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackDateTime(this Buffer buffer, out DateTime value)
        {
            buffer.Unpack(out long ticks);
            value = new DateTime(ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackTimeSpan(this Buffer buffer, ref TimeSpan value)
        {
            long ticks = value.Ticks;
            buffer.Pack(ref ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackTimeSpan(this Buffer buffer, out TimeSpan value)
        {
            buffer.Unpack(out long ticks);
            value = new TimeSpan(ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackEnum<TEnum>(this Buffer buffer, ref TEnum target) where TEnum : unmanaged, Enum
        {
            buffer.Pack(ref target);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum UnpackEnum<TEnum>(this Buffer buffer) where TEnum : unmanaged, Enum
        {
            buffer.Unpack(out TEnum value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackBlittableArray<T>(this Buffer buffer, T[] data) where T : unmanaged
        {
            buffer.PackArray(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] UnpackBlittableArray<T>(this Buffer buffer) where T : unmanaged
        {
            return buffer.UnpackArray<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackUnmanagedArrayOut<T>(this Buffer buffer, out T[] data) where T : unmanaged
        {
            data = buffer.UnpackArray<T>();
        }

        public static void PackKeyValuePair<T1, T2>(this Buffer buffer, ref KeyValuePair<T1, T2> value)
        {
            var k = value.Key;
            RePacking.Pack<T1>(buffer, ref k);

            var v = value.Value;
            RePacking.Pack<T2>(buffer, ref v);
        }

        public static void UnpackKeyValuePair<T1, T2>(this Buffer buffer, out KeyValuePair<T1, T2> value)
        {
            value = new KeyValuePair<T1, T2>(
                RePacking.Unpack<T1>(buffer),
                RePacking.Unpack<T2>(buffer)
            );
        }

        public static void PackNullable<TValue>(this Buffer buffer, ref Nullable<TValue> value) where TValue : unmanaged
        {
            bool hasValue = value.HasValue;
            buffer.Pack(ref hasValue);

            if (hasValue)
            {
                TValue v = value.Value;
                RePacking.Pack(buffer, ref v);
            }
        }

        public static void UnpackNullable<TValue>(this Buffer buffer, out Nullable<TValue> value) where TValue : unmanaged
        {
            buffer.Unpack<bool>(out bool hasValue);

            if (hasValue)
            {
                value = RePacking.Unpack<TValue>(buffer);
            }
            else
            {
                value = null;
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RePacker.Buffers;
using RePacker.Unsafe;
using Buffer = RePacker.Buffers.ReBuffer;

namespace RePacker.Buffers
{
    public static class BufferExt
    {
        static uint ZERO_UINT = 0;
        static ulong ZERO_ULONG = 0;

        /// <summary>
        /// Packs String into buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="str"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackString(this ReBuffer buffer, ref string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                buffer.Pack(ref ZERO_ULONG);
                return;
            }

            int count = 0;
            if (buffer.Array != null)
            {
                int str_len = StringHelper.SizeOf(str);

                if (!buffer.CanWriteBytes(str_len + sizeof(ulong)))
                {
                    throw new IndexOutOfRangeException($"Cant fit string of size {str_len} into buffer");
                }

                count = StringHelper.CopyString(str, buffer.Array, buffer.WriteCursor() + sizeof(ulong));

                ulong c = (ulong)count;
                buffer.Pack(ref c);

                buffer.MoveWriteCursor(count);
            }
        }

        /// <summary>
        /// Unpacks a String from buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string UnpackString(this ReBuffer buffer)
        {
            buffer.Unpack(out ulong length);

            if (length == 0)
            {
                return "";
            }

            int start = buffer.ReadCursor();

            if (buffer.Array != null)
            {
                if (!buffer.CanReadBytes((int)length))
                {
                    throw new IndexOutOfRangeException($"Cant read string of size {length} from buffer");
                }

                string s = StringHelper.GetString(buffer.Array, start, (int)length);
                buffer.MoveReadCursor((int)length);
                return s;
            }

            return "";
        }

        /// <summary>
        /// Unpacks a String from buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackString(this ReBuffer buffer, out string value)
        {
            value = UnpackString(buffer);
        }

        /// <summary>
        /// Packs DateTime into buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="str"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackDateTime(this ReBuffer buffer, ref DateTime value)
        {
            long ticks = value.Ticks;
            buffer.Pack(ref ticks);
        }

        /// <summary>
        /// Unpacks a DateTime from buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackDateTime(this ReBuffer buffer, out DateTime value)
        {
            buffer.Unpack(out long ticks);
            value = new DateTime(ticks);
        }

        /// <summary>
        /// Packs TimeSpan into buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="str"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackTimeSpan(this ReBuffer buffer, ref TimeSpan value)
        {
            long ticks = value.Ticks;
            buffer.Pack(ref ticks);
        }

        /// <summary>
        /// Unpacks a TimeSpan from buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackTimeSpan(this ReBuffer buffer, out TimeSpan value)
        {
            buffer.Unpack(out long ticks);
            value = new TimeSpan(ticks);
        }

        /// <summary>
        /// Packs Enum into buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="str"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackEnum<TEnum>(this ReBuffer buffer, ref TEnum target) where TEnum : unmanaged, Enum
        {
            buffer.Pack(ref target);
        }

        /// <summary>
        /// Unpacks a Enum from buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TEnum UnpackEnum<TEnum>(this ReBuffer buffer) where TEnum : unmanaged, Enum
        {
            buffer.Unpack(out TEnum value);
            return value;
        }

        /// <summary>
        /// Packs BlittableArray into buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="str"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackBlittableArray<T>(this ReBuffer buffer, T[] data) where T : unmanaged
        {
            buffer.PackArray(data);
        }

        /// <summary>
        /// Unpacks a BlittableArray from buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] UnpackBlittableArray<T>(this ReBuffer buffer) where T : unmanaged
        {
            return buffer.UnpackArray<T>();
        }

        /// <summary>
        /// Unpacks a UnmanagedArrayOut from buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackUnmanagedArrayOut<T>(this ReBuffer buffer, out T[] data) where T : unmanaged
        {
            data = buffer.UnpackArray<T>();
        }

        /// <summary>
        /// Packs KeyValuePair into buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="str"></param>

        public static void PackKeyValuePair<T1, T2>(this ReBuffer buffer, ref KeyValuePair<T1, T2> value)
        {
            var k = value.Key;
            RePacking.Pack<T1>(buffer, ref k);

            var v = value.Value;
            RePacking.Pack<T2>(buffer, ref v);
        }

        /// <summary>
        /// Unpacks a KeyValuePair from buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>

        public static void UnpackKeyValuePair<T1, T2>(this ReBuffer buffer, out KeyValuePair<T1, T2> value)
        {
            value = new KeyValuePair<T1, T2>(
                RePacking.Unpack<T1>(buffer),
                RePacking.Unpack<T2>(buffer)
            );
        }

        /// <summary>
        /// Packs Nullable into buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="str"></param>

        public static void PackNullable<TValue>(this ReBuffer buffer, ref Nullable<TValue> value) where TValue : unmanaged
        {
            bool hasValue = value.HasValue;
            buffer.Pack(ref hasValue);

            if (hasValue)
            {
                TValue v = value.Value;
                RePacking.Pack(buffer, ref v);
            }
        }

        /// <summary>
        /// Unpacks a Nullable from buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>

        public static void UnpackNullable<TValue>(this ReBuffer buffer, out Nullable<TValue> value) where TValue : unmanaged
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


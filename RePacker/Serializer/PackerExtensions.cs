using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Unsafe;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker
{
    public static class PackerExtensions
    {
        public static void Pack(this ref Buffer buffer, IPacker target)
        {
            target.ToBuffer(ref buffer);
        }

        public static object Unpack(this ref Buffer buffer, Type type)
        {
            IPacker instance = Activator.CreateInstance(type) as IPacker;

            instance.FromBuffer(ref buffer);

            return instance;
        }

        public static T Unpack<T>(this ref Buffer buffer) where T : IPacker
        {
            IPacker instance = Activator.CreateInstance(typeof(T)) as IPacker;

            instance.FromBuffer(ref buffer);

            return (T)instance;
        }

        public static void PackString(this ref Buffer buffer, ref string str)
        {
            ulong length = (ulong)str.Length;
            buffer.Push<ulong>(ref length);

            var bytes = System.Text.Encoding.UTF8.GetBytes(str);
            buffer.Write(new ReadOnlySpan<byte>(bytes));
        }

        public static string UnpackString(this ref Buffer buffer)
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

        public static void PackDateTime(this ref Buffer buffer, ref DateTime value)
        {
            long ticks = value.Ticks;
            buffer.PushLong(ref ticks);
        }

        public static void UnpackDateTime(this ref Buffer buffer, out DateTime value)
        {
            buffer.PopLong(out long ticks);
            value = new DateTime(ticks);
        }

        public static void PackEnum<TEnum>(this ref Buffer buffer, ref TEnum target) where TEnum : unmanaged, Enum
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

        public static TEnum UnpackEnum<TEnum>(this ref Buffer buffer) where TEnum : unmanaged, Enum
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

        public static void EncodeArray<T>(this ref Buffer buffer, T[] data) where T : IPacker
        {
            ulong dataLen = (ulong)data.Length;
            buffer.Push<ulong>(ref dataLen);

            for (int i = 0; i < data.Length; i++)
            {
                Pack(ref buffer, data[i]);
            }
        }

        public static T[] DecodeArray<T>(this ref Buffer buffer) where T : IPacker
        {
            buffer.Pop<ulong>(out ulong length);

            T[] data = new T[(int)length];
            for (int i = 0; i < (int)length; i++)
            {
                data[i] = (T)Unpack(ref buffer, typeof(T));
            }

            return data;
        }

        #region BoxedBuffer
        public static void PackDateTime(this BoxedBuffer buffer, ref DateTime value)
        {
            long ticks = value.Ticks;
            buffer.Buffer.PushLong(ref ticks);
        }

        public static void UnpackDateTime(this BoxedBuffer buffer, out DateTime value)
        {
            buffer.Buffer.PopLong(out long ticks);
            value = new DateTime(ticks);
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

        public static T[] UnpackArrayAsRet<T>(this BoxedBuffer buffer)
        {
            UnpackArray<T>(buffer, out var data);
            return data;
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

        public static void PackIEnumerable<T>(this BoxedBuffer buffer, IEnumerable<T> data)
        {
            if (TypeCache.TryGetTypePacker(typeof(T), out var packer))
            {
                ulong dataLen = (ulong)data.Count();
                buffer.Buffer.Push<ulong>(ref dataLen);

                foreach (var element in data)
                {
                    var ele = element;
                    packer.Pack<T>(buffer, ref ele);
                }
            }
        }

        public static void UnpackIEnumerable<T>(this BoxedBuffer buffer, IEnumerableType type, out IEnumerable<T> data)
        {
            if (TypeCache.TryGetTypePacker(typeof(T), out var packer))
            {
                buffer.Buffer.Pop<ulong>(out ulong len);

                T[] temp_data = new T[(int)len];
                for (int i = 0; i < (int)len; i++)
                {
                    temp_data[i] = RePacker.Unpack<T>(buffer);
                }

                var asSpan = new Span<T>(temp_data);
                data = CreateBaseIEnumerableType<T>(type, ref asSpan);
            }
            else
            {
                data = null;
            }
        }

        public static void PackIEnumerableBlittable<T>(this BoxedBuffer buffer, IEnumerable<T> data) where T : unmanaged
        {
            ulong dataLen = (ulong)data.Count();
            buffer.Buffer.Push<ulong>(ref dataLen);

            foreach (T element in data)
            {
                T ele = element;
                buffer.Buffer.Push<T>(ref ele);
            }
        }

        public static void UnpackIEnumerableBlittable<T>(this BoxedBuffer buffer, IEnumerableType type, out IEnumerable<T> data) where T : unmanaged
        {
            buffer.Buffer.Pop<ulong>(out ulong len);

            Span<T> temp_data = stackalloc T[(int)len];
            for (int i = 0; i < (int)len; i++)
            {
                buffer.Buffer.Pop<T>(out T value);
                temp_data[i] = value;
            }

            data = CreateBaseIEnumerableType<T>(type, ref temp_data);
        }

        static IEnumerable<T> CreateBaseIEnumerableType<T>(IEnumerableType type, ref Span<T> from)
        {
            if (type == IEnumerableType.Queue)
            {
                var container = new Queue<T>(from.Length);
                foreach (var item in from)
                {
                    container.Enqueue(item);
                }
                return container;
            }
            else if (type == IEnumerableType.Stack)
            {
                var container = new Stack<T>(from.Length);
                for (int i = from.Length - 1; i >= 0; i--)
                {
                    container.Push(from[i]);
                }
                return container;
            }
            else if (type == IEnumerableType.HashSet)
            {
                var container = new HashSet<T>(from.Length);
                foreach (var item in from)
                {
                    container.Add(item);
                }
                return container;
            }

            return from.ToArray();
        }

        public static void RecreateDictionary<K, V>(IList<K> keys, IList<V> values, out Dictionary<K, V> dict)
        {
            dict = new Dictionary<K, V>();

            for (int i = 0; i < keys.Count; i++)
            {
                dict.Add(keys[i], values[i]);
            }
        }

        public static void PackKeyValuePair<T1, T2>(this BoxedBuffer buffer, ref KeyValuePair<T1, T2> value)
        {
            var k = value.Key;
            RePacker.Pack<T1>(buffer, ref k);

            var v = value.Value;
            RePacker.Pack<T2>(buffer, ref v);
        }

        public static void UnpackKeyValuePair<T1, T2>(this BoxedBuffer buffer, out KeyValuePair<T1, T2> value)
        {
            value = new KeyValuePair<T1, T2>(
                RePacker.Unpack<T1>(buffer),
                RePacker.Unpack<T2>(buffer)
            );
        }

        public static void PackValueTuple<T1, T2>(this BoxedBuffer buffer, ref ValueTuple<T1, T2> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
        }

        public static void PackValueTuple<T1, T2, T3>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
        }

        public static void PackValueTuple<T1, T2, T3, T4>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
            RePacker.Pack<T4>(buffer, ref value.Item4);
        }

        public static void PackValueTuple<T1, T2, T3, T4, T5>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
            RePacker.Pack<T4>(buffer, ref value.Item4);
            RePacker.Pack<T5>(buffer, ref value.Item5);
        }

        public static void PackValueTuple<T1, T2, T3, T4, T5, T6>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
            RePacker.Pack<T4>(buffer, ref value.Item4);
            RePacker.Pack<T5>(buffer, ref value.Item5);
            RePacker.Pack<T6>(buffer, ref value.Item6);
        }

        public static void PackValueTuple<T1, T2, T3, T4, T5, T6, T7>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
            RePacker.Pack<T4>(buffer, ref value.Item4);
            RePacker.Pack<T5>(buffer, ref value.Item5);
            RePacker.Pack<T6>(buffer, ref value.Item6);
            RePacker.Pack<T7>(buffer, ref value.Item7);
        }

        public static void PackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value) where TRest : struct
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
            RePacker.Pack<T4>(buffer, ref value.Item4);
            RePacker.Pack<T5>(buffer, ref value.Item5);
            RePacker.Pack<T6>(buffer, ref value.Item6);
            RePacker.Pack<T7>(buffer, ref value.Item7);
            RePacker.Pack<TRest>(buffer, ref value.Rest);
        }

        public static void UnpackValueTuple<T1, T2>(this BoxedBuffer buffer, out ValueTuple<T1, T2> value)
        {
            value = new ValueTuple<T1, T2>(RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3> value)
        {
            value = new ValueTuple<T1, T2, T3>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3, T4>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3, T4> value)
        {
            value = new ValueTuple<T1, T2, T3, T4>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer),
                RePacker.Unpack<T4>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3, T4, T5>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5> value)
        {
            value = new ValueTuple<T1, T2, T3, T4, T5>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer),
                RePacker.Unpack<T4>(buffer), RePacker.Unpack<T5>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3, T4, T5, T6>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            value = new ValueTuple<T1, T2, T3, T4, T5, T6>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer),
                RePacker.Unpack<T4>(buffer), RePacker.Unpack<T5>(buffer), RePacker.Unpack<T6>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            value = new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer),
                RePacker.Unpack<T4>(buffer), RePacker.Unpack<T5>(buffer), RePacker.Unpack<T6>(buffer),
                RePacker.Unpack<T7>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value) where TRest : struct
        {
            value = new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer),
                RePacker.Unpack<T4>(buffer), RePacker.Unpack<T5>(buffer), RePacker.Unpack<T6>(buffer),
                RePacker.Unpack<T7>(buffer), RePacker.Unpack<TRest>(buffer));
        }
    }

    public enum IEnumerableType : byte
    {
        None = 0,
        HashSet,
        Queue,
        Stack,
    }
    #endregion
}


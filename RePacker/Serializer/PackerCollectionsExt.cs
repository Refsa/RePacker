using System;
using Refsa.RePacker.Buffers;
using System.Collections.Generic;
using System.Linq;
using Refsa.RePacker.Utils;
using System.Reflection;

namespace Refsa.RePacker.Builder
{
    public static class PackerCollectionsExt
    {
        public enum IEnumerableType : byte
        {
            None = 0,
            HashSet,
            Queue,
            Stack,
        }

        public static void PackArray<T>(this BoxedBuffer buffer, T[] data)
        {
            if (TypeCache.TryGetTypePacker(typeof(T), out var packer))
            {
                ulong dataLen = (ulong)data.Length;
                buffer.Buffer.PushULong(ref dataLen);

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
                buffer.Buffer.PopULong(out ulong len);
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
                buffer.Buffer.PushULong(ref dataLen);

                for (int i = 0; i < data.Count; i++)
                {
                    var ele = data[i];
                    packer.Pack<T>(buffer, ref ele);
                }
            }
            else
            {
                RePacker.Logger.Error($"Couldnt find packer for list with elements of {typeof(T)}");
            }
        }

        public static void UnpackIList<T>(this BoxedBuffer buffer, out IList<T> data)
        {
            if (TypeCache.TryGetTypePacker(typeof(T), out var packer))
            {
                buffer.Buffer.PopULong(out ulong len);

                data = new List<T>();
                for (int i = 0; i < (int)len; i++)
                {
                    data.Add(RePacker.Unpack<T>(buffer));
                }
            }
            else
            {
                RePacker.Logger.Error($"Couldnt find unpacker for list with elements of {typeof(T)}");
                data = null;
            }
        }

        public static void PackIListBlittable<T>(this BoxedBuffer buffer, IList<T> data) where T : unmanaged
        {
            buffer.MemoryCopy((T[])data);
        }

        public static void UnpackIListBlittable<T>(this BoxedBuffer buffer, out IList<T> data) where T : unmanaged
        {
            data = buffer.MemoryCopy<T>();
        }

        public static void PackIEnumerable<T>(this BoxedBuffer buffer, IEnumerable<T> data)
        {
            if (TypeCache.TryGetTypePacker(typeof(T), out var packer))
            {
                ulong dataLen = (ulong)data.Count();
                buffer.Buffer.PushULong(ref dataLen);

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
                buffer.Buffer.PopULong(out ulong len);

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
            buffer.MemoryCopy((T[])data);
        }

        public static void UnpackIEnumerableBlittable<T>(this BoxedBuffer buffer, IEnumerableType type, out IEnumerable<T> data) where T : unmanaged
        {
            data = buffer.MemoryCopy<T>();
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
                var container = new HashSet<T>();
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
    }
}
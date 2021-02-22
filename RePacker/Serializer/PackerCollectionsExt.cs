using System;
using RePacker.Buffers;
using System.Collections.Generic;
using System.Linq;
using RePacker.Utils;
using System.Reflection;

namespace RePacker.Builder
{
    public static class PackerCollectionsExt
    {
        #region Array
        public static void PackArray<T>(this BoxedBuffer buffer, T[] data)
        {
            if (data == null || data.Length == 0)
            {
                ulong zero = 0;
                buffer.Buffer.Pack(ref zero);
                return;
            }

            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                ulong dataLen = (ulong)data.Length;
                buffer.Buffer.PushULong(ref dataLen);

                for (int i = 0; i < data.Length; i++)
                {
                    packer.Pack(buffer, ref data[i]);
                }
            }
            else
            {
                throw new NotSupportedException($"Couldnt find packer for {typeof(T)}");
            }
        }

        public static void UnpackArray<T>(this BoxedBuffer buffer, out T[] data)
        {
            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                buffer.Buffer.PopULong(out ulong len);
                data = new T[(int)len];

                for (int i = 0; i < data.Length; i++)
                {
                    packer.Unpack(buffer, out data[i]);
                }
            }
            else
            {
                throw new NotSupportedException($"Couldnt find unpacker for {typeof(T)}");
            }
        }

        public static void PackArray2D<T>(this BoxedBuffer buffer, ref T[,] data)
        {
            if (data == null)
            {
                int zero = 0;
                buffer.Buffer.Pack(ref zero);
                buffer.Buffer.Pack(ref zero);
                return;
            }

            int width = data.GetLength(0);
            int height = data.GetLength(1);

            buffer.Buffer.Pack(ref width);
            buffer.Buffer.Pack(ref height);

            if (width == 0 || height == 0)
            {
                return;
            }

            for (int x = 0; x < (int)width; x++)
            {
                for (int y = 0; y < (int)height; y++)
                {
                    buffer.Pack(ref data[x, y]);
                }
            }
        }

        public static void UnpackArray2D<T>(this BoxedBuffer buffer, out T[,] data)
        {
            buffer.Buffer.Unpack<int>(out int width);
            buffer.Buffer.Unpack<int>(out int height);

            data = new T[width, height];

            if (width == 0 || height == 0)
            {
                return;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    data[x, y] = buffer.Unpack<T>();
                }
            }
        }

        public static void PackArray3D<T>(this BoxedBuffer buffer, ref T[,,] data)
        {
            if (data == null)
            {
                ulong zero = 0;
                buffer.Buffer.Pack(ref zero);
                buffer.Buffer.Pack(ref zero);
                buffer.Buffer.Pack(ref zero);
                return;
            }

            ulong width = (ulong)data.GetLength(0);
            ulong height = (ulong)data.GetLength(1);
            ulong depth = (ulong)data.GetLength(2);

            buffer.Buffer.Pack(ref width);
            buffer.Buffer.Pack(ref height);
            buffer.Buffer.Pack(ref depth);

            if (width == 0 || height == 0)
            {
                return;
            }

            for (int x = 0; x < (int)width; x++)
                for (int y = 0; y < (int)height; y++)
                    for (int z = 0; z < (int)depth; z++)
                    {
                        buffer.Pack(ref data[x, y, z]);
                    }
        }

        public static void UnpackArray3D<T>(this BoxedBuffer buffer, out T[,,] data)
        {
            buffer.Buffer.Unpack<ulong>(out ulong width);
            buffer.Buffer.Unpack<ulong>(out ulong height);
            buffer.Buffer.Unpack<ulong>(out ulong depth);

            data = new T[(int)width, (int)height, (int)depth];

            if (width == 0 || height == 0 || depth == 0)
            {
                return;
            }

            for (int x = 0; x < (int)width; x++)
                for (int y = 0; y < (int)height; y++)
                    for (int z = 0; z < (int)depth; z++)
                    {
                        data[x, y, z] = buffer.Unpack<T>();
                    }
        }

        public static void PackArray4D<T>(this BoxedBuffer buffer, ref T[,,,] data)
        {
            if (data == null)
            {
                ulong zero = 0;
                buffer.Buffer.Pack(ref zero);
                buffer.Buffer.Pack(ref zero);
                buffer.Buffer.Pack(ref zero);
                buffer.Buffer.Pack(ref zero);
                return;
            }

            ulong width = (ulong)data.GetLength(0);
            ulong height = (ulong)data.GetLength(1);
            ulong depth = (ulong)data.GetLength(2);
            ulong seg = (ulong)data.GetLength(3);

            buffer.Buffer.Pack(ref width);
            buffer.Buffer.Pack(ref height);
            buffer.Buffer.Pack(ref depth);
            buffer.Buffer.Pack(ref seg);

            if (width == 0 || height == 0)
            {
                return;
            }

            for (int x = 0; x < (int)width; x++)
                for (int y = 0; y < (int)height; y++)
                    for (int z = 0; z < (int)depth; z++)
                        for (int w = 0; w < (int)seg; w++)
                        {
                            buffer.Pack(ref data[x, y, z, w]);
                        }
        }

        public static void UnpackArray4D<T>(this BoxedBuffer buffer, out T[,,,] data)
        {
            buffer.Buffer.Unpack<ulong>(out ulong width);
            buffer.Buffer.Unpack<ulong>(out ulong height);
            buffer.Buffer.Unpack<ulong>(out ulong depth);
            buffer.Buffer.Unpack<ulong>(out ulong seg);

            data = new T[(int)width, (int)height, (int)depth, (int)seg];

            if (width == 0 || height == 0 || depth == 0 || seg == 0)
            {
                return;
            }

            for (int x = 0; x < (int)width; x++)
                for (int y = 0; y < (int)height; y++)
                    for (int z = 0; z < (int)depth; z++)
                        for (int w = 0; w < (int)seg; w++)
                        {
                            data[x, y, z, w] = buffer.Unpack<T>();
                        }
        }
        #endregion

        #region IList
        public static void PackIList<T>(this BoxedBuffer buffer, IList<T> data)
        {
            if (data == null)
            {
                ulong zero = 0;
                buffer.Buffer.PushULong(ref zero);
                return;
            }

            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                ulong dataLen = (ulong)data.Count;
                buffer.Buffer.PushULong(ref dataLen);

                for (int i = 0; i < data.Count; i++)
                {
                    var ele = data[i];
                    packer.Pack(buffer, ref ele);
                }
            }
            else
            {
                throw new NotSupportedException($"Couldnt find packer for {typeof(T)}");
            }
        }

        public static void UnpackIList<T>(this BoxedBuffer buffer, out IList<T> data)
        {
            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                buffer.Buffer.PopULong(out ulong len);

                data = new List<T>();
                for (int i = 0; i < (int)len; i++)
                {
                    packer.Unpack(buffer, out T value);
                    data.Add(value);
                }
            }
            else
            {
                throw new NotSupportedException($"Couldnt find unpacker for {typeof(T)}");
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
        #endregion

        #region IEnumerable
        public static void PackIEnumerable<T>(this BoxedBuffer buffer, IEnumerable<T> data)
        {
            if (data == null || data.Count() == 0)
            {
                ulong zero = 0;
                buffer.Buffer.PushULong(ref zero);
                return;
            }

            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                ulong dataLen = (ulong)data.Count();
                buffer.Buffer.PushULong(ref dataLen);

                foreach (var element in data)
                {
                    var ele = element;
                    packer.Pack(buffer, ref ele);
                }
            }
            else
            {
                throw new NotSupportedException($"Couldnt find packer for {typeof(T)}");
            }
        }

        public static void UnpackIEnumerable<T>(this BoxedBuffer buffer, out IEnumerable<T> data)
        {
            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                buffer.Buffer.PopULong(out ulong len);

                T[] temp_data = new T[(int)len];
                for (int i = 0; i < (int)len; i++)
                {
                    packer.Unpack(buffer, out temp_data[i]);
                }
                data = temp_data;
            }
            else
            {
                throw new NotSupportedException($"Couldnt find unpacker for {typeof(T)}");
            }
        }

        public static void PackIEnumerableBlittable<T>(this BoxedBuffer buffer, IEnumerable<T> data) where T : unmanaged
        {
            buffer.MemoryCopy((T[])data);
        }

        public static void UnpackIEnumerableBlittable<T>(this BoxedBuffer buffer, out IEnumerable<T> data) where T : unmanaged
        {
            T[] temp = buffer.MemoryCopy<T>();
            data = temp;
        }
        #endregion

        #region ICollection
        public static void PackICollection<T>(this BoxedBuffer buffer, ICollection<T> data)
        {
            if (data == null)
            {
                ulong zero = 0;
                buffer.Buffer.PushULong(ref zero);
                return;
            }

            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                ulong dataLen = (ulong)data.Count;
                buffer.Buffer.PushULong(ref dataLen);

                foreach (var d in data)
                {
                    var ele = d;
                    packer.Pack(buffer, ref ele);
                }
            }
            else
            {
                throw new NotSupportedException($"Couldnt find packer for {typeof(T)}");
            }
        }

        public static void UnpackICollection<T>(this BoxedBuffer buffer, out ICollection<T> data)
        {
            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                buffer.Buffer.PopULong(out ulong len);

                data = new T[(int)len];
                for (int i = 0; i < (int)len; i++)
                {
                    packer.Unpack(buffer, out T value);
                    data.Add(value);
                }
            }
            else
            {
                throw new NotSupportedException($"Couldnt find unpacker for {typeof(T)}");
            }
        }

        public static void PackICollectionBlittable<T>(this BoxedBuffer buffer, ICollection<T> data) where T : unmanaged
        {
            buffer.MemoryCopy((T[])data);
        }

        public static void UnpackICollectionBlittable<T>(this BoxedBuffer buffer, out ICollection<T> data) where T : unmanaged
        {
            data = buffer.MemoryCopy<T>();
        }
        #endregion

        public static void RecreateDictionary<K, V>(IList<K> keys, IList<V> values, out Dictionary<K, V> dict)
        {
            dict = new Dictionary<K, V>();

            for (int i = 0; i < keys.Count; i++)
            {
                dict.Add(keys[i], values[i]);
            }
        }

        public static void PackQueue<T>(this BoxedBuffer buffer, Queue<T> data)
        {
            PackIEnumerable<T>(buffer, data);
        }

        public static void PackStack<T>(this BoxedBuffer buffer, Stack<T> data)
        {
            if (data == null)
            {
                ulong zero = 0;
                buffer.Buffer.PushULong(ref zero);
                return;
            }

            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                ulong dataLen = (ulong)data.Count;
                buffer.Buffer.PushULong(ref dataLen);

                for (int i = (int)dataLen - 1; i >= 0; i--)
                {
                    var ele = data.ElementAt(i);
                    packer.Pack(buffer, ref ele);
                }
            }
            else
            {
                throw new NotSupportedException($"Couldnt find packer for {typeof(T)}");
            }
        }

        public static void PackHashSet<T>(this BoxedBuffer buffer, HashSet<T> data)
        {
            PackIEnumerable<T>(buffer, data);
        }

        static void UnpackCollectionInternal<T>(this BoxedBuffer buffer, Action<T> predicate)
        {
            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                buffer.Buffer.PopULong(out ulong len);

                for (int i = 0; i < (int)len; i++)
                {
                    packer.Unpack(buffer, out var item);
                    predicate.Invoke(item);
                }
            }
            else
            {
                throw new NotSupportedException($"Couldnt find unpacker for {typeof(T)}");
            }
        }

        public static void UnpackQueue<T>(this BoxedBuffer buffer, out Queue<T> data)
        {
            data = new Queue<T>();
            UnpackCollectionInternal<T>(buffer, data.Enqueue);
        }

        public static void UnpackStack<T>(this BoxedBuffer buffer, out Stack<T> data)
        {
            data = new Stack<T>();
            UnpackCollectionInternal<T>(buffer, data.Push);
        }

        public static void UnpackHashSet<T>(this BoxedBuffer buffer, out HashSet<T> data)
        {
            data = new HashSet<T>();
            UnpackCollectionInternal<T>(buffer, data.HashSetAdd);
        }

        static void HashSetAdd<T>(this HashSet<T> self, T item)
        {
            self.Add(item);
        }
    }
}
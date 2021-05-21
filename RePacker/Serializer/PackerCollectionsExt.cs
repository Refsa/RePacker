using RePacker.Buffers;
using System.Collections.Generic;
using System.Linq;
using RePacker.Utils;
using System.Reflection;
using System.Collections;

namespace RePacker.Builder
{
    public static class PackerCollectionsExt
    {
        #region Array
        /// <summary>
        /// Packs a managed Array
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackArray<T>(this ReBuffer buffer, T[] data)
        {
            if (data == null || data.Length == 0)
            {
                ulong zero = 0;
                buffer.Pack(ref zero);
                return;
            }

            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                ulong dataLen = (ulong)data.Length;
                buffer.Pack(ref dataLen);

                for (int i = 0; i < data.Length; i++)
                {
                    packer.Pack(buffer, ref data[i]);
                }
            }
            else
            {
                throw new System.NotSupportedException($"Couldnt find packer for {typeof(T)}");
            }
        }

        /// <summary>
        /// Unpacks a managed Array
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackArray<T>(this ReBuffer buffer, out T[] data)
        {
            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                buffer.Unpack(out ulong len);
                data = new T[(int)len];

                for (int i = 0; i < data.Length; i++)
                {
                    packer.Unpack(buffer, out data[i]);
                }
            }
            else
            {
                throw new System.NotSupportedException($"Couldnt find unpacker for {typeof(T)}");
            }
        }

        /// <summary>
        /// Packs a managed 2D Array
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackArray2D<T>(this ReBuffer buffer, ref T[,] data)
        {
            if (data == null)
            {
                int zero = 0;
                buffer.Pack(ref zero);
                buffer.Pack(ref zero);
                return;
            }

            int width = data.GetLength(0);
            int height = data.GetLength(1);

            buffer.Pack(ref width);
            buffer.Pack(ref height);

            if (width == 0 || height == 0)
            {
                return;
            }

            for (int x = 0; x < (int)width; x++)
            {
                for (int y = 0; y < (int)height; y++)
                {
                    RePacking.Pack(buffer, ref data[x, y]);
                }
            }
        }

        /// <summary>
        /// Unpacks a managed 2D Array
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackArray2D<T>(this ReBuffer buffer, out T[,] data)
        {
            buffer.Unpack<int>(out int width);
            buffer.Unpack<int>(out int height);

            data = new T[width, height];

            if (width == 0 || height == 0)
            {
                return;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    data[x, y] = RePacking.Unpack<T>(buffer);
                }
            }
        }

        /// <summary>
        /// Packs a managed 3D Array
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackArray3D<T>(this ReBuffer buffer, ref T[,,] data)
        {
            if (data == null)
            {
                ulong zero = 0;
                buffer.Pack(ref zero);
                buffer.Pack(ref zero);
                buffer.Pack(ref zero);
                return;
            }

            ulong width = (ulong)data.GetLength(0);
            ulong height = (ulong)data.GetLength(1);
            ulong depth = (ulong)data.GetLength(2);

            buffer.Pack(ref width);
            buffer.Pack(ref height);
            buffer.Pack(ref depth);

            if (width == 0 || height == 0)
            {
                return;
            }

            for (int x = 0; x < (int)width; x++)
                for (int y = 0; y < (int)height; y++)
                    for (int z = 0; z < (int)depth; z++)
                    {
                        RePacking.Pack(buffer, ref data[x, y, z]);
                    }
        }

        /// <summary>
        /// Unpacks a managed 3D Array
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackArray3D<T>(this ReBuffer buffer, out T[,,] data)
        {
            buffer.Unpack<ulong>(out ulong width);
            buffer.Unpack<ulong>(out ulong height);
            buffer.Unpack<ulong>(out ulong depth);

            data = new T[(int)width, (int)height, (int)depth];

            if (width == 0 || height == 0 || depth == 0)
            {
                return;
            }

            for (int x = 0; x < (int)width; x++)
                for (int y = 0; y < (int)height; y++)
                    for (int z = 0; z < (int)depth; z++)
                    {
                        data[x, y, z] = RePacking.Unpack<T>(buffer);
                    }
        }

        /// <summary>
        /// Packs a managed 4D Array
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackArray4D<T>(this ReBuffer buffer, ref T[,,,] data)
        {
            if (data == null)
            {
                ulong zero = 0;
                buffer.Pack(ref zero);
                buffer.Pack(ref zero);
                buffer.Pack(ref zero);
                buffer.Pack(ref zero);
                return;
            }

            ulong width = (ulong)data.GetLength(0);
            ulong height = (ulong)data.GetLength(1);
            ulong depth = (ulong)data.GetLength(2);
            ulong seg = (ulong)data.GetLength(3);

            buffer.Pack(ref width);
            buffer.Pack(ref height);
            buffer.Pack(ref depth);
            buffer.Pack(ref seg);

            if (width == 0 || height == 0)
            {
                return;
            }

            for (int x = 0; x < (int)width; x++)
                for (int y = 0; y < (int)height; y++)
                    for (int z = 0; z < (int)depth; z++)
                        for (int w = 0; w < (int)seg; w++)
                        {
                            RePacking.Pack(buffer, ref data[x, y, z, w]);
                        }
        }

        /// <summary>
        /// Unpacks a managed 4D Array
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackArray4D<T>(this ReBuffer buffer, out T[,,,] data)
        {
            buffer.Unpack<ulong>(out ulong width);
            buffer.Unpack<ulong>(out ulong height);
            buffer.Unpack<ulong>(out ulong depth);
            buffer.Unpack<ulong>(out ulong seg);

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
                            data[x, y, z, w] = RePacking.Unpack<T>(buffer);
                        }
        }
        #endregion

        #region IList
        /// <summary>
        /// Packs a managed IList
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackIList<T>(this ReBuffer buffer, IList<T> data)
        {
            if (data == null)
            {
                ulong zero = 0;
                buffer.Pack(ref zero);
                return;
            }

            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                ulong dataLen = (ulong)data.Count;
                buffer.Pack(ref dataLen);

                for (int i = 0; i < data.Count; i++)
                {
                    var ele = data[i];
                    packer.Pack(buffer, ref ele);
                }
            }
            else
            {
                throw new System.NotSupportedException($"Couldnt find packer for {typeof(T)}");
            }
        }

        /// <summary>
        /// Unpacks a managed IList
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackIList<T>(this ReBuffer buffer, out IList<T> data)
        {
            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                buffer.Unpack(out ulong len);

                data = new List<T>();
                for (int i = 0; i < (int)len; i++)
                {
                    packer.Unpack(buffer, out T value);
                    data.Add(value);
                }
            }
            else
            {
                throw new System.NotSupportedException($"Couldnt find unpacker for {typeof(T)}");
            }
        }

        /// <summary>
        /// Packs an unmanaged IList
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackIListBlittable<T>(this ReBuffer buffer, IList<T> data) where T : unmanaged
        {
            buffer.PackArray((T[])data);
        }

        /// <summary>
        /// Unpacks an unmanaged IListBlittable
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackIListBlittable<T>(this ReBuffer buffer, out IList<T> data) where T : unmanaged
        {
            data = buffer.UnpackArray<T>();
        }
        #endregion

        #region IEnumerable
        /// <summary>
        /// Packs a managed IEnumerable
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackIEnumerable<T>(this ReBuffer buffer, IEnumerable<T> data)
        {
            if (data == null || data.Count() == 0)
            {
                ulong zero = 0;
                buffer.Pack(ref zero);
                return;
            }

            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                ulong dataLen = (ulong)data.Count();
                buffer.Pack(ref dataLen);

                foreach (var element in data)
                {
                    var ele = element;
                    packer.Pack(buffer, ref ele);
                }
            }
            else
            {
                throw new System.NotSupportedException($"Couldnt find packer for {typeof(T)}");
            }
        }

        /// <summary>
        /// Unpacks a managed IEnumerable
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackIEnumerable<T>(this ReBuffer buffer, out IEnumerable<T> data)
        {
            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                buffer.Unpack(out ulong len);

                T[] temp_data = new T[(int)len];
                for (int i = 0; i < (int)len; i++)
                {
                    packer.Unpack(buffer, out temp_data[i]);
                }
                data = temp_data;
            }
            else
            {
                throw new System.NotSupportedException($"Couldnt find unpacker for {typeof(T)}");
            }
        }

        /// <summary>
        /// Packs an unmanaged IEnumerable
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackIEnumerableBlittable<T>(this ReBuffer buffer, IEnumerable<T> data) where T : unmanaged
        {
            buffer.PackArray((T[])data);
        }

        /// <summary>
        /// Unpacks an unmanaged IEnumerableBlittable
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackIEnumerableBlittable<T>(this ReBuffer buffer, out IEnumerable<T> data) where T : unmanaged
        {
            T[] temp = buffer.UnpackArray<T>();
            data = temp;
        }
        #endregion

        #region ICollection
        /// <summary>
        /// Packs a managed ICollection
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackICollection<T>(this ReBuffer buffer, ICollection<T> data)
        {
            if (data == null)
            {
                ulong zero = 0;
                buffer.Pack(ref zero);
                return;
            }

            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                ulong dataLen = (ulong)data.Count;
                buffer.Pack(ref dataLen);

                foreach (var d in data)
                {
                    var ele = d;
                    packer.Pack(buffer, ref ele);
                }
            }
            else
            {
                throw new System.NotSupportedException($"Couldnt find packer for {typeof(T)}");
            }
        }

        /// <summary>
        /// Unpacks a managed ICollection
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackICollection<T>(this ReBuffer buffer, out ICollection<T> data)
        {
            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                buffer.Unpack(out ulong len);

                data = new T[(int)len];
                for (int i = 0; i < (int)len; i++)
                {
                    packer.Unpack(buffer, out T value);
                    data.Add(value);
                }
            }
            else
            {
                throw new System.NotSupportedException($"Couldnt find unpacker for {typeof(T)}");
            }
        }

        /// <summary>
        /// Packs an unmanaged ICollection
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackICollectionBlittable<T>(this ReBuffer buffer, ICollection<T> data) where T : unmanaged
        {
            buffer.PackArray((T[])data);
        }

        /// <summary>
        /// Unpacks an unmanaged ICollectionBlittable
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackICollectionBlittable<T>(this ReBuffer buffer, out ICollection<T> data) where T : unmanaged
        {
            data = buffer.UnpackArray<T>();
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

        /// <summary>
        /// Packs a managed Queue
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackQueue<T>(this ReBuffer buffer, Queue<T> data)
        {
            PackIEnumerable<T>(buffer, data);
        }

        /// <summary>
        /// Packs a managed Stack
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackStack<T>(this ReBuffer buffer, Stack<T> data)
        {
            if (data == null)
            {
                ulong zero = 0;
                buffer.Pack(ref zero);
                return;
            }

            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                ulong dataLen = (ulong)data.Count;
                buffer.Pack(ref dataLen);

                for (int i = (int)dataLen - 1; i >= 0; i--)
                {
                    var ele = data.ElementAt(i);
                    packer.Pack(buffer, ref ele);
                }
            }
            else
            {
                throw new System.NotSupportedException($"Couldnt find packer for {typeof(T)}");
            }
        }

        /// <summary>
        /// Packs a managed HashSet
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void PackHashSet<T>(this ReBuffer buffer, HashSet<T> data)
        {
            PackIEnumerable<T>(buffer, data);
        }

        static void UnpackCollectionInternal<T>(this ReBuffer buffer, System.Action<T> predicate)
        {
            if (TypeCache.TryGetTypePacker<T>(out var packer))
            {
                buffer.Unpack(out ulong len);

                for (int i = 0; i < (int)len; i++)
                {
                    packer.Unpack(buffer, out var item);
                    predicate.Invoke(item);
                }
            }
            else
            {
                throw new System.NotSupportedException($"Couldnt find unpacker for {typeof(T)}");
            }
        }

        /// <summary>
        /// Unpacks a managed Queue
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackQueue<T>(this ReBuffer buffer, out Queue<T> data)
        {
            data = new Queue<T>();
            UnpackCollectionInternal<T>(buffer, data.Enqueue);
        }

        /// <summary>
        /// Unpacks a managed Stack
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackStack<T>(this ReBuffer buffer, out Stack<T> data)
        {
            data = new Stack<T>();
            UnpackCollectionInternal<T>(buffer, data.Push);
        }

        /// <summary>
        /// Unpacks a managed HashSet
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void UnpackHashSet<T>(this ReBuffer buffer, out HashSet<T> data)
        {
            data = new HashSet<T>();
            UnpackCollectionInternal<T>(buffer, data.HashSetAdd);
        }

        static void HashSetAdd<T>(this HashSet<T> self, T item)
        {
            self.Add(item);
        }

        /// <summary>
        /// Gives the size of an unmanaged collection of T
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>size of collection in bytes (N * sizeof(T))</returns>
        public static int SizeOfCollection<T>(this IEnumerator data)
        {
            int size = sizeof(ulong);
            if (data == null) return size;

            while (data.MoveNext())
            {
                var refable = (T)data.Current;
                size += RePacking.SizeOf(ref refable);
            }
            return size;
        }
    }
}
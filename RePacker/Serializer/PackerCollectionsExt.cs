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
        public static void PackArray<T>(this Buffer buffer, T[] data)
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

        public static void UnpackArray<T>(this Buffer buffer, out T[] data)
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

        public static void PackArray2D<T>(this Buffer buffer, ref T[,] data)
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

        public static void UnpackArray2D<T>(this Buffer buffer, out T[,] data)
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

        public static void PackArray3D<T>(this Buffer buffer, ref T[,,] data)
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

        public static void UnpackArray3D<T>(this Buffer buffer, out T[,,] data)
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

        public static void PackArray4D<T>(this Buffer buffer, ref T[,,,] data)
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

        public static void UnpackArray4D<T>(this Buffer buffer, out T[,,,] data)
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
        public static void PackIList<T>(this Buffer buffer, IList<T> data)
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

        public static void UnpackIList<T>(this Buffer buffer, out IList<T> data)
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

        public static void PackIListBlittable<T>(this Buffer buffer, IList<T> data) where T : unmanaged
        {
            buffer.PackArray((T[])data);
        }

        public static void UnpackIListBlittable<T>(this Buffer buffer, out IList<T> data) where T : unmanaged
        {
            data = buffer.UnpackArray<T>();
        }
        #endregion

        #region IEnumerable
        public static void PackIEnumerable<T>(this Buffer buffer, IEnumerable<T> data)
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

        public static void UnpackIEnumerable<T>(this Buffer buffer, out IEnumerable<T> data)
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

        public static void PackIEnumerableBlittable<T>(this Buffer buffer, IEnumerable<T> data) where T : unmanaged
        {
            buffer.PackArray((T[])data);
        }

        public static void UnpackIEnumerableBlittable<T>(this Buffer buffer, out IEnumerable<T> data) where T : unmanaged
        {
            T[] temp = buffer.UnpackArray<T>();
            data = temp;
        }
        #endregion

        #region ICollection
        public static void PackICollection<T>(this Buffer buffer, ICollection<T> data)
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

        public static void UnpackICollection<T>(this Buffer buffer, out ICollection<T> data)
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

        public static void PackICollectionBlittable<T>(this Buffer buffer, ICollection<T> data) where T : unmanaged
        {
            buffer.PackArray((T[])data);
        }

        public static void UnpackICollectionBlittable<T>(this Buffer buffer, out ICollection<T> data) where T : unmanaged
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

        public static void PackQueue<T>(this Buffer buffer, Queue<T> data)
        {
            PackIEnumerable<T>(buffer, data);
        }

        public static void PackStack<T>(this Buffer buffer, Stack<T> data)
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

        public static void PackHashSet<T>(this Buffer buffer, HashSet<T> data)
        {
            PackIEnumerable<T>(buffer, data);
        }

        static void UnpackCollectionInternal<T>(this Buffer buffer, System.Action<T> predicate)
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

        public static void UnpackQueue<T>(this Buffer buffer, out Queue<T> data)
        {
            data = new Queue<T>();
            UnpackCollectionInternal<T>(buffer, data.Enqueue);
        }

        public static void UnpackStack<T>(this Buffer buffer, out Stack<T> data)
        {
            data = new Stack<T>();
            UnpackCollectionInternal<T>(buffer, data.Push);
        }

        public static void UnpackHashSet<T>(this Buffer buffer, out HashSet<T> data)
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
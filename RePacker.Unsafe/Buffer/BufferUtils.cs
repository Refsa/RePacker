using System;
using System.Runtime.CompilerServices;
using RePacker.Unsafe;

namespace RePacker.Buffers
{
    public static class BufferUtils
    {
        /// <summary>
        /// Packs an unmanaged value type T<para />
        /// 
        /// Moves the write cursor for sizeof(T) bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value">value to apack</param>
        /// <exception cref="System.IndexOutOfRangeException">Thrown if buffer cant fit value T</exception>
        /// <typeparam name="T">unamanged value type</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Pack<T>(this ReBuffer buffer, ref T value)
            where T : unmanaged
        {
            if (!buffer.CanWrite<T>())
            {
                throw new IndexOutOfRangeException($"Trying to write type {typeof(T)} outside of range of buffer");
            }

            fixed (byte* data = &buffer.Array[buffer.WriteCursor()])
            {
                *((T*)data) = value;
            }

            buffer.MoveWriteCursor(sizeof(T));
        }

        /// <summary>
        /// Unpack an unmanaged value of type T<para />
        /// 
        /// Moves the read cursor for sizeof(T) bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value">out variable to hold the value T</param>
        /// <exception cref="System.IndexOutOfRangeException">Thrown if buffer cant read value T</exception>
        /// <typeparam name="T">unmanaged value type</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Unpack<T>(this ReBuffer buffer, out T value)
            where T : unmanaged
        {
            if (!buffer.CanRead<T>())
            {
                throw new IndexOutOfRangeException($"Trying to read type {typeof(T)} outside of range of buffer");
            }

            fixed (byte* data = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(T*)data;
            }

            buffer.MoveReadCursor(sizeof(T));
        }

        /// <summary>
        /// Returns a references to an unmanaged value type in the buffer at given byte offset<para />
        /// 
        /// To use the references, the caller needs to call it like this:<para />
        /// ref T thing = ref buffer.GetRef(...)<para />
        /// 
        /// You can then modify "thing" to change it's value in the buffer without repacking it<para />
        /// 
        /// Does not move the read/write cursor
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="byteOffset">Offset in bytes to read value reference from</param>
        /// <exception cref="System.IndexOutOfRangeException">Thrown if buffer cant read value T</exception>
        /// <typeparam name="T">unmanaged value type</typeparam>
        /// <returns>A reference to the wanted value type T</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static ref T GetRef<T>(this ReBuffer buffer, int byteOffset = 0)
            where T : unmanaged
        {
            if (buffer.ReadCursor() + byteOffset + sizeof(T) > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException($"Trying to read type {typeof(T)} outside of range of buffer");
            }

            fixed(byte* data = &buffer.Array[buffer.ReadCursor() + byteOffset])
            {
                T* t = (T*)data;

                return ref *t;
            }
        }

        /// <summary>
        /// Peeks an unmanaged value type T from the buffer<para />
        /// 
        /// Does not move the read/write cursor
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="byteOffset">offset in bytes to peek T from</param>
        /// <exception cref="System.IndexOutOfRangeException">Thrown if buffer cant read value T</exception>
        /// <typeparam name="T">an unmanaged value type</typeparam>
        /// <returns>unmanaged value type of T</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T Peek<T>(this ReBuffer buffer, int byteOffset = 0)
            where T : unmanaged
        {
            if (buffer.ReadCursor() + byteOffset + sizeof(T) > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException($"Trying to read type {typeof(T)} outside of range of buffer");
            }

            fixed (byte* data = &buffer.Array[buffer.ReadCursor() + byteOffset])
            {
                return *(T*)data;
            }
        }

        /// <summary>
        /// Directly copies an array of unmanaged value types T into buffer<para />
        /// 
        /// Moves write cursor for (sizeof(T) * N) bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="array">Array of elements to copy</param>
        /// <param name="offset">Offset into source array</param>
        /// <param name="length">Number of elements from source array</param>
        /// <exception cref="System.IndexOutOfRangeException">Thrown if buffer cant fit N elements of T</exception>
        /// <typeparam name="T">unmanaged value type</typeparam>
        public static unsafe void PackArray<T>(this ReBuffer buffer, T[] array, int offset = 0, int length = 0)
            where T : unmanaged
        {
            if (array == null || (length == 0 && array.Length == 0))
            {
                ulong zero = 0;
                buffer.Pack(ref zero);
                return;
            }
            if (length == 0) length = array.Length;

            if (!buffer.CanWrite<T>(length))
            {
                throw new IndexOutOfRangeException();
            }

            ulong len = (ulong)length;
            buffer.Pack(ref len);

            int pos = buffer.WriteCursor();
            int size = UnsafeUtils.SizeOf<T>();

            fixed (void* src = &array[offset], dest = &buffer.Array[pos])
            {
                System.Buffer.MemoryCopy(src, dest, length * size, length * size);
            }

            buffer.MoveWriteCursor(length * size);
        }

        /// <summary>
        /// Attempts to unpack an array that was packed with ReBuffer::PackArray<para />
        /// 
        /// Will return an empty array of T if no array was recognized on the buffer<para />
        /// 
        /// Moves read cursor equal to (sizeof(T) * N) bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <typeparam name="T">unmanaged value type</typeparam>
        /// <returns>An array of T*N elements, or an empty array of T</returns>
        public static unsafe T[] UnpackArray<T>(this ReBuffer buffer)
            where T : unmanaged
        {
            buffer.Unpack(out ulong len);

            if (len == 0 || !buffer.CanRead<T>((int)len))
            {
                return new T[0];
            }

            T[] destArray = new T[(int)len];
            int size = UnsafeUtils.SizeOf<T>();

            fixed (void* src = &buffer.Array[buffer.ReadCursor()], dest = destArray)
            {
                System.Buffer.MemoryCopy(src, dest, len * (ulong)size, len * (ulong)size);
            }

            buffer.MoveReadCursor(size * (int)len);

            return destArray;
        }

        /// <summary>
        /// Packs given buffer into this buffer<para />
        /// 
        /// Moves read cursor for (16 + other.Length()) bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="other">Other buffer to pack into this one</param>
        /// <exception cref="System.IndexOutOfRangeException">Thrown if buffer cant fit (8 + N) bytes</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackBuffer(this ReBuffer buffer, ReBuffer other)
        {
            int readCursor = other.ReadCursor();
            int writeCursor = other.WriteCursor();
            buffer.Pack(ref readCursor);
            buffer.Pack(ref writeCursor);
            buffer.PackArray(other.Array, other.ReadCursor(), other.Length());
        }

        /// <summary>
        /// Attempts to unpack a ReBuffer that was packed with ReBuffer::PackBuffer<para />
        /// 
        /// Will return an empty buffer if no buffer was found<para />
        /// 
        /// Moves read cursor for (16 + buffer.Length) bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>A ReBuffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReBuffer UnpackBuffer(this ReBuffer buffer)
        {
            buffer.Unpack(out int readCursor);
            buffer.Unpack(out int writeCursor);

            var value = new ReBuffer(buffer.UnpackArray<byte>());
            value.SetReadCursor(readCursor);
            value.SetWriteCursor(writeCursor);

            return value;
        }
    }
}
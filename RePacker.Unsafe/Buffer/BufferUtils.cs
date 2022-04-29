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
        /// Moves the write cursor for sizeof(T) bytes<para />
        /// 
        /// changes endianness
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

            buffer.EnsureEndianness(ref value);
            buffer.Blob.Write(buffer.WriteCursor(), value);
            buffer.MoveWriteCursor(sizeof(T));
        }

        /// <summary>
        /// Unpack an unmanaged value of type T<para />
        /// 
        /// Moves the read cursor for sizeof(T) bytes<para />
        /// 
        /// changes endianness
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

            value = buffer.Blob.Read<T>(buffer.ReadCursor());
            buffer.EnsureEndianness(ref value);
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
        /// Does not move the read/write cursor<para />
        /// 
        /// Does not change endianness
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
            if (buffer.ReadCursor() + byteOffset + sizeof(T) > buffer.Blob.Capacity)
            {
                throw new IndexOutOfRangeException($"Trying to read type {typeof(T)} outside of range of buffer");
            }

            return ref buffer.Blob.GetRef<T>(buffer.ReadCursor() + byteOffset);
        }

        /// <summary>
        /// Peeks an unmanaged value type T from the buffer<para />
        /// 
        /// Does not move the read/write cursor<para />
        /// 
        /// Changes endianness
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
            if (buffer.ReadCursor() + byteOffset + sizeof(T) > buffer.Blob.Capacity)
            {
                throw new IndexOutOfRangeException($"Trying to read type {typeof(T)} outside of range of buffer");
            }

            var value = buffer.Blob.Read<T>(buffer.ReadCursor() + byteOffset);
            buffer.EnsureEndianness(ref value);
            return value;
        }

        /// <summary>
        /// Directly copies an array of unmanaged value types T into buffer<para />
        /// 
        /// Moves write cursor for (sizeof(T) * N) bytes<para />
        /// 
        /// does not change endianness
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
            buffer.Blob.Append(array, offset, pos, length);
            buffer.MoveWriteCursor(length * sizeof(T));
        }

        /// <summary>
        /// Attempts to unpack an array that was packed with ReBuffer::PackArray<para />
        /// 
        /// Will return an empty array of T if no array was recognized on the buffer<para />
        /// 
        /// Moves read cursor equal to (sizeof(T) * N) bytes<para />
        /// 
        /// Does not change endianness
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
            buffer.Blob.CopyTo(destArray, buffer.ReadCursor(), (int)len);
            buffer.MoveReadCursor((int)len * sizeof(T));

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
            var readCursor = other.ReadCursor();
            var writeCursor = other.WriteCursor();
            var endianness = other.Endianness;

            buffer.Pack(ref readCursor);
            buffer.Pack(ref writeCursor);
            buffer.Pack(ref endianness);

            var length = (ulong)other.Length();
            buffer.Pack(ref length);

            other.Blob.CopyTo(ref buffer.Blob, other.Length(), other.ReadCursor(), buffer.WriteCursor());
            buffer.MoveWriteCursor(other.Length());
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
            buffer.Unpack(out Endianness endianness);

            buffer.Unpack(out ulong length);
            var dest = new ReBuffer((int)length);

            buffer.Blob.CopyTo(ref dest.Blob, (int)length, buffer.ReadCursor(), 0);

            dest.SetReadCursor(readCursor);
            dest.SetWriteCursor(writeCursor);
            dest.SetEndianness(endianness);

            return dest;
        }
    }
}
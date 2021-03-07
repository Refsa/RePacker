using System;
using System.Runtime.CompilerServices;
using RePacker.Unsafe;

namespace RePacker.Buffers
{
    public static class BufferUtils
    {
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T Peek<T>(this ReBuffer buffer, int offset = 0)
            where T : unmanaged
        {
            if (buffer.ReadCursor() + offset + sizeof(T) > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException($"Trying to read type {typeof(T)} outside of range of buffer");
            }

            fixed (byte* data = &buffer.Array[buffer.ReadCursor() + offset])
            {
                return *(T*)data;
            }
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackBuffer(this ReBuffer buffer, ReBuffer other)
        {
            int readCursor = other.ReadCursor();
            int writeCursor = other.WriteCursor();
            buffer.Pack(ref readCursor);
            buffer.Pack(ref writeCursor);
            buffer.PackArray(other.Array, other.ReadCursor(), other.Length());
        }

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
using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;
using RePacker.Utils;
using RePacker.Unsafe;

namespace RePacker.Buffers
{
    public class Buffer
    {
        byte[] array;
        public byte[] Array => array;

        int writeCursor;
        int readCursor;

        public Buffer(int size)
        {
            this.array = new byte[size];
            this.writeCursor = 0;
            this.readCursor = 0;
        }

        public Buffer(byte[] buffer, int index = 0, int offset = 0)
        {
            this.writeCursor = offset;
            this.readCursor = 0;

            this.array = buffer;

            if (this.array == null)
            {
                throw new ArgumentNullException("Array on Buffer is null");
            }
        }

        public Buffer (Buffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("Buffer is null");
            }

            this.writeCursor = buffer.writeCursor;
            this.readCursor = buffer.readCursor;

            this.array = buffer.array;

            if (this.array == null)
            {
                throw new ArgumentNullException("Array on Buffer is null");
            }
        }

        public unsafe void Copy(Buffer destination)
        {
            int length = Length();
            if (!destination.CanWriteBytes(length))
            {
                throw new IndexOutOfRangeException("Cant copy Buffer, destination too small");
            }

            fixed (void* src = &array[readCursor], dest = &destination.array[destination.writeCursor])
            {
                System.Buffer.MemoryCopy(src, dest, length, length);
            }

            destination.MoveWriteCursor(length);
        }

        #region General
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            Reset();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            readCursor = 0;
            writeCursor = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int WriteCursor()
        {
            return writeCursor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Length()
        {
            return writeCursor - readCursor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadCursor()
        {
            return readCursor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FreeSpace()
        {
            return array.Length - writeCursor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetReadCursor(int pos)
        {
            readCursor = pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetWriteCursor(int pos)
        {
            writeCursor = pos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveWriteCursor(int amount)
        {
            writeCursor += amount;

            if (writeCursor > array.Length || writeCursor < 0)
            {
                writeCursor -= amount;
                throw new IndexOutOfRangeException("Trying to move write cursor outside of buffer range");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveReadCursor(int amount)
        {
            readCursor += amount;

            if (readCursor > array.Length || readCursor < 0)
            {
                readCursor -= amount;
                throw new IndexOutOfRangeException("Trying to move read cursor outside of buffer range");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanWriteBytes(int count)
        {
            return writeCursor + count <= array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanReadBytes(int count)
        {
            return readCursor + count <= array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanWrite<T>(int count = 1) where T : unmanaged
        {
            return (writeCursor + (count * UnsafeUtils.SizeOf<T>())) <= array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanRead<T>(int count = 1) where T : unmanaged
        {
            return (readCursor + (count * UnsafeUtils.SizeOf<T>())) <= array.Length;
        }
        #endregion

        #region Size
        unsafe void Expand()
        {
            const float GOLDEN_RATIO = 1.618f;

            int newSize = (int)((float)array.Length * GOLDEN_RATIO);
            byte[] newBuffer = new byte[newSize];

            fixed (void* src = array, dest = newBuffer)
            {
                System.Buffer.MemoryCopy(src, dest, writeCursor, writeCursor);
            }

            array = newBuffer;
        }
        #endregion
    }
}
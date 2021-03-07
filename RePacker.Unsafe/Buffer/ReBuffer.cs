using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;
using RePacker.Utils;
using RePacker.Unsafe;

namespace RePacker.Buffers
{
    public class ReBuffer
    {
        byte[] array;
        public byte[] Array => array;

        int writeCursor;
        int readCursor;

        bool expand;

        public ReBuffer(int size, bool expand = false)
        {
            this.array = new byte[size];
            this.writeCursor = 0;
            this.readCursor = 0;
            this.expand = expand;
        }

        public ReBuffer(byte[] buffer, int offset = 0, bool expand = false)
        {
            this.writeCursor = offset;
            this.readCursor = 0;

            this.array = buffer;
            this.expand = expand;

            if (this.array == null)
            {
                throw new ArgumentNullException("Array on Buffer is null");
            }
        }

        public ReBuffer(ReBuffer buffer)
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

        public unsafe void Copy(ReBuffer source)
        {
            int length = source.writeCursor;
            if (!CanWriteBytes(length))
            {
                throw new IndexOutOfRangeException("Cant copy Buffer, destination too small");
            }

            fixed (void* src = &source.array[readCursor], dest = &array[writeCursor])
            {
                System.Buffer.MemoryCopy(src, dest, length, length);
            }

            MoveWriteCursor(length);
        }

        public unsafe ReBuffer Clone()
        {
            var buffer = new ReBuffer(writeCursor);

            fixed (void* src = &array[readCursor], dest = buffer.array)
            {
                System.Buffer.MemoryCopy(src, dest, writeCursor, writeCursor);
            }

            buffer.writeCursor = writeCursor;

            return buffer;
        }

        #region General
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            for (int i = 0; i < writeCursor; i++)
            {
                array[i] = 0;
            }
            Reset();
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
            if (writeCursor + count > array.Length)
            {
                if (!expand) return false;

                Expand<byte>(count);
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanReadBytes(int count)
        {
            return readCursor + count <= array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool CanWrite<T>(int count = 1) where T : unmanaged
        {
            if ((writeCursor + (count * sizeof(T))) > array.Length)
            {
                if (!expand) return false;

                Expand<T>();
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool CanRead<T>(int count = 1) where T : unmanaged
        {
            return (readCursor + (count * sizeof(T))) <= array.Length;
        }
        #endregion

        #region Size
        unsafe void Expand<T>(int count = 1) where T : unmanaged
        {
            byte[] newBuffer = new byte[writeCursor + sizeof(T)];

            if (writeCursor > 0)
            {
                fixed (void* src = array, dest = newBuffer)
                {
                    System.Buffer.MemoryCopy(src, dest, writeCursor, writeCursor);
                }
            }

            array = newBuffer;
        }
        #endregion
    }
}
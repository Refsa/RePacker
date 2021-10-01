using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;
using RePacker.Utils;
using RePacker.Unsafe;

namespace RePacker.Buffers
{
    public enum Endianness : byte
    {
        BigEndian = 0,
        LittleEndian,
    }

    public class ReBuffer
    {
        static Endianness DefaultEndianness = BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;

        byte[] array;
        public byte[] Array => array;

        int writeCursor;
        int readCursor;
        Endianness endianness;

        bool expand;

        public Endianness Endianness => endianness;
        public void SetEndianness(Endianness endianness) => this.endianness = endianness;

        /// <summary>
        /// Create a new ReBuffer
        /// </summary>
        /// <param name="size">initial capacity of buffer</param>
        /// <param name="expand">enable auto-expansion of buffer to fit elements</param>
        public ReBuffer(int size, bool expand = false)
        {
            this.array = new byte[size];
            this.writeCursor = 0;
            this.readCursor = 0;
            this.expand = expand;

            this.endianness = DefaultEndianness;
        }

        /// <summary>
        /// Create a ReBuffer from the given byte array
        /// </summary>
        /// <param name="buffer">array to wrap</param>
        /// <param name="offset">initial position of write cursor</param>
        /// <param name="expand">enable auto-expansion of buffer to fit elements</param>
        public ReBuffer(byte[] buffer, int offset = 0, bool expand = false)
        {
            this.writeCursor = offset;
            this.readCursor = 0;

            this.array = buffer;
            this.expand = expand;

            this.endianness = DefaultEndianness;

            if (this.array == null)
            {
                throw new ArgumentNullException("Array on Buffer is null");
            }
        }

        /// <summary>
        /// Shallow copy of given buffer
        /// </summary>
        /// <param name="buffer">Buffer to get a shallow copy of</param>
        public ReBuffer(ReBuffer buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("Buffer is null");
            }

            this.writeCursor = buffer.writeCursor;
            this.readCursor = buffer.readCursor;

            this.array = buffer.array;
            this.endianness = buffer.endianness;

            if (this.array == null)
            {
                throw new ArgumentNullException("Array on Buffer is null");
            }
        }

        /// <summary>
        /// Copies the given buffer, appending it to the end of this one
        /// 
        /// Requires that this buffer has enough space available
        /// </summary>
        /// <param name="source">Buffer to copy from</param>
        public unsafe void Copy(ReBuffer source)
        {
            int length = source.Length();
            if (!CanWriteBytes(length))
            {
                throw new IndexOutOfRangeException("Cant copy Buffer, destination too small");
            }

            //System.Buffer.BlockCopy(source.array, source.readCursor, array, writeCursor, length);

            fixed (void* src = &source.array[source.readCursor], dest = &array[writeCursor])
            {
                System.Buffer.MemoryCopy(src, dest, length, length);
            }

            MoveWriteCursor(length);
        }

        /// <summary>
        /// Clones buffer into a new buffer, only copying the used space
        /// </summary>
        /// <returns>A new buffer with the used space of this buffer</returns>
        public unsafe ReBuffer Clone()
        {
            var buffer = new ReBuffer(Length());

            fixed (void* src = &array[readCursor], dest = buffer.array)
            {
                System.Buffer.MemoryCopy(src, dest, writeCursor, writeCursor);
            }

            buffer.writeCursor = writeCursor;

            return buffer;
        }

        /// <summary>
        /// Shrinks internal array to only contain contents between read and write cursor
        /// </summary>
        public void ShrinkFit()
        {
            var newArray = new byte[Length()];
            System.Buffer.BlockCopy(array, readCursor, newArray, 0, Length());
            array = newArray;

            writeCursor -= readCursor;
            readCursor = 0;
        }

        #region General
        /// <summary>
        /// Clears read/write cursor and zeroes out internal byte array
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            for (int i = 0; i < writeCursor; i++)
            {
                array[i] = 0;
            }
            Reset();
        }

        /// <summary>
        /// Clears read/write cursor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            readCursor = 0;
            writeCursor = 0;
        }

        /// <summary>
        /// Gives the current position of write cursor
        /// </summary>
        /// <returns>current position of write cursor</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int WriteCursor()
        {
            return writeCursor;
        }

        /// <summary>
        /// Gives the current position of read cursor
        /// </summary>
        /// <returns>current position of read cursor</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadCursor()
        {
            return readCursor;
        }

        /// <summary>
        /// Gives the actively used length of the buffer<para />
        /// 
        /// Equal to (writeCursor - readCursor)
        /// </summary>
        /// <returns>actively used length of buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Length()
        {
            return writeCursor - readCursor;
        }
        
        /// <summary>
        /// Gives the amount of space left in buffer<para />
        /// 
        /// Equal to (internalArray.Length - writeCursor)
        /// </summary>
        /// <returns>amount of space left in buffer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FreeSpace()
        {
            return array.Length - writeCursor;
        }

        /// <summary>
        /// Directly sets the position of the read cursor
        /// </summary>
        /// <param name="pos">wanted position for the read cursor</param>
        /// <exception cref="System.IndexOutOfRangeException" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetReadCursor(int pos)
        {
            if (pos > array.Length || pos < 0)
            {
                throw new IndexOutOfRangeException("Trying to set read cursor outside of buffer range");
            }

            readCursor = pos;
        }

        /// <summary>
        /// Directly sets the position of the write cursor
        /// </summary>
        /// <param name="pos">wanted position for the write cursor</param>
        /// <exception cref="System.IndexOutOfRangeException" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetWriteCursor(int pos)
        {
            if (pos > array.Length || pos < 0)
            {
                throw new IndexOutOfRangeException("Trying to set write cursor outside of buffer range");
            }

            writeCursor = pos;
        }

        /// <summary>
        /// Moves write cursor by given amount
        /// </summary>
        /// <param name="amount">amount in bytes to move write cursor</param>
        /// <exception cref="System.IndexOutOfRangeException" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveWriteCursor(int amount)
        {
            if (writeCursor + amount > array.Length || writeCursor < 0)
            {
                throw new IndexOutOfRangeException("Trying to move write cursor outside of buffer range");
            }

            writeCursor += amount;
        }

        /// <summary>
        /// Moves read cursor by given amount
        /// </summary>
        /// <param name="amount">amount in bytes to move read cursor</param>
        /// <exception cref="System.IndexOutOfRangeException" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveReadCursor(int amount)
        {
            if (readCursor + amount > array.Length || readCursor < 0)
            {
                throw new IndexOutOfRangeException("Trying to move read cursor outside of buffer range");
            }

            readCursor += amount;
        }

        /// <summary>
        /// Checks if buffer can fit given amount of bytes<para />
        /// 
        /// Will expand buffer if auto-size is enabled
        /// </summary>
        /// <param name="byteCount">bytes to move write cursor by</param>
        /// <returns>true if it's possible to write N bytes</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanWriteBytes(int byteCount)
        {
            if (writeCursor + byteCount > array.Length)
            {
                if (!expand) return false;

                Expand<byte>(byteCount);
            }

            return true;
        }

         /// <summary>
        /// Checks if you can read given amount of bytes from buffer<para />
        /// </summary>
        /// <param name="byteCount">bytes to move read cursor by</param>
        /// <returns>true if it's possible to read N bytes</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanReadBytes(int byteCount)
        {
            return readCursor + byteCount <= array.Length;
        }

        /// <summary>
        /// Checks if buffer can fit given amount of value type T<para />
        /// 
        /// Will expand buffer if auto-size is enabled
        /// </summary>
        /// <param name="count">number of elements of T to check for</param>
        /// <typeparam name="T">unmanaged value type</typeparam>
        /// <returns>true if it can fit N elements of T</returns>
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

        /// <summary>
        /// Checks if you can read N elements of unmanaged value type T
        /// </summary>
        /// <param name="count">number of elements</param>
        /// <typeparam name="T">unmanaged value type</typeparam>
        /// <returns>true if it's possible to read N elements of T</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool CanRead<T>(int count = 1) where T : unmanaged
        {
            return (readCursor + (count * sizeof(T))) <= array.Length;
        }
        #endregion

        #region Size
        unsafe void Expand<T>(int count = 1) where T : unmanaged
        {
            byte[] newBuffer = new byte[writeCursor + sizeof(T) * count];

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
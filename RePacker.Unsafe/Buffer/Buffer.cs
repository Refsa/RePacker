using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;
using RePacker.Utils;
using RePacker.Unsafe;

namespace RePacker.Buffers
{
    public struct Buffer
    {
        byte[] array;
        public byte[] Array => array;

        int writeCursor;
        int readCursor;
        public int Index;

        public Buffer(byte[] buffer, int index = 0, int offset = 0)
        {
            Index = index;

            this.writeCursor = offset;
            this.readCursor = 0;

            this.array = buffer;

            if (this.array == null)
            {
                throw new ArgumentNullException("Array on Buffer is null");
            }
        }

        public Buffer(ref Buffer buffer)
        {
            this.Index = buffer.Index;
            this.writeCursor = buffer.writeCursor;
            this.readCursor = buffer.readCursor;

            this.array = buffer.array;

            if (this.array == null)
            {
                throw new ArgumentNullException("Array on Buffer is null");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Pack<T>(ref T value) where T : unmanaged
        {
            // int size = UnsafeUtils.SizeOf<T>();

            if (writeCursor + sizeof(T) > array.Length)
            {
                throw new IndexOutOfRangeException($"Trying to write type {typeof(T)} outside of range of buffer");
            }

            fixed (byte* data = &array[writeCursor])
            {
                *((T*)data) = value;
            }
            writeCursor += sizeof(T);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Unpack<T>(out T value) where T : unmanaged
        {
            // int size = UnsafeUtils.SizeOf<T>();

            if (readCursor + sizeof(T) > array.Length)
            {
                throw new IndexOutOfRangeException($"Trying to read type {typeof(T)} outside of range of buffer");
            }

            fixed (byte* data = &array[readCursor])
            {
                value = *(T*)data;
            }
            readCursor += sizeof(T);
        }

        public unsafe void MemoryCopyFromUnsafe<T>(T[] array) where T : unmanaged
        {
            if (array == null || array.Length == 0)
            {
                ulong zero = 0;
                PushULong(ref zero);
                return;
            }

            if (!CanFit<T>(array.Length))
            {
                throw new IndexOutOfRangeException();
            }

            ulong len = (ulong)array.Length;
            PushULong(ref len);

            int pos = WriteCursor();
            int size = UnsafeUtils.SizeOf<T>();

            fixed (void* src = array, dest = &Array[pos])
            {
                System.Buffer.MemoryCopy(src, dest, array.Length * size, array.Length * size);
            }

            /* fixed (T* src = array) fixed (byte* dest = &data[pos])
            {
                for (int i = 0; i < array.Length; i++)
                {
                    *((T*)(dest + i * size)) = *(src + i);
                }
            } */

            MoveWriteCursor(array.Length * size);
        }

        public unsafe T[] MemoryCopyToUnsafe<T>() where T : unmanaged
        {
            PopULong(out ulong len);

            T[] destArray = new T[(int)len];

            if (len == 0)
            {
                return destArray;
            }

            int pos = ReadCursor();
            int size = UnsafeUtils.SizeOf<T>();

            fixed (void* src = &array[pos], dest = destArray)
            {
                System.Buffer.MemoryCopy(src, dest, len * (ulong)size, len * (ulong)size);
            }

            /* fixed (byte* src = &data[pos]) fixed (T* dest = destArray)
            {
                for (int i = 0; i < (int)len; i++)
                {
                    *(dest + i) = *(T*)(src + i * size);
                }
            } */

            MoveReadCursor(size * (int)len);

            return destArray;
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
        public void SetReadCursor(int count)
        {
            writeCursor = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveWriteCursor(int amount)
        {
            writeCursor += amount;

            if (writeCursor >= array.Length || writeCursor < 0)
            {
                writeCursor -= amount;
                throw new IndexOutOfRangeException("Trying to move write cursor outside of buffer range");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveReadCursor(int amount)
        {
            readCursor += amount;

            if (readCursor >= array.Length || readCursor < 0)
            {
                readCursor -= amount;
                throw new IndexOutOfRangeException("Trying to move read cursor outside of buffer range");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFitBytes(int count)
        {
            return writeCursor + count <= array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFit<T>(int count = 1) where T : unmanaged
        {
            return (writeCursor + (count * UnsafeUtils.SizeOf<T>())) <= array.Length;
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

        #region DirectPacking
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushBool(ref bool value)
        {
            array[writeCursor++] = value ? (byte)1 : (byte)0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopBool(out bool value)
        {
            byte val = array[readCursor++];
            value = val == 0 ? false : true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushShort(ref short value)
        {
            if (writeCursor + 2 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write short outside of buffer range");
            }

            fixed (byte* val = &array[writeCursor])
            {
                *((short*)val) = value;
            }
            writeCursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopShort(out short value)
        {
            if (readCursor + 2 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read short outside of buffer range");
            }

            fixed (byte* val = &array[readCursor])
            {
                value = *(short*)val;
            }
            readCursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushUShort(ref ushort value)
        {
            if (writeCursor + 2 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write ushort outside of buffer range");
            }

            fixed (byte* val = &array[writeCursor])
            {
                *((ushort*)val) = value;
            }
            writeCursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopUShort(out ushort value)
        {
            if (readCursor + 2 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read ushort outside of buffer range");
            }

            fixed (byte* val = &array[readCursor])
            {
                value = *(ushort*)val;
            }
            readCursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushInt(ref int value)
        {
            if (writeCursor + 4 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write int outside of buffer range");
            }

            fixed (byte* val = &array[writeCursor])
            {
                *((int*)val) = value;
            }
            writeCursor += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopInt(out int value)
        {
            if (readCursor + 4 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read int outside of buffer range");
            }

            fixed (byte* val = &array[readCursor])
            {
                value = *(int*)val;
            }
            readCursor += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushUInt(ref uint value)
        {
            if (writeCursor + 4 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write uint outside of buffer range");
            }

            fixed (byte* val = &array[writeCursor])
            {
                *((uint*)val) = value;
            }
            writeCursor += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopUInt(out uint value)
        {
            if (readCursor + 4 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read uint outside of buffer range");
            }

            fixed (byte* val = &array[readCursor])
            {
                value = *(uint*)val;
            }
            readCursor += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushLong(ref long value)
        {
            if (writeCursor + 8 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write long outside of buffer range");
            }

            fixed (byte* val = &array[writeCursor])
            {
                *((long*)val) = value;
            }
            writeCursor += 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopLong(out long value)
        {
            if (readCursor + 8 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read long outside of buffer range");
            }

            fixed (byte* val = &array[readCursor])
            {
                value = *(long*)val;
            }
            readCursor += 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushULong(ref ulong value)
        {
            if (writeCursor + 8 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write ulong outside of buffer range");
            }

            fixed (byte* val = &array[writeCursor])
            {
                *((ulong*)val) = value;
            }
            writeCursor += 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopULong(out ulong value)
        {
            if (readCursor + 8 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read ulong outside of buffer range");
            }

            fixed (byte* val = &array[readCursor])
            {
                value = *(ulong*)val;
            }
            readCursor += 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushChar(ref char value)
        {
            if (writeCursor + 2 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write char outside of buffer range");
            }

            fixed (byte* val = &array[writeCursor])
            {
                *((char*)val) = value;
            }
            writeCursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopChar(out char value)
        {
            if (readCursor + 2 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read char outside of buffer range");
            }

            fixed (byte* val = &array[readCursor])
            {
                value = *(char*)val;
            }
            readCursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushByte(ref byte value)
        {
            array[writeCursor++] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopByte(out byte value)
        {
            value = array[readCursor++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushSByte(ref sbyte value)
        {
            array[writeCursor++] = (byte)value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PopSByte(out sbyte value)
        {
            value = (sbyte)array[readCursor++];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushFloat(ref float value)
        {
            if (writeCursor + 4 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write float outside of buffer range");
            }

            fixed (byte* buf = &array[writeCursor])
            {
                *(float*)buf = value;
            }
            writeCursor += 4;

            // IEEE-like impl
            /* float source = value;

            bool isNegative = value < 0f;
            if (isNegative) value = -value;

            int exp = 0;
            if (value == 0f)
            {
                exp = 0;
            }
            else
            {
                source = MathExt.LDExp(MathExt.FRExp(source, out exp), 24);
                source += 126;
            }

            int mantissa = (int)source;

            buffer.Span[writeCursor++] = (byte)((isNegative ? 0x80 : 0x00) | (exp >> 1));
            buffer.Span[writeCursor++] = (byte)(((exp << 7) & 0x80) | ((mantissa >> 16) & 0x7F));
            buffer.Span[writeCursor++] = (byte)(mantissa >> 7);
            buffer.Span[writeCursor++] = (byte)(mantissa); */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopFloat(out float value)
        {
            if (readCursor + 4 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read float outside of buffer range");
            }

            value = 0;

            fixed (byte* buf = &array[readCursor])
            {
                value = *(float*)buf;
            }

            readCursor += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushDouble(ref double value)
        {
            if (writeCursor + 8 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write double outside of buffer range");
            }

            fixed (byte* buf = &array[writeCursor])
            {
                *(double*)buf = value;
            }
            writeCursor += 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopDouble(out double value)
        {
            if (readCursor + 8 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read double outside of buffer range");
            }

            value = 0;

            fixed (byte* buf = &array[readCursor])
            {
                value = *(double*)buf;
            }

            readCursor += 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushDecimal(ref decimal value)
        {
            if (writeCursor + 24 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write decimal outside of buffer range");
            }

            fixed (byte* buf = &array[writeCursor])
            {
                *(decimal*)buf = value;
            }
            writeCursor += 24;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopDecimal(out decimal value)
        {
            if (readCursor + 24 > array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read decimal outside of buffer range");
            }

            value = 0;

            fixed (byte* buf = &array[readCursor])
            {
                value = *(decimal*)buf;
            }

            readCursor += 24;
        }
        #endregion
    }
}
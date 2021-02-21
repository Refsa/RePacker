using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;
using RePacker.Utils;

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
        }

        public Buffer(ref Buffer buffer)
        {
            this.Index = buffer.Index;
            this.writeCursor = buffer.writeCursor;
            this.readCursor = buffer.readCursor;

            this.array = buffer.array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Pack<T>(ref T value) where T : unmanaged
        {
            fixed (byte* data = &array[writeCursor])
            {
                *((T*)data) = value;
            }
            writeCursor += sizeof(T);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Unpack<T>(out T value) where T : unmanaged
        {
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
            int size = Marshal.SizeOf<T>();

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

            int size = Marshal.SizeOf<T>();
            int pos = ReadCursor();

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
        public void Reset()
        {
            readCursor = 0;
            writeCursor = 0;
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
            // TODO: Make sure cursor doesnt move outside of buffer
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveReadCursor(int amount)
        {
            readCursor += amount;
            // TODO: Make sure cursor doesnt move outside of buffer
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFit(int count)
        {
            return writeCursor + count <= array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFit<T>(int count)
        {
            return writeCursor + (count * Marshal.SizeOf<T>()) <= array.Length;
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
            fixed (byte* val = &array[writeCursor])
            {
                *((short*)val) = value;
            }
            writeCursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopShort(out short value)
        {
            fixed (byte* val = &array[readCursor])
            {
                value = *(short*)val;
            }
            readCursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushUShort(ref ushort value)
        {
            fixed (byte* val = &array[writeCursor])
            {
                *((ushort*)val) = value;
            }
            writeCursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopUShort(out ushort value)
        {
            fixed (byte* val = &array[readCursor])
            {
                value = *(ushort*)val;
            }
            readCursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushInt(ref int value)
        {
            fixed (byte* val = &array[writeCursor])
            {
                *((int*)val) = value;
            }
            writeCursor += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopInt(out int value)
        {
            fixed (byte* val = &array[readCursor])
            {
                value = *(int*)val;
            }
            readCursor += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushUInt(ref uint value)
        {
            fixed (byte* val = &array[writeCursor])
            {
                *((uint*)val) = value;
            }
            writeCursor += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopUInt(out uint value)
        {
            fixed (byte* val = &array[readCursor])
            {
                value = *(uint*)val;
            }
            readCursor += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushLong(ref long value)
        {
            fixed (byte* val = &array[writeCursor])
            {
                *((long*)val) = value;
            }
            writeCursor += 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopLong(out long value)
        {
            fixed (byte* val = &array[readCursor])
            {
                value = *(long*)val;
            }
            readCursor += 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushULong(ref ulong value)
        {
            fixed (byte* val = &array[writeCursor])
            {
                *((ulong*)val) = value;
            }
            writeCursor += 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopULong(out ulong value)
        {
            fixed (byte* val = &array[readCursor])
            {
                value = *(ulong*)val;
            }
            readCursor += 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PushChar(ref char value)
        {
            fixed (byte* val = &array[writeCursor])
            {
                *((char*)val) = value;
            }
            writeCursor += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopChar(out char value)
        {
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
            fixed (byte* buf = &array[writeCursor])
            {
                *(double*)buf = value;
            }
            writeCursor += 8;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopDouble(out double value)
        {
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
            fixed (byte* buf = &array[writeCursor])
            {
                *(decimal*)buf = value;
            }
            writeCursor += 24;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void PopDecimal(out decimal value)
        {
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
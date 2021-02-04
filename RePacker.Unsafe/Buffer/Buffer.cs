using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;
using Refsa.RePacker.Utils;

namespace Refsa.RePacker.Buffers
{
    public struct Buffer
    {
        Memory<byte> buffer;
        int writeCursor;
        int readCursor;
        public int Index;

        public Buffer(Memory<byte> buffer, int index = 0, int offset = 0)
        {
            this.buffer = buffer;
            Index = index;

            this.writeCursor = offset;
            this.readCursor = 0;
        }

        public Buffer(ref Buffer buffer)
        {
            this.buffer = buffer.buffer;
            this.Index = buffer.Index;
            this.writeCursor = buffer.writeCursor;
            this.readCursor = buffer.readCursor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer Push<T>(ref T value) where T : unmanaged
        {
            int size = Marshal.SizeOf<T>();
            var span = buffer.Span.Slice(writeCursor, size);
            MemoryMarshal.Write(span, ref value);
            writeCursor += size;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer Push<T>(ref T value, int index) where T : unmanaged
        {
            int size = Marshal.SizeOf<T>();
            var span = buffer.Span.Slice(index, size);
            MemoryMarshal.Write(span, ref value);

            return this;
            // TODO: Make sure offset aligns with new length
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer Pop<T>(out T value) where T : unmanaged
        {
            int size = Marshal.SizeOf<T>();
            value = MemoryMarshal.Read<T>(buffer.Span.Slice(readCursor, size));
            readCursor += size;

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ReadOnlySpan<byte> data)
        {
            var span = buffer.Span.Slice(writeCursor, data.Length);
            data.CopyTo(span);
            writeCursor += data.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(Memory<byte> data)
        {
            var mem = buffer.Slice(writeCursor, data.Length);
            data.CopyTo(mem);
            writeCursor += data.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(Buffer data)
        {
            int len = data.Length();

            var mem = buffer.Slice(writeCursor, len);
            data.Read().CopyTo(mem);

            writeCursor += len;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyMemory<byte> Read()
        {
            return buffer.Slice(readCursor, writeCursor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlyMemory<byte> Read(int count)
        {
            var slice = buffer.Slice(readCursor, count);
            readCursor += count;
            return slice;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            buffer.Span.Slice(readCursor, Length()).Clear();
            Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
        {
            return writeCursor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Length()
        {
            return writeCursor - readCursor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Cursor()
        {
            return readCursor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FreeSpace()
        {
            return buffer.Length - writeCursor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            readCursor = 0;
            writeCursor = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCount(int count)
        {
            writeCursor = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveOffset(int amount)
        {
            writeCursor += amount;
            // TODO: Make sure cursor doesnt move outside of buffer
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFit(int count)
        {
            return writeCursor + count <= buffer.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFit<T>(int count)
        {
            return writeCursor + (count * Marshal.SizeOf<T>()) <= buffer.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (byte[] array, int length) GetArray()
        {
            if (MemoryMarshal.TryGetArray<byte>(buffer, out var seg))
            {
                return (seg.Array, Length());
            }

            return (null, 0);
        }

        #region DirectPacking
        public void PushBool(ref bool value)
        {
            buffer.Span[writeCursor++] = value ? (byte)255 : (byte)0;
        }

        public void PopBool(out bool value)
        {
            byte val = buffer.Span[readCursor++];
            value = val == 0 ? false : true;
        }

        public unsafe void PushShort(ref short value)
        {
            fixed (byte* val = buffer.Span.Slice(writeCursor, 2))
            {
                for (short i = 0; i < 2; i++)
                {
                    *((short*)val + i) = value;
                }
            }
            writeCursor += 2;
        }

        public unsafe void PopShort(out short value)
        {
            value = 0;
            for (short i = 0; i < 2; i++)
            {
                value |= (short)(buffer.Span[readCursor++] << (i * 8));
            }
        }

        public unsafe void PushUShort(ref ushort value)
        {
            fixed (byte* val = buffer.Span.Slice(writeCursor, 2))
            {
                for (ushort i = 0; i < 2; i++)
                {
                    *((ushort*)val + i) = value;
                }
            }
            writeCursor += 2;
        }

        public unsafe void PopUShort(out ushort value)
        {
            value = 0;
            for (ushort i = 0; i < 2; i++)
            {
                value |= (ushort)(buffer.Span[readCursor++] << (i * 8));
            }
        }

        public unsafe void PushInt(ref int value)
        {
            fixed (byte* val = buffer.Span.Slice(writeCursor, 4))
            {
                for (int i = 0; i < 4; i++)
                {
                    *((int*)val + i) = value;
                }
            }
            writeCursor += 4;
        }

        public unsafe void PopInt(out int value)
        {
            var span = buffer.Span.Slice(readCursor, 4);
            readCursor += 4;
            value = ((span[3] << 24) | (span[2] << 16) | (span[1]) << 8 | (span[0]));
        }

        public unsafe void PushUInt(ref uint value)
        {
            fixed (byte* val = buffer.Span.Slice(writeCursor, 4))
            {
                for (int i = 0; i < 4; i++)
                {
                    *((uint*)val + i) = value;
                }
            }
            writeCursor += 4;
        }

        public void PopUInt(out uint value)
        {
            var span = buffer.Span.Slice(readCursor, 4);
            readCursor += 4;
            value = (uint)((span[3] << 24) | (span[2] << 16) | (span[1]) << 8 | (span[0]));
        }

        public unsafe void PushLong(ref long value)
        {
            fixed (byte* val = buffer.Span.Slice(writeCursor, 8))
            {
                for (int i = 0; i < 8; i++)
                {
                    *((long*)val + i) = value;
                }
            }
            writeCursor += 8;
        }

        public void PopLong(out long value)
        {
            value = 0;
            for (int i = 0; i < 8; i++)
            {
                value |= (long)buffer.Span[readCursor++] << (i * 8);
            }
        }

        public unsafe void PushULong(ref ulong value)
        {
            fixed (byte* val = buffer.Span.Slice(writeCursor, 8))
            {
                for (int i = 0; i < 8; i++)
                {
                    *((ulong*)val + i) = value;
                }
            }
            writeCursor += 8;
        }

        public void PopULong(out ulong value)
        {
            value = 0;
            for (int i = 0; i < 8; i++)
            {
                value |= (ulong)buffer.Span[readCursor++] << (i * 8);
            }
        }

        public unsafe void PushChar(ref char value)
        {
            fixed (byte* val = buffer.Span.Slice(writeCursor, 2))
            {
                for (int i = 0; i < 2; i++)
                {
                    *((char*)val + i) = value;
                }
            }
            writeCursor += 2;
        }

        public unsafe void PopChar(out char value)
        {
            value = '\0';
            for (int i = 0; i < 2; i++)
            {
                value |= (char)(buffer.Span[readCursor++] << (i * 8));
            }
        }

        public void PushByte(ref byte value)
        {
            buffer.Span[writeCursor++] = value;
        }

        public void PopByte(out byte value)
        {
            value = buffer.Span[readCursor++];
        }

        public void PushSByte(ref sbyte value)
        {
            buffer.Span[writeCursor++] = (byte)value;
        }

        public void PopSByte(out sbyte value)
        {
            value = (sbyte)buffer.Span[readCursor++];
        }


        public unsafe void PushFloat(ref float value)
        {
            fixed (byte* buf = buffer.Span.Slice(writeCursor, 4))
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

        public unsafe void PopFloat(out float value)
        {
            value = 0;

            fixed (byte* buf = buffer.Span.Slice(readCursor, 4))
            {
                value = *(float*)buf;
            }

            readCursor += 4;
        }

        public unsafe void PushDouble(ref double value)
        {
            fixed (byte* buf = buffer.Span.Slice(writeCursor, 8))
            {
                *(double*)buf = value;
            }
            writeCursor += 8;
        }

        public unsafe void PopDouble(out double value)
        {
            value = 0;

            fixed (byte* buf = buffer.Span.Slice(readCursor, 8))
            {
                value = *(double*)buf;
            }

            readCursor += 8;
        }

        public unsafe void PushDecimal(ref decimal value)
        {
            fixed (byte* buf = buffer.Span.Slice(writeCursor, 24))
            {
                *(decimal*)buf = value;
            }
            writeCursor += 24;
        }

        public unsafe void PopDecimal(out decimal value)
        {
            value = 0;

            fixed (byte* buf = buffer.Span.Slice(readCursor, 24))
            {
                value = *(decimal*)buf;
            }

            readCursor += 24;
        }
        #endregion
    }
}
using System;
using System.Runtime.CompilerServices;
using RePacker.Unsafe;

namespace RePacker.Buffers.Extra
{
    public static class BufferUtils
    {
        #region DirectPacking
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PushBool(this Buffer buffer, ref bool value)
        {
            buffer.Array[buffer.WriteCursor()] = value ? (byte)1 : (byte)0;
            buffer.MoveWriteCursor(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PopBool(this Buffer buffer, out bool value)
        {
            byte val = buffer.Array[buffer.ReadCursor()];
            buffer.MoveReadCursor(1);
            value = val == 0 ? false : true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PushShort(this Buffer buffer, ref short value)
        {
            if (buffer.WriteCursor() + 2 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write short outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                *((short*)val) = value;
            }
            buffer.MoveWriteCursor(2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PopShort(this Buffer buffer, out short value)
        {
            if (buffer.ReadCursor() + 2 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read short outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(short*)val;
            }
            buffer.MoveReadCursor(2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PushUShort(this Buffer buffer, ref ushort value)
        {
            if (buffer.WriteCursor() + 2 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write ushort outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                *((ushort*)val) = value;
            }
            buffer.MoveWriteCursor(2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PopUShort(this Buffer buffer, out ushort value)
        {
            if (buffer.ReadCursor() + 2 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read ushort outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(ushort*)val;
            }
            buffer.MoveReadCursor(2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PushInt(this Buffer buffer, ref int value)
        {
            if (buffer.WriteCursor() + 4 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write int outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                *((int*)val) = value;
            }
            buffer.MoveWriteCursor(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PopInt(this Buffer buffer, out int value)
        {
            if (buffer.ReadCursor() + 4 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read int outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(int*)val;
            }
            buffer.MoveReadCursor(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PushUInt(this Buffer buffer, ref uint value)
        {
            if (buffer.WriteCursor() + 4 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write uint outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                *((uint*)val) = value;
            }
            buffer.MoveWriteCursor(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PopUInt(this Buffer buffer, out uint value)
        {
            if (buffer.ReadCursor() + 4 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read uint outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(uint*)val;
            }
            buffer.MoveReadCursor(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PushLong(this Buffer buffer, ref long value)
        {
            if (buffer.WriteCursor() + 8 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write long outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                *((long*)val) = value;
            }
            buffer.MoveWriteCursor(8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PopLong(this Buffer buffer, out long value)
        {
            if (buffer.ReadCursor() + 8 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read long outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(long*)val;
            }
            buffer.MoveReadCursor(8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PushULong(this Buffer buffer, ref ulong value)
        {
            if (buffer.WriteCursor() + 8 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write ulong outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                *((ulong*)val) = value;
            }
            buffer.MoveWriteCursor(8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PopULong(this Buffer buffer, out ulong value)
        {
            if (buffer.ReadCursor() + 8 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read ulong outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(ulong*)val;
            }
            buffer.MoveReadCursor(8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PushChar(this Buffer buffer, ref char value)
        {
            if (buffer.WriteCursor() + 2 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write char outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                *((char*)val) = value;
            }
            buffer.MoveWriteCursor(2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PopChar(this Buffer buffer, out char value)
        {
            if (buffer.ReadCursor() + 2 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read char outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(char*)val;
            }
            buffer.MoveReadCursor(2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PushByte(this Buffer buffer, ref byte value)
        {
            buffer.Array[buffer.WriteCursor()] = value;
            buffer.MoveWriteCursor(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PopByte(this Buffer buffer, out byte value)
        {
            value = buffer.Array[buffer.ReadCursor()];
            buffer.MoveReadCursor(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PushSByte(this Buffer buffer, ref sbyte value)
        {
            buffer.Array[buffer.WriteCursor()] = (byte)value;
            buffer.MoveWriteCursor(1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PopSByte(this Buffer buffer, out sbyte value)
        {
            value = (sbyte)buffer.Array[buffer.ReadCursor()];
            buffer.MoveReadCursor(1);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PushFloat(this Buffer buffer, ref float value)
        {
            if (buffer.WriteCursor() + 4 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write float outside of buffer range");
            }

            fixed (byte* buf = &buffer.Array[buffer.WriteCursor()])
            {
                *(float*)buf = value;
            }
            buffer.MoveWriteCursor(4);

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
        public static unsafe void PopFloat(this Buffer buffer, out float value)
        {
            if (buffer.ReadCursor() + 4 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read float outside of buffer range");
            }

            value = 0;

            fixed (byte* buf = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(float*)buf;
            }

            buffer.MoveReadCursor(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PushDouble(this Buffer buffer, ref double value)
        {
            if (buffer.WriteCursor() + 8 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write double outside of buffer range");
            }

            fixed (byte* buf = &buffer.Array[buffer.WriteCursor()])
            {
                *(double*)buf = value;
            }
            buffer.MoveWriteCursor(8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PopDouble(this Buffer buffer, out double value)
        {
            if (buffer.ReadCursor() + 8 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read double outside of buffer range");
            }

            value = 0;

            fixed (byte* buf = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(double*)buf;
            }

            buffer.MoveReadCursor(8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PushDecimal(this Buffer buffer, ref decimal value)
        {
            if (buffer.WriteCursor() + 24 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to write decimal outside of buffer range");
            }

            fixed (byte* buf = &buffer.Array[buffer.WriteCursor()])
            {
                *(decimal*)buf = value;
            }
            buffer.MoveWriteCursor(24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PopDecimal(this Buffer buffer, out decimal value)
        {
            if (buffer.ReadCursor() + 24 > buffer.Array.Length)
            {
                throw new IndexOutOfRangeException("Trying to read decimal outside of buffer range");
            }

            value = 0;

            fixed (byte* buf = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(decimal*)buf;
            }

            buffer.MoveReadCursor(24);
        }
        #endregion
    }
}
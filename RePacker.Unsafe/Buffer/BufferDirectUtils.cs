using System;
using System.Runtime.CompilerServices;
using RePacker.Unsafe;

namespace RePacker.Buffers.Extra
{
    public static class BufferUtils
    {
        #region DirectPacking

        /// <summary>
        /// Packs a Bool into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PackBool(this ReBuffer buffer, ref bool value)
        {
            buffer.Array[buffer.WriteCursor()] = value ? (byte)1 : (byte)0;
            buffer.MoveWriteCursor(1);
        }

        /// <summary>
        /// Unpacks Bool bool from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackBool(this ReBuffer buffer, out bool value)
        {
            byte val = buffer.Array[buffer.ReadCursor()];
            buffer.MoveReadCursor(1);
            value = val == 0 ? false : true;
        }

        /// <summary>
        /// Packs a is into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackByte(this ReBuffer buffer, ref byte value)
        {
            buffer.Array[buffer.WriteCursor()] = value;
            buffer.MoveWriteCursor(1);
        }

        /// <summary>
        /// Unpacks Byte bool from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackByte(this ReBuffer buffer, out byte value)
        {
            value = buffer.Array[buffer.ReadCursor()];
            buffer.MoveReadCursor(1);
        }

        /// <summary>
        /// Packs a his into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PackSByte(this ReBuffer buffer, ref sbyte value)
        {
            buffer.Array[buffer.WriteCursor()] = (byte)value;
            buffer.MoveWriteCursor(1);
        }

        /// <summary>
        /// Unpacks SByte bool from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnpackSByte(this ReBuffer buffer, out sbyte value)
        {
            value = (sbyte)buffer.Array[buffer.ReadCursor()];
            buffer.MoveReadCursor(1);
        }

        /// <summary>
        /// Packs a Short into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PackShort(this ReBuffer buffer, ref short value)
        {
            if (!buffer.CanWrite<short>())
            {
                throw new IndexOutOfRangeException("Trying to write short outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                if (buffer.Endianness == Endianness.BigEndian)
                {
                    *((short*)val) = UnsafeUtils.ToBigEndian(value);
                }
                else
                {
                    *((short*)val) = value;
                }
            }
            buffer.MoveWriteCursor(2);
        }

        /// <summary>
        /// Unpacks a Short from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void UnpackShort(this ReBuffer buffer, out short value)
        {
            if (!buffer.CanRead<short>())
            {
                throw new IndexOutOfRangeException("Trying to read short outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(short*)val;

                if (buffer.Endianness == Endianness.BigEndian)
                {
                    value = UnsafeUtils.ToBigEndian(value);
                }
            }
            buffer.MoveReadCursor(2);
        }

        /// <summary>
        /// Packs a UShort into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PackUShort(this ReBuffer buffer, ref ushort value)
        {
            if (!buffer.CanWrite<ushort>())
            {
                throw new IndexOutOfRangeException("Trying to write ushort outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                if (buffer.Endianness == Endianness.BigEndian)
                {
                    *((ushort*)val) = UnsafeUtils.ToBigEndian(value);
                }
                else
                {
                    *((ushort*)val) = value;
                }
            }
            buffer.MoveWriteCursor(2);
        }

        /// <summary>
        /// Unpacks a UShort from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void UnpackUShort(this ReBuffer buffer, out ushort value)
        {
            if (!buffer.CanRead<ushort>())
            {
                throw new IndexOutOfRangeException("Trying to read ushort outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(ushort*)val;

                if (buffer.Endianness == Endianness.BigEndian)
                {
                    value = UnsafeUtils.ToBigEndian(value);
                }
            }
            buffer.MoveReadCursor(2);
        }

        /// <summary>
        /// Packs a Int into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PackInt(this ReBuffer buffer, ref int value)
        {
            if (!buffer.CanWrite<int>())
            {
                throw new IndexOutOfRangeException("Trying to write int outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                if (buffer.Endianness == Endianness.BigEndian)
                {
                    *((int*)val) = UnsafeUtils.ToBigEndian(value);
                }
                else
                {
                    *((int*)val) = value;
                }
            }
            buffer.MoveWriteCursor(4);
        }

        /// <summary>
        /// Unpacks a Int from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void UnpackInt(this ReBuffer buffer, out int value)
        {
            if (!buffer.CanRead<int>())
            {
                throw new IndexOutOfRangeException("Trying to read int outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(int*)val;

                if (buffer.Endianness == Endianness.BigEndian)
                {
                    value = UnsafeUtils.ToBigEndian(value);
                }
            }
            buffer.MoveReadCursor(4);
        }

        /// <summary>
        /// Packs a UInt into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PackUInt(this ReBuffer buffer, ref uint value)
        {
            if (!buffer.CanWrite<uint>())
            {
                throw new IndexOutOfRangeException("Trying to write uint outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                if (buffer.Endianness == Endianness.BigEndian)
                {
                    *((uint*)val) = UnsafeUtils.ToBigEndian(value);
                }
                else
                {
                    *((uint*)val) = value;
                }
            }
            buffer.MoveWriteCursor(4);
        }

        /// <summary>
        /// Unpacks a UInt from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void UnpackUInt(this ReBuffer buffer, out uint value)
        {
            if (!buffer.CanRead<uint>())
            {
                throw new IndexOutOfRangeException("Trying to read uint outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(uint*)val;

                if (buffer.Endianness == Endianness.BigEndian)
                {
                    value = UnsafeUtils.ToBigEndian(value);
                }
            }
            buffer.MoveReadCursor(4);
        }

        /// <summary>
        /// Packs a Long into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PackLong(this ReBuffer buffer, ref long value)
        {
            if (!buffer.CanWrite<long>())
            {
                throw new IndexOutOfRangeException("Trying to write long outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                if (buffer.Endianness == Endianness.BigEndian)
                {
                    *((long*)val) = UnsafeUtils.ToBigEndian(value);
                }
                else
                {
                    *((long*)val) = value;
                }
            }
            buffer.MoveWriteCursor(8);
        }

        /// <summary>
        /// Unpacks a Long from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void UnpackLong(this ReBuffer buffer, out long value)
        {
            if (!buffer.CanRead<long>())
            {
                throw new IndexOutOfRangeException("Trying to read long outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(long*)val;

                if (buffer.Endianness == Endianness.BigEndian)
                {
                    value = UnsafeUtils.ToBigEndian(value);
                }
            }
            buffer.MoveReadCursor(8);
        }

        /// <summary>
        /// Packs a ULong into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PackULong(this ReBuffer buffer, ref ulong value)
        {
            if (!buffer.CanWrite<ulong>())
            {
                throw new IndexOutOfRangeException("Trying to write ulong outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                if (buffer.Endianness == Endianness.BigEndian)
                {
                    *((ulong*)val) = UnsafeUtils.ToBigEndian(value);
                }
                else
                {
                    *((ulong*)val) = value;
                }
            }
            buffer.MoveWriteCursor(8);
        }

        /// <summary>
        /// Unpacks a ULong from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void UnpackULong(this ReBuffer buffer, out ulong value)
        {
            if (!buffer.CanRead<ulong>())
            {
                throw new IndexOutOfRangeException("Trying to read ulong outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(ulong*)val;

                if (buffer.Endianness == Endianness.BigEndian)
                {
                    value = UnsafeUtils.ToBigEndian(value);
                }
            }
            buffer.MoveReadCursor(8);
        }

        /// <summary>
        /// Packs a Char into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PackChar(this ReBuffer buffer, ref char value)
        {
            if (!buffer.CanWrite<char>())
            {
                throw new IndexOutOfRangeException("Trying to write char outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.WriteCursor()])
            {
                if (buffer.Endianness == Endianness.BigEndian)
                {
                    *((char*)val) = UnsafeUtils.ToBigEndian(value);
                }
                else
                {
                    *((char*)val) = value;
                }
            }
            buffer.MoveWriteCursor(2);
        }

        /// <summary>
        /// Unpacks a Char from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void UnpackChar(this ReBuffer buffer, out char value)
        {
            if (!buffer.CanRead<char>())
            {
                throw new IndexOutOfRangeException("Trying to read char outside of buffer range");
            }

            fixed (byte* val = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(char*)val;

                if (buffer.Endianness == Endianness.BigEndian)
                {
                    value = UnsafeUtils.ToBigEndian(value);
                }
            }
            buffer.MoveReadCursor(2);
        }

        /// <summary>
        /// Packs a Float into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PackFloat(this ReBuffer buffer, ref float value)
        {
            if (!buffer.CanWrite<float>())
            {
                throw new IndexOutOfRangeException("Trying to write float outside of buffer range");
            }

            fixed (byte* buf = &buffer.Array[buffer.WriteCursor()])
            {
                if (buffer.Endianness == Endianness.BigEndian)
                {
                    *(float*)buf = UnsafeUtils.ToBigEndian(value);
                }
                else
                {
                    *(float*)buf = value;
                }
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

        /// <summary>
        /// Unpacks a Float from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void UnpackFloat(this ReBuffer buffer, out float value)
        {
            if (!buffer.CanRead<float>())
            {
                throw new IndexOutOfRangeException("Trying to read float outside of buffer range");
            }

            fixed (byte* buf = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(float*)buf;

                if (buffer.Endianness == Endianness.BigEndian)
                {
                    value = UnsafeUtils.ToBigEndian(value);
                }
            }

            buffer.MoveReadCursor(4);
        }

        /// <summary>
        /// Packs a Double into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PackDouble(this ReBuffer buffer, ref double value)
        {
            if (!buffer.CanWrite<double>())
            {
                throw new IndexOutOfRangeException("Trying to write double outside of buffer range");
            }

            fixed (byte* buf = &buffer.Array[buffer.WriteCursor()])
            {
                if (buffer.Endianness == Endianness.BigEndian)
                {
                    *(double*)buf = UnsafeUtils.ToBigEndian(value);
                }
                else
                {
                    *(double*)buf = value;
                }
            }
            buffer.MoveWriteCursor(8);
        }

        /// <summary>
        /// Unpacks a Double from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void UnpackDouble(this ReBuffer buffer, out double value)
        {
            if (!buffer.CanRead<double>())
            {
                throw new IndexOutOfRangeException("Trying to read double outside of buffer range");
            }

            fixed (byte* buf = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(double*)buf;

                if (buffer.Endianness == Endianness.BigEndian)
                {
                    value = UnsafeUtils.ToBigEndian(value);
                }
            }

            buffer.MoveReadCursor(8);
        }

        /// <summary>
        /// Packs a Decimal into the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void PackDecimal(this ReBuffer buffer, ref decimal value)
        {
            if (!buffer.CanWrite<decimal>())
            {
                throw new IndexOutOfRangeException("Trying to write decimal outside of buffer range");
            }

            fixed (byte* buf = &buffer.Array[buffer.WriteCursor()])
            {
                if (buffer.Endianness == Endianness.BigEndian)
                {
                    *(decimal*)buf = UnsafeUtils.ToBigEndian(value);
                }
                else
                {
                    *(decimal*)buf = value;
                }
            }
            buffer.MoveWriteCursor(24);
        }

        /// <summary>
        /// Unpacks a Decimal from the buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void UnpackDecimal(this ReBuffer buffer, out decimal value)
        {
            if (!buffer.CanRead<decimal>())
            {
                throw new IndexOutOfRangeException("Trying to read decimal outside of buffer range");
            }

            fixed (byte* buf = &buffer.Array[buffer.ReadCursor()])
            {
                value = *(decimal*)buf;

                if (buffer.Endianness == Endianness.BigEndian)
                {
                    value = UnsafeUtils.ToBigEndian(value);
                }
            }

            buffer.MoveReadCursor(24);
        }
        #endregion
    }
}
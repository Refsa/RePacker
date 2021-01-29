using System.Runtime.InteropServices;
using System;

namespace Refsa.Repacker.Buffers
{
    public struct Buffer
    {
        Memory<byte> buffer;
        int offset;
        int cursor;
        public int Index;

        public Buffer(Memory<byte> buffer, int index, int offset = 0)
        {
            this.buffer = buffer;
            Index = index;

            this.offset = offset;
            this.cursor = 0;
        }

        public void Push<T>(ref T value) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            var span = buffer.Span.Slice(offset, size);
            MemoryMarshal.Write(span, ref value);
            offset += size;
        }

        public void Push<T>(ref T value, int index) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            var span = buffer.Span.Slice(index, size);
            MemoryMarshal.Write(span, ref value);

            // TODO: Make sure offset aligns with new length
        }

        public void Pop<T>(out T value) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            value = MemoryMarshal.Read<T>(buffer.Span.Slice(cursor, size));
            cursor += size;
        }

        public void Write(ReadOnlySpan<byte> data)
        {
            var span = buffer.Span.Slice(offset, data.Length);
            data.CopyTo(span);
            offset += data.Length;
        }

        public void Write(Memory<byte> data)
        {
            var mem = buffer.Slice(offset, data.Length);
            data.CopyTo(mem);
            offset += data.Length;
        }

        public void Write(Buffer data)
        {
            int len = data.Length();

            var mem = buffer.Slice(offset, len);
            data.Read().CopyTo(mem);

            offset += len;
        }

        public Memory<byte> Write()
        {
            return buffer;
        }

        public ReadOnlyMemory<byte> Read()
        {
            return buffer.Slice(cursor, offset);
        }

        public ReadOnlyMemory<byte> Read(int count)
        {
            var slice = buffer.Slice(cursor, count);
            cursor += count;
            return slice;
        }

        public void Flush()
        {
            buffer.Span.Slice(cursor, Length()).Clear();
            Reset();
        }

        public int Count()
        {
            return offset;
        }

        public int Length()
        {
            return offset - cursor;
        }

        public int Cursor()
        {
            return cursor;
        }

        public int FreeSpace()
        {
            return buffer.Length - offset;
        }

        public void Reset()
        {
            cursor = 0;
            offset = 0;
        }

        public void SetCount(int count)
        {
            offset = count;
        }

        public void MoveOffset(int amount)
        {
            offset += amount;
            // TODO: Make sure cursor doesnt move outside of buffer
        }

        public bool CanFit(int count)
        {
            return offset + count <= buffer.Length;
        }

        public bool CanFit<T>(int count)
        {
            return offset + (count * Marshal.SizeOf<T>()) <= buffer.Length;
        }

        public (byte[] array, int length) GetArray()
        {
            if (MemoryMarshal.TryGetArray<byte>(buffer, out var seg))
            {
                return (seg.Array, Length());
            }

            return (null, 0);
        }
    }
}
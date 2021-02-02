using System.Runtime.InteropServices;
using System;

namespace Refsa.RePacker.Buffers
{
    public struct Buffer
    {
        Memory<byte> buffer;
        int writeCursor;
        int readCursor;
        public int Index;

        public Buffer(Memory<byte> buffer, int index, int offset = 0)
        {
            this.buffer = buffer;
            Index = index;

            this.writeCursor = offset;
            this.readCursor = 0;
        }

        public Buffer Push<T>(ref T value) where T : unmanaged
        {
            int size = Marshal.SizeOf<T>();
            var span = buffer.Span.Slice(writeCursor, size);
            MemoryMarshal.Write(span, ref value);
            writeCursor += size;

            return this;
        }

        public Buffer Push<T>(ref T value, int index) where T : unmanaged
        {
            int size = Marshal.SizeOf<T>();
            var span = buffer.Span.Slice(index, size);
            MemoryMarshal.Write(span, ref value);

            return this;
            // TODO: Make sure offset aligns with new length
        }

        public Buffer Pop<T>(out T value) where T : unmanaged
        {
            int size = Marshal.SizeOf<T>();
            value = MemoryMarshal.Read<T>(buffer.Span.Slice(readCursor, size));
            readCursor += size;

            return this;
        }

        public void Write(ReadOnlySpan<byte> data)
        {
            var span = buffer.Span.Slice(writeCursor, data.Length);
            data.CopyTo(span);
            writeCursor += data.Length;
        }

        public void Write(Memory<byte> data)
        {
            var mem = buffer.Slice(writeCursor, data.Length);
            data.CopyTo(mem);
            writeCursor += data.Length;
        }

        public void Write(Buffer data)
        {
            int len = data.Length();

            var mem = buffer.Slice(writeCursor, len);
            data.Read().CopyTo(mem);

            writeCursor += len;
        }

        public Memory<byte> Write()
        {
            return buffer;
        }

        public ReadOnlyMemory<byte> Read()
        {
            return buffer.Slice(readCursor, writeCursor);
        }

        public ReadOnlyMemory<byte> Read(int count)
        {
            var slice = buffer.Slice(readCursor, count);
            readCursor += count;
            return slice;
        }

        public void Flush()
        {
            buffer.Span.Slice(readCursor, Length()).Clear();
            Reset();
        }

        public int Count()
        {
            return writeCursor;
        }

        public int Length()
        {
            return writeCursor - readCursor;
        }

        public int Cursor()
        {
            return readCursor;
        }

        public int FreeSpace()
        {
            return buffer.Length - writeCursor;
        }

        public void Reset()
        {
            readCursor = 0;
            writeCursor = 0;
        }

        public void SetCount(int count)
        {
            writeCursor = count;
        }

        public void MoveOffset(int amount)
        {
            writeCursor += amount;
            // TODO: Make sure cursor doesnt move outside of buffer
        }

        public bool CanFit(int count)
        {
            return writeCursor + count <= buffer.Length;
        }

        public bool CanFit<T>(int count)
        {
            return writeCursor + (count * Marshal.SizeOf<T>()) <= buffer.Length;
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
using System.Runtime.InteropServices;
using System;

namespace Refsa.Repacker.Buffers
{
    public struct ReadOnlyBuffer
    {
        ReadOnlyMemory<byte> buffer;
        int size;
        int cursor;
        public int Index;

        public ReadOnlyBuffer(ReadOnlyMemory<byte> buffer, int index, int offset = 0)
        {
            this.buffer = buffer;
            Index = index;

            this.size = offset;
            this.cursor = 0;
        }

        public ReadOnlyBuffer(ref Buffer buffer)
        {
            this.buffer = buffer.Read();
            Index = 0;

            this.size = buffer.Length();
            this.cursor = 0;
        }

        public void Pop<T>(out T value) where T : unmanaged
        {
            int size = Marshal.SizeOf<T>();
            value = MemoryMarshal.Read<T>(buffer.Span.Slice(cursor, size));
            cursor += size;
        }

        public ReadOnlyMemory<byte> Read()
        {
            return buffer.Slice(cursor, size);
        }

        public ReadOnlyMemory<byte> Read(int count)
        {
            var slice = buffer.Slice(cursor, count);
            cursor += count;
            return slice;
        }

        public int Count()
        {
            return size;
        }

        public int Length()
        {
            return size - cursor;
        }

        public int Cursor()
        {
            return cursor;
        }

        public void Reset()
        {
            cursor = 0;
        }
    }
}
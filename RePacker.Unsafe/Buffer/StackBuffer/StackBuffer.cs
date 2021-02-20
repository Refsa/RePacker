using System.Runtime.InteropServices;
using System;

namespace RePacker.Buffers
{
    public unsafe ref struct StackBuffer
    {
        void* buf;
        Span<byte> buffer;
        int offset;
        int cursor;
        public int Index;

        public StackBuffer(int size, int index, int offset = 0)
        {
            void* buf = stackalloc byte[size];

            this.buf = buf;
            this.buffer = new Span<byte>(buf, size);

            this.Index = index;
            this.offset = offset;
            this.cursor = 0;
        }

        public void Push<T>(ref T value) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            var span = buffer.Slice(offset, size);
            MemoryMarshal.Write(span, ref value);
            offset += size;
        }

        public void Push<T>(ref T value, int index) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            var span = buffer.Slice(index, size);
            MemoryMarshal.Write(span, ref value);

            // TODO: Make sure offset aligns with new length
        }

        public void Pop<T>(out T value) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            value = MemoryMarshal.Read<T>(buffer.Slice(cursor, size));
            cursor += size;
        }

        public void Write(ReadOnlySpan<byte> data)
        {
            var span = buffer.Slice(offset, data.Length);
            data.CopyTo(span);
            offset += data.Length;
        }

        public void Write(Buffer data)
        {
            int len = data.WriteCursor();

            var mem = buffer.Slice(offset, len - 1);
            data.Read().Span.CopyTo(mem);

            offset += len;
        }

        public ReadOnlySpan<byte> Read()
        {
            return buffer.Slice(cursor, offset);
        }

        public void Flush()
        {
            buffer.Clear();
            offset = 0;
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

        public void SetCount(int count)
        {
            offset = count;
        }

        public void MoveOffset(int amount)
        {
            offset += amount;
            // TODO: Make sure cursor doesnt move outside of buffer
        }

        public void Dispose()
        {
            Free();
        }

        unsafe void Free()
        {

        }
    }
}
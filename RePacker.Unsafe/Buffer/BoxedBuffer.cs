using System;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Buffers
{
    public class BoxedBuffer
    {
        public Buffer Buffer;

        public BoxedBuffer(int size)
        {
            Buffer = new Buffer(new byte[size], 0);
        }

        public BoxedBuffer(byte[] buffer)
        {
            Buffer = new Buffer(buffer, 0);
        }

        public BoxedBuffer(ref Buffer buffer)
        {
            Buffer = buffer;
        }

#region Buffer Wrappers
        public void Push<T>(ref T value) where T : unmanaged
        {
            Buffer.Push(ref value);
        }

        public void Push<T>(ref T value, int index) where T : unmanaged
        {
            Buffer.Push(ref value, index);
        }

        public void Pop<T>(out T value) where T : unmanaged
        {
            Buffer.Pop<T>(out value);
        }

        public void Write(ReadOnlySpan<byte> data)
        {
            Buffer.Write(data);
        }

        public void Write(Memory<byte> data)
        {
            Buffer.Write(data);
        }

        public void Write(Buffer data)
        {
            Buffer.Write(data);
        }

        public ReadOnlyMemory<byte> Read()
        {
            return Buffer.Read();
        }

        public ReadOnlyMemory<byte> Read(int count)
        {
            return Buffer.Read(count);
        }

        public int Count()
        {
            return Buffer.Count();
        }

        public int Length()
        {
            return Buffer.Length();
        }

        public int Cursor()
        {
            return Buffer.Cursor();
        }

        public int FreeSpace()
        {
            return Buffer.FreeSpace();
        }

        public void Flush()
        {
            Buffer.Flush();
        }

        public void Reset()
        {
            Buffer.Reset();
        }

        public void SetCount(int count)
        {
            Buffer.SetCount(count);
        }

        public void MoveOffset(int amount)
        {
            Buffer.MoveOffset(amount);
            // TODO: Make sure cursor doesnt move outside of buffer
        }

        public bool CanFit(int count)
        {
            return Buffer.CanFit(count);
        }

        public bool CanFit<T>(int count)
        {
            return Buffer.CanFit<T>(count);
        }

        public (byte[] array, int length) GetArray()
        {
            return Buffer.GetArray();
        }
#endregion
    }
}
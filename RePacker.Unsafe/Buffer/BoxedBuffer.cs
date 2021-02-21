using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Buffer = RePacker.Buffers.Buffer;

namespace RePacker.Buffers
{
    public class BoxedBuffer
    {
        public byte[] Data;
        public Buffer Buffer;

        public BoxedBuffer(int size)
        {
            Data = new byte[size];
            Buffer = new Buffer(Data, 0);
        }

        public BoxedBuffer(byte[] buffer)
        {
            Data = buffer;
            Buffer = new Buffer(buffer, 0);
        }

        public BoxedBuffer(ref Buffer buffer)
        {
            Data = buffer.Array;
            Buffer = buffer;
        }
        public unsafe void MemoryCopy<T>(T[] array) where T : unmanaged
        {
            Buffer.MemoryCopyFromUnsafe(array);
        }
        public unsafe T[] MemoryCopy<T>() where T : unmanaged
        {
            return Buffer.MemoryCopyToUnsafe<T>();
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
            return Buffer.WriteCursor();
        }

        public int Length()
        {
            return Buffer.Length();
        }

        public int Cursor()
        {
            return Buffer.ReadCursor();
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
            Buffer.SetReadCursor(count);
        }

        public void MoveOffset(int amount)
        {
            Buffer.MoveWriteCursor(amount);
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

        public byte[] GetArray()
        {
            return Buffer.Array;
        }
        #endregion
    }
}
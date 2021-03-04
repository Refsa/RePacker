using System;
using System.IO;
using System.Runtime.CompilerServices;
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

        public void Copy(ref Buffer other)
        {
            Buffer.Copy(ref other);
        }

        public void Copy(BoxedBuffer other)
        {
            Buffer.Copy(ref other.Buffer);
        }

        #region Buffer Wrappers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void MemoryCopy<T>(T[] array, int offset = 0, int count = 0) where T : unmanaged
        {
            Buffer.PackArray(array, offset, count);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T[] MemoryCopy<T>() where T : unmanaged
        {
            return Buffer.UnpackArray<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push<T>(ref T value) where T : unmanaged
        {
            Buffer.Pack(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pop<T>(out T value) where T : unmanaged
        {
            Buffer.Unpack<T>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop<T>() where T : unmanaged
        {
            Buffer.Unpack<T>(out T value);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek<T>() where T : unmanaged
        {
            return Buffer.Peek<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int WriteCursor()
        {
            return Buffer.WriteCursor();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Length()
        {
            return Buffer.Length();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadCursor()
        {
            return Buffer.ReadCursor();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FreeSpace()
        {
            return Buffer.FreeSpace();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Flush()
        {
            Buffer.Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            Buffer.Reset();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveReadCursor(int amount)
        {
            Buffer.MoveReadCursor(amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveWriteCursor(int amount)
        {
            Buffer.MoveWriteCursor(amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetWriteCursor(int pos)
        {
            Buffer.SetWriteCursor(pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetReadCursor(int pos)
        {
            Buffer.SetReadCursor(pos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFit(int count)
        {
            return Buffer.CanWriteBytes(count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFit<T>(int count) where T : unmanaged
        {
            return Buffer.CanWrite<T>(count);
        }
        #endregion
    }
}
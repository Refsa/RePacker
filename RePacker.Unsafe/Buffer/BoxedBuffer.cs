using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Buffers
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
            var array = buffer.GetArray();
            Data = array;
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
            Buffer.SetCount(count);
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
            return Buffer.GetArray();
        }

        public void PushShort(ref short value)
        {
            Buffer.PushShort(ref value);
        }
        public void PopShort(out short value)
        {
            Buffer.PopShort(out value);
        }
        public void PushUShort(ref ushort value)
        {
            Buffer.PushUShort(ref value);
        }
        public void PopUShort(out ushort value)
        {
            Buffer.PopUShort(out value);
        }
        public void PushInt(ref int value)
        {
            Buffer.PushInt(ref value);
        }
        public void PopInt(out int value)
        {
            Buffer.PopInt(out value);
        }
        public void PushUInt(ref uint value)
        {
            Buffer.PushUInt(ref value);
        }
        public void PopUInt(out uint value)
        {
            Buffer.PopUInt(out value);
        }
        public void PushLong(ref long value)
        {
            Buffer.PushLong(ref value);
        }
        public void PopLong(out long value)
        {
            Buffer.PopLong(out value);
        }
        public void PushULong(ref ulong value)
        {
            Buffer.PushULong(ref value);
        }
        public void PopULong(out ulong value)
        {
            Buffer.PopULong(out value);
        }
        public void PushChar(ref char value)
        {
            Buffer.PushChar(ref value);
        }
        public void PopChar(out char value)
        {
            Buffer.PopChar(out value);
        }
        public void PushByte(ref byte value)
        {
            Buffer.PushByte(ref value);
        }
        public void PopByte(out byte value)
        {
            Buffer.PopByte(out value);
        }
        public void PushSByte(ref sbyte value)
        {
            Buffer.PushSByte(ref value);
        }
        public void PopSByte(out sbyte value)
        {
            Buffer.PopSByte(out value);
        }
        public void PushFloat(ref float value)
        {
            Buffer.PushFloat(ref value);
        }
        public void PopFloat(out float value)
        {
            Buffer.PopFloat(out value);
        }
        public void PushDouble(ref double value)
        {
            Buffer.PushDouble(ref value);
        }
        public void PopDouble(out double value)
        {
            Buffer.PopDouble(out value);
        }
        public void PushDecimal(ref decimal value)
        {
            Buffer.PushDecimal(ref value);
        }
        public void PopDecimal(out decimal value)
        {
            Buffer.PopDecimal(out value);
        }
        #endregion
    }
}
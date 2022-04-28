using System;
using System.Runtime.InteropServices;

namespace RePacker
{
    unsafe public struct NativeBlob : IDisposable
    {
        IntPtr ptr;
        byte* data;

        int capacity;

        public int Capacity => capacity;

        public NativeBlob(int capacity)
        {
            this.capacity = capacity;
            this.ptr = Marshal.AllocHGlobal(capacity);
            this.data = (byte*)this.ptr.ToPointer();
        }

        public static NativeBlob From(byte[] array)
        {
            var blob = new NativeBlob(array.Length);
            Marshal.Copy(array, 0, blob.ptr, array.Length);
            return blob;
        }

        public void Write<T>(int index, in T value)
            where T : unmanaged
        {
            *(T*)(data + index) = value;
        }

        public T Read<T>(int index)
            where T : unmanaged
        {
            return *(T*)(data + index);
        }

        public void EnsureCapacity(int capacity)
        {
            if (this.capacity >= capacity) return;

            Marshal.ReAllocHGlobal(ptr, (IntPtr)capacity);

            this.data = (byte*)ptr.ToPointer();
            this.capacity = capacity;
        }

        public void Shrink(int length)
        {
            if (length >= capacity) return;

            Marshal.ReAllocHGlobal(ptr, (IntPtr)length);
        }

        public byte[] ToArray(int length)
        {
            var array = new byte[length];

            Marshal.Copy(ptr, array, 0, length);

            return array;
        }

        public void CopyTo(ref NativeBlob other, int length)
        {
            Buffer.MemoryCopy(data, other.data, length, length);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}
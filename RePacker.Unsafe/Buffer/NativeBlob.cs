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

        public void Write<T>(int byteOffset, in T value)
            where T : unmanaged
        {
            if (byteOffset >= capacity) throw new IndexOutOfRangeException();

            *(T*)(data + byteOffset) = value;
        }

        public T Read<T>(int byteOffset)
            where T : unmanaged
        {
            if (byteOffset >= capacity) throw new IndexOutOfRangeException();

            return *(T*)(data + byteOffset);
        }

        public ref T GetRef<T>(int byteOffset)
            where T : unmanaged
        {
            if (byteOffset >= capacity) throw new IndexOutOfRangeException();

            return ref *(T*)(data + byteOffset);
        }

        public void EnsureCapacity(int capacity)
        {
            if (this.capacity > capacity) return;

            this.ptr = Marshal.ReAllocHGlobal(this.ptr, (IntPtr)capacity);
            this.data = (byte*)this.ptr.ToPointer();
            this.capacity = capacity;
        }

        public void ShrinkFit(int startOffset, int bytes)
        {
            if ((startOffset + bytes) > capacity) return;
            this.capacity = bytes;

            if (startOffset == 0)
            {
                this.ptr = Marshal.ReAllocHGlobal(this.ptr, (IntPtr)bytes);
                this.data = (byte*)this.ptr.ToPointer();
            }
            else
            {
                var newPtr = Marshal.AllocHGlobal(bytes);
                var newData = (byte*)newPtr.ToPointer();

                var src = this.data + startOffset;

                Buffer.MemoryCopy(src, newData, bytes, bytes);
                Marshal.FreeHGlobal(this.ptr);

                this.ptr = newPtr;
                this.data = newData;
            }
        }

        public byte[] ToArray(int length)
        {
            var array = new byte[length];
            Marshal.Copy(ptr, array, 0, length);
            return array;
        }

        public void CopyTo(ref NativeBlob dst, int length, int srcOffset = 0, int dstOffset = 0)
        {
            if ((srcOffset + length) > capacity)
                throw new IndexOutOfRangeException("Source offset is outside of source capacity");
            if ((dstOffset + length) > dst.capacity)
                throw new IndexOutOfRangeException("Dest offset is outside of dest capacity");

            var srcPtr = data + srcOffset;
            var dstPtr = dst.data + dstOffset;
            Buffer.MemoryCopy(srcPtr, dstPtr, dst.capacity, length);
        }

        public void CopyTo<T>(T[] dst, int srcOffset, int elements)
            where T : unmanaged
        {
            if ((srcOffset + elements) > capacity) throw new IndexOutOfRangeException();

            var src = data + srcOffset;
            var len = sizeof(T) * elements;

            fixed (void* dstPtr = &dst[0])
            {
                Buffer.MemoryCopy(src, dstPtr, len, len);
            }
        }

        public void Append<T>(T[] src, int srcOffset, int dstOffset, int elements)
            where T : unmanaged
        {
            if ((dstOffset + elements) > capacity)
                throw new IndexOutOfRangeException("Dest offset is outside of capacity");
            if ((srcOffset + elements) > src.Length)
                throw new IndexOutOfRangeException("Src offset is outside of src Length");

            int stride = sizeof(T);
            int len = elements * stride;
            var dst = data + dstOffset;

            fixed (void* srcPtr = &src[srcOffset])
            {
                Buffer.MemoryCopy(srcPtr, dst, len, len);
            }
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class UnmanagedPacker<T> : IPacker<T> where T : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pack(ReBuffer buffer, ref T value)
        {
            buffer.Pack(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unpack(ReBuffer buffer, out T value)
        {
            buffer.Unpack<T>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnpackInto(ReBuffer buffer, ref T value)
        {
            buffer.Unpack<T>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int SizeOf(ref T value)
        {
            return RePacker.Unsafe.UnsafeUtils.SizeOf<T>();
        }
    }
}
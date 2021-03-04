using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class UnmanagedPacker<T> : IPacker<T> where T : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pack(Buffer buffer, ref T value)
        {
            buffer.Pack(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unpack(Buffer buffer, out T value)
        {
            buffer.Unpack<T>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnpackInto(Buffer buffer, ref T value)
        {
            buffer.Unpack<T>(out value);
        }
    }
}
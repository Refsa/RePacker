using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class UnmanagedPacker<T> : IPacker<T> where T : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pack(BoxedBuffer buffer, ref T value)
        {
            buffer.Buffer.Pack(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unpack(BoxedBuffer buffer, out T value)
        {
            buffer.Buffer.Unpack<T>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnpackInto(BoxedBuffer buffer, ref T value)
        {
            buffer.Buffer.Unpack<T>(out value);
        }
    }
}
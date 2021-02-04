

using System.Runtime.CompilerServices;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    public abstract class RePackerWrapper<T> : ITypePacker
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Pack(BoxedBuffer buffer, ref T value);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Unpack(BoxedBuffer buffer, ref T value);
    }
}
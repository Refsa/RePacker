using System;
using System.Runtime.CompilerServices;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Builder;

namespace Refsa.RePacker
{
    public abstract class RePackerWrapper<T> : ITypePacker
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Pack(BoxedBuffer buffer, ref T value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Unpack(BoxedBuffer buffer, out T value)
        {
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void UnpackInto(BoxedBuffer buffer, ref T value)
        {
            throw new NotSupportedException();
        }
    }
}
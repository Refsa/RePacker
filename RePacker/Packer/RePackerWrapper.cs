using System;
using System.Runtime.CompilerServices;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    public abstract class RePackerWrapper<T> : IPacker<T>
    {
        public virtual bool IsDirectlyCopyable { get; } = false;

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

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RePackerWrapperAttribute : System.Attribute
    {
        public Type WrapperFor;

        public RePackerWrapperAttribute(Type wrapperFor)
        {
            WrapperFor = wrapperFor;
        }
    }
}
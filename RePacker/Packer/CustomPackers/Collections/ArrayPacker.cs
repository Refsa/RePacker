using System.Runtime.CompilerServices;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Builder;

namespace Refsa.RePacker.Builder
{
    internal class ArrayPacker<TElement> : RePackerWrapper<TElement[]>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(BoxedBuffer buffer, ref TElement[] value)
        {
            buffer.PackArray(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(BoxedBuffer buffer, out TElement[] value)
        {
            buffer.UnpackArray<TElement>(out value);
        }
    }

    internal class ArrayUnmanagedPacker<TElement> : RePackerWrapper<TElement[]> where TElement : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(BoxedBuffer buffer, ref TElement[] value)
        {
            buffer.Buffer.PackBlittableArray(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(BoxedBuffer buffer, out TElement[] value)
        {
            buffer.Buffer.UnpackUnmanagedArrayOut<TElement>(out value);
        }
    }
}
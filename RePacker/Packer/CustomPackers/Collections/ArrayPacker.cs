using Refsa.RePacker.Buffers;
using Refsa.RePacker.Builder;

namespace Refsa.RePacker.Builder
{
    internal class ArrayPacker<TElement> : RePackerWrapper<TElement[]>
    {
        public override void Pack(BoxedBuffer buffer, ref TElement[] value)
        {
            buffer.PackArray(value);
        }

        public override void Unpack(BoxedBuffer buffer, out TElement[] value)
        {
            buffer.UnpackArray<TElement>(out value);
        }
    }

    internal class ArrayUnmanagedPacker<TElement> : RePackerWrapper<TElement[]> where TElement : unmanaged
    {
        public override void Pack(BoxedBuffer buffer, ref TElement[] value)
        {
            buffer.Buffer.PackBlittableArray(value);
        }

        public override void Unpack(BoxedBuffer buffer, out TElement[] value)
        {
            buffer.Buffer.UnpackUnmanagedArrayOut<TElement>(out value);
        }
    }
}
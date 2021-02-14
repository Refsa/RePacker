using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class ArrayWrapper<TElement> : RePackerWrapper<TElement[]>
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
}
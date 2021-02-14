using System.Collections.Generic;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class StackWrapper<TElement> : RePackerWrapper<Stack<TElement>>
    {
        public override void Pack(BoxedBuffer buffer, ref Stack<TElement> value)
        {
            buffer.PackIEnumerable(value);
        }

        public override void Unpack(BoxedBuffer buffer, out Stack<TElement> value)
        {
            buffer.UnpackIEnumerable<TElement>(PackerCollectionsExt.IEnumerableType.Stack, out var ien);
            value = (Stack<TElement>)ien;
        }
    }
}
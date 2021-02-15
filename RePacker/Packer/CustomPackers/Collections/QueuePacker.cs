using System.Collections.Generic;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class QueuePacker<TElement> : RePackerWrapper<Queue<TElement>>
    {
        public override void Pack(BoxedBuffer buffer, ref Queue<TElement> value)
        {
            buffer.PackIEnumerable(value);
        }

        public override void Unpack(BoxedBuffer buffer, out Queue<TElement> value)
        {
            buffer.UnpackIEnumerable<TElement>(PackerCollectionsExt.IEnumerableType.Queue, out var ien);
            value = (Queue<TElement>)ien;
        }
    }
}
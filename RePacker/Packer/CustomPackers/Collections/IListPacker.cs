using System.Collections.Generic;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class IListPacker<TElement> : RePackerWrapper<IList<TElement>>
    {
        public override void Pack(BoxedBuffer buffer, ref IList<TElement> value)
        {
            buffer.PackIList(value);
        }

        public override void Unpack(BoxedBuffer buffer, out IList<TElement> value)
        {
            buffer.UnpackIList<TElement>(out var ilist);
            value = (IList<TElement>)ilist;
        }
    }
}
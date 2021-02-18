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

    internal class IListUnmanagedPacker<TElement> : RePackerWrapper<IList<TElement>> where TElement : unmanaged
    {
        public override void Pack(BoxedBuffer buffer, ref IList<TElement> value)
        {
            buffer.PackIListBlittable(value);
        }

        public override void Unpack(BoxedBuffer buffer, out IList<TElement> value)
        {
            buffer.UnpackIListBlittable<TElement>(out var ilist);
            value = (IList<TElement>)ilist;
        }
    }
}
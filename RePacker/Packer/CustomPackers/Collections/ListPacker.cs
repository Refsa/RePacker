using System.Collections.Generic;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class ListPacker<TElement> : RePackerWrapper<List<TElement>>
    {
        public override void Pack(Buffer buffer, ref List<TElement> value)
        {
            buffer.PackIList(value);
        }

        public override void Unpack(Buffer buffer, out List<TElement> value)
        {
            buffer.UnpackIList<TElement>(out var ilist);
            value = (List<TElement>)ilist;
        }
    }
}
using System.Collections.Generic;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class HashSetPacker<TElement> : RePackerWrapper<HashSet<TElement>>
    {
        public override void Pack(BoxedBuffer buffer, ref HashSet<TElement> value)
        {
            buffer.PackHashSet(value);
        }

        public override void Unpack(BoxedBuffer buffer, out HashSet<TElement> value)
        {
            buffer.UnpackHashSet<TElement>(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref HashSet<TElement> value)
        {
            Unpack(buffer, out value);
        }
    }
}
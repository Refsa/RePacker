using System.Collections.Generic;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class HashSetWrapper<TElement> : RePackerWrapper<HashSet<TElement>>
    {
        public override void Pack(BoxedBuffer buffer, ref HashSet<TElement> value)
        {
            buffer.PackIEnumerable(value);
        }

        public override void Unpack(BoxedBuffer buffer, out HashSet<TElement> value)
        {
            buffer.UnpackIEnumerable<TElement>(PackerCollectionsExt.IEnumerableType.HashSet, out var ien);
            value = (HashSet<TElement>)ien;
        }
    }
}
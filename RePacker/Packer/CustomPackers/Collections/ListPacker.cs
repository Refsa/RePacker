using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class ListPacker<TElement> : RePackerWrapper<List<TElement>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref List<TElement> value)
        {
            buffer.PackIList(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out List<TElement> value)
        {
            buffer.UnpackIList<TElement>(out var ilist);
            value = (List<TElement>)ilist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref List<TElement> value)
        {
            return PackerCollectionsExt.SizeOfColleciton<TElement>(value.GetEnumerator());
        }
    }
}
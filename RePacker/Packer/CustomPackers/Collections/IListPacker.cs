using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class IListPacker<TElement> : RePackerWrapper<IList<TElement>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref IList<TElement> value)
        {
            buffer.PackIList(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out IList<TElement> value)
        {
            buffer.UnpackIList<TElement>(out var ilist);
            value = (IList<TElement>)ilist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref IList<TElement> value)
        {
            return PackerCollectionsExt.SizeOfColleciton<TElement>(value.GetEnumerator());
        }
    }

    internal class IListUnmanagedPacker<TElement> : RePackerWrapper<IList<TElement>> where TElement : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref IList<TElement> value)
        {
            buffer.PackIListBlittable(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out IList<TElement> value)
        {
            buffer.UnpackIListBlittable<TElement>(out var ilist);
            value = (IList<TElement>)ilist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref IList<TElement> value)
        {
            return PackerCollectionsExt.SizeOfColleciton<TElement>(value.GetEnumerator());
        }
    }
}
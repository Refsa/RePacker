using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class HashSetPacker<TElement> : RePackerWrapper<HashSet<TElement>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref HashSet<TElement> value)
        {
            buffer.PackHashSet(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out HashSet<TElement> value)
        {
            buffer.UnpackHashSet<TElement>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref HashSet<TElement> value)
        {
            Unpack(buffer, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref HashSet<TElement> value)
        {
            return PackerCollectionsExt.SizeOfColleciton<TElement>(value.GetEnumerator());
        }
    }
}
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class QueuePacker<TElement> : RePackerWrapper<Queue<TElement>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref Queue<TElement> value)
        {
            buffer.PackIEnumerable(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out Queue<TElement> value)
        {
            buffer.UnpackQueue<TElement>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref Queue<TElement> value)
        {
            Unpack(buffer, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref Queue<TElement> value)
        {
            return PackerCollectionsExt.SizeOfCollection<TElement>(value.GetEnumerator());
        }
    }
}
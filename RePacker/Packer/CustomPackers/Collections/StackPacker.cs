using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class StackPacker<TElement> : RePackerWrapper<Stack<TElement>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref Stack<TElement> value)
        {
            buffer.PackStack(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out Stack<TElement> value)
        {
            buffer.UnpackStack<TElement>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref Stack<TElement> value)
        {
            Unpack(buffer, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref Stack<TElement> value)
        {
            return PackerCollectionsExt.SizeOfColleciton<TElement>(value.GetEnumerator());
        }
    }
}
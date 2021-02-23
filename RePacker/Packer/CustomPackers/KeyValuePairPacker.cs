using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class KeyValuePairPacker<T1, T2> : RePackerWrapper<KeyValuePair<T1, T2>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(BoxedBuffer buffer, ref KeyValuePair<T1, T2> value)
        {
            buffer.PackKeyValuePair(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(BoxedBuffer buffer, out KeyValuePair<T1, T2> value)
        {
            buffer.UnpackKeyValuePair<T1, T2>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(BoxedBuffer buffer, ref KeyValuePair<T1, T2> value)
        {
            buffer.UnpackKeyValuePair<T1, T2>(out value);
        }
    }
}
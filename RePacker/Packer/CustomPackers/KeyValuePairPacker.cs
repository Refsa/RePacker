using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class KeyValuePairPacker<T1, T2> : RePackerWrapper<KeyValuePair<T1, T2>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref KeyValuePair<T1, T2> value)
        {
            buffer.PackKeyValuePair(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out KeyValuePair<T1, T2> value)
        {
            buffer.UnpackKeyValuePair<T1, T2>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref KeyValuePair<T1, T2> value)
        {
            buffer.UnpackKeyValuePair<T1, T2>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref KeyValuePair<T1, T2> value)
        {
            var refableKey = value.Key;
            var refableValue = value.Value;
            return RePacking.SizeOf(ref refableKey) + RePacking.SizeOf(ref refableValue);
        }
    }
}
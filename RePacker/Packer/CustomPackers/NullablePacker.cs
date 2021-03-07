using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class NullablePacker<TValue> : RePackerWrapper<System.Nullable<TValue>> where TValue : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref System.Nullable<TValue> value)
        {
            buffer.PackNullable(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out System.Nullable<TValue> value)
        {
            buffer.UnpackNullable<TValue>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref System.Nullable<TValue> value)
        {
            buffer.UnpackNullable<TValue>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.Nullable<TValue> value)
        {
            if (value.HasValue)
            {
                var refable = value.Value;
                return sizeof(byte) + RePacking.SizeOf(ref refable);
            }
            else
            {
                return sizeof(byte);
            }
        }
    }
}
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class NullablePacker<TValue> : RePackerWrapper<System.Nullable<TValue>> where TValue : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref System.Nullable<TValue> value)
        {
            buffer.PackNullable(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out System.Nullable<TValue> value)
        {
            buffer.UnpackNullable<TValue>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref System.Nullable<TValue> value)
        {
            buffer.UnpackNullable<TValue>(out value);
        }
    }
}
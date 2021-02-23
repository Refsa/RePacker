using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class NullablePacker<TValue> : RePackerWrapper<Nullable<TValue>> where TValue : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(BoxedBuffer buffer, ref Nullable<TValue> value)
        {
            buffer.PackNullable(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(BoxedBuffer buffer, out Nullable<TValue> value)
        {
            buffer.UnpackNullable<TValue>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(BoxedBuffer buffer, ref Nullable<TValue> value)
        {
            buffer.UnpackNullable<TValue>(out value);
        }
    }
}
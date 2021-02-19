using System;
using System.Collections.Generic;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class NullablePacker<TValue> : RePackerWrapper<Nullable<TValue>> where TValue : unmanaged
    {
        public override void Pack(BoxedBuffer buffer, ref Nullable<TValue> value)
        {
            buffer.PackNullable(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out Nullable<TValue> value)
        {
            buffer.UnpackNullable<TValue>(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Nullable<TValue> value)
        {
            buffer.UnpackNullable<TValue>(out value);
        }
    }
}
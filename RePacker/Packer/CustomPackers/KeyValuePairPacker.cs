using System.Collections.Generic;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class KeyValuePairPacker<T1, T2> : RePackerWrapper<KeyValuePair<T1, T2>>
    {
        public override void Pack(BoxedBuffer buffer, ref KeyValuePair<T1, T2> value)
        {
            buffer.PackKeyValuePair(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out KeyValuePair<T1, T2> value)
        {
            buffer.UnpackKeyValuePair<T1, T2>(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref KeyValuePair<T1, T2> value)
        {
            buffer.UnpackKeyValuePair<T1, T2>(out value);
        }
    }
}
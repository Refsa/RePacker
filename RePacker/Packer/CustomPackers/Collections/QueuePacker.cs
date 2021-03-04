using System.Collections.Generic;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class QueuePacker<TElement> : RePackerWrapper<Queue<TElement>>
    {
        public override void Pack(Buffer buffer, ref Queue<TElement> value)
        {
            buffer.PackIEnumerable(value);
        }

        public override void Unpack(Buffer buffer, out Queue<TElement> value)
        {
            buffer.UnpackQueue<TElement>(out value);
        }

        public override void UnpackInto(Buffer buffer, ref Queue<TElement> value)
        {
            Unpack(buffer, out value);
        }
    }
}
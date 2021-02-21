using System.Collections.Generic;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class StackPacker<TElement> : RePackerWrapper<Stack<TElement>>
    {
        public override void Pack(BoxedBuffer buffer, ref Stack<TElement> value)
        {
            buffer.PackStack(value);
        }

        public override void Unpack(BoxedBuffer buffer, out Stack<TElement> value)
        {
            buffer.UnpackStack<TElement>(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Stack<TElement> value)
        {
            Unpack(buffer, out value);
        }
    }
}
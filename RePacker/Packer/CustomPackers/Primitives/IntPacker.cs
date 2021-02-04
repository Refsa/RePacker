
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(int))]
    public class IntWrapper : RePackerWrapper<int>
    {
        public override void Pack(BoxedBuffer buffer, ref int value)
        {
            buffer.Buffer.PushInt(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, ref int value)
        {
            buffer.Buffer.PopInt(out value);
        }
    }
}
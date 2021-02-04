
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(bool))]
    public class BoolWrapper : RePackerWrapper<bool>
    {
        public override void Pack(BoxedBuffer buffer, ref bool value)
        {
            buffer.Buffer.PushBool(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, ref bool value)
        {
            buffer.Buffer.PopBool(out value);
        }
    }
}
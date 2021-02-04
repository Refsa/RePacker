
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(short))]
    public class ShortWrapper : RePackerWrapper<short>
    {
        public override void Pack(BoxedBuffer buffer, ref short value)
        {
            buffer.Buffer.PushShort(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, ref short value)
        {
            buffer.Buffer.PopShort(out value);
        }
    }
}
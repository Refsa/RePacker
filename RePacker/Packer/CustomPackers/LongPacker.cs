
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(long))]
    public class LongWrapper : RePackerWrapper<long>
    {
        public override void Pack(BoxedBuffer buffer, ref long value)
        {
            buffer.Buffer.PushLong(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, ref long value)
        {
            buffer.Buffer.PopLong(out value);
        }
    }
}

using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(double))]
    public class DoubleWrapper : RePackerWrapper<double>
    {
        public override void Pack(BoxedBuffer buffer, ref double value)
        {
            buffer.Buffer.PushDouble(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, ref double value)
        {
            buffer.Buffer.PopDouble(out value);
        }
    }
}
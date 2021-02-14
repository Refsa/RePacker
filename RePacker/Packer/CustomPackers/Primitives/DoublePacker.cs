using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class DoubleWrapper : RePackerWrapper<double>
    {
        public override void Pack(BoxedBuffer buffer, ref double value)
        {
            buffer.Buffer.PushDouble(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out double value)
        {
            buffer.Buffer.PopDouble(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref double value)
        {
            buffer.Buffer.PopDouble(out value);
        }
    }
}
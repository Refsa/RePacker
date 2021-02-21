using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class DoublePacker : RePackerWrapper<double>
    {
        public static new bool IsCopyable = true;

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
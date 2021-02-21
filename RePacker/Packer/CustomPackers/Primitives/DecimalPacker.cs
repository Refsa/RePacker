using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class DecimalPacker : RePackerWrapper<decimal>
    {
        public static new bool IsCopyable = true;

        public override void Pack(BoxedBuffer buffer, ref decimal value)
        {
            buffer.Buffer.PushDecimal(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out decimal value)
        {
            buffer.Buffer.PopDecimal(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref decimal value)
        {
            buffer.Buffer.PopDecimal(out value);
        }
    }
}
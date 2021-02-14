using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class ShortWrapper : RePackerWrapper<short>
    {
        public override void Pack(BoxedBuffer buffer, ref short value)
        {
            buffer.Buffer.PushShort(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out short value)
        {
            buffer.Buffer.PopShort(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref short value)
        {
            buffer.Buffer.PopShort(out value);
        }
    }
}
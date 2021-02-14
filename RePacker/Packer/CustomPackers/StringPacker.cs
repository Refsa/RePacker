using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class StringWrapper : RePackerWrapper<string>
    {
        public override void Pack(BoxedBuffer buffer, ref string value)
        {
            buffer.Buffer.PackString(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out string value)
        {
            buffer.Buffer.UnpackString(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref string value)
        {
            buffer.Buffer.UnpackString(out value);
        }
    }
}
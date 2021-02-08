
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    // [RePackerWrapper(typeof(byte))]
    public class CharWrapper : RePackerWrapper<char>
    {
        public override void Pack(BoxedBuffer buffer, ref char value)
        {
            buffer.Buffer.PushChar(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out char value)
        {
            buffer.Buffer.PopChar(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref char value)
        {
            buffer.Buffer.PopChar(out value);
        }
    }
}
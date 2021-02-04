
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(byte))]
    public class ByteWrapper : RePackerWrapper<byte>
    {
        public override void Pack(BoxedBuffer buffer, ref byte value)
        {
            buffer.Buffer.PushByte(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, ref byte value)
        {
            buffer.Buffer.PopByte(out value);
        }
    }
}
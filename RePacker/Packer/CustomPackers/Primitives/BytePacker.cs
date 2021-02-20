using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class BytePacker : RePackerWrapper<byte>
    {
        public override void Pack(BoxedBuffer buffer, ref byte value)
        {
            buffer.Buffer.PushByte(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out byte value)
        {
            buffer.Buffer.PopByte(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref byte value)
        {
            buffer.Buffer.PopByte(out value);
        }
    }
}
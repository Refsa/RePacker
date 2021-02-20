using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class UShortPacker : RePackerWrapper<ushort>
    {
        public override void Pack(BoxedBuffer buffer, ref ushort value)
        {
            buffer.Buffer.PushUShort(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out ushort value)
        {
            buffer.Buffer.PopUShort(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref ushort value)
        {
            buffer.Buffer.PopUShort(out value);
        }
    }
}
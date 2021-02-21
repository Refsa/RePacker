using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class SBytePacker : RePackerWrapper<sbyte>
    {
        public static new bool IsCopyable = true;

        public override void Pack(BoxedBuffer buffer, ref sbyte value)
        {
            buffer.Buffer.PushSByte(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out sbyte value)
        {
            buffer.Buffer.PopSByte(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref sbyte value)
        {
            buffer.Buffer.PopSByte(out value);
        }
    }
}
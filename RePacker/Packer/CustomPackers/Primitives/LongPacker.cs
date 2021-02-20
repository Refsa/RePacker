using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class LongPacker : RePackerWrapper<long>
    {
        public override void Pack(BoxedBuffer buffer, ref long value)
        {
            buffer.Buffer.PushLong(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out long value)
        {
            buffer.Buffer.PopLong(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref long value)
        {
            buffer.Buffer.PopLong(out value);
        }
    }
}
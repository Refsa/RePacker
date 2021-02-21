using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class ULongPacker : RePackerWrapper<ulong>
    {
        public static new bool IsCopyable = true;

        public override void Pack(BoxedBuffer buffer, ref ulong value)
        {
            buffer.Buffer.PushULong(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out ulong value)
        {
            buffer.Buffer.PopULong(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref ulong value)
        {
            buffer.Buffer.PopULong(out value);
        }
    }
}
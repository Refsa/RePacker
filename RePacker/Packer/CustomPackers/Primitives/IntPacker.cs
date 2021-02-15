using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class IntPacker : RePackerWrapper<int>
    {
        public override void Pack(BoxedBuffer buffer, ref int value)
        {
            buffer.Buffer.PushInt(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out int value)
        {
            buffer.Buffer.PopInt(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref int value)
        {
            buffer.Buffer.PopInt(out value);
        }
    }
}
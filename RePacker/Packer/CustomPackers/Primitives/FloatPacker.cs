using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class FloatPacker : RePackerWrapper<float>
    {
        public static new bool IsCopyable = true;

        public override void Pack(BoxedBuffer buffer, ref float value)
        {
            buffer.Buffer.PushFloat(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out float value)
        {
            buffer.Buffer.PopFloat(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref float value)
        {
            buffer.Buffer.PopFloat(out value);
        }
    }
}
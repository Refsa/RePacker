
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(float))]
    public class FloatWrapper : RePackerWrapper<float>
    {
        public override void Pack(BoxedBuffer buffer, ref float value)
        {
            buffer.Buffer.PushFloat(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, ref float value)
        {
            buffer.Buffer.PopFloat(out value);
        }
    }
}
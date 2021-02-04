
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(float))]
    public class FloatWrapper : RePackerWrapper<float>
    {
        public override void Pack(BoxedBuffer buffer, ref float value)
        {
            buffer.Push<float>(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, ref float value)
        {
            buffer.Pop(out value);
        }
    }
}
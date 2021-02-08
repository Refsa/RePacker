
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    // [RePackerWrapper(typeof(decimal))]
    public class DecimalWrapper : RePackerWrapper<decimal>
    {
        public override void Pack(BoxedBuffer buffer, ref decimal value)
        {
            buffer.Push<decimal>(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out decimal value)
        {
            buffer.Pop(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref decimal value)
        {
            buffer.Pop(out value);
        }
    }
}
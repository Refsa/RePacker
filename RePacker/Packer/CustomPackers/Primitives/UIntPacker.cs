
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    // [RePackerWrapper(typeof(uint))]
    public class UIntWrapper : RePackerWrapper<uint>
    {
        public override void Pack(BoxedBuffer buffer, ref uint value)
        {
            buffer.Buffer.PushUInt(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out uint value)
        {
            buffer.Buffer.PopUInt(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref uint value)
        {
            buffer.Buffer.PopUInt(out value);
        }
    }
}
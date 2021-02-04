
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(uint))]
    public class UIntWrapper : RePackerWrapper<uint>
    {
        public override void Pack(BoxedBuffer buffer, ref uint value)
        {
            buffer.Buffer.PushUInt(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, ref uint value)
        {
            buffer.Buffer.PopUInt(out value);
        }
    }
}
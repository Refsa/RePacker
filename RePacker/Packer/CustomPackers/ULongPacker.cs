
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(ulong))]
    public class ULongWrapper : RePackerWrapper<ulong>
    {
        public override void Pack(BoxedBuffer buffer, ref ulong value)
        {
            buffer.Buffer.PushULong(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, ref ulong value)
        {
            buffer.Buffer.PopULong(out value);
        }
    }
}
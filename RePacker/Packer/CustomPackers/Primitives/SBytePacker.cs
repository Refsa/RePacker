
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(sbyte))]
    public class SByteWrapper : RePackerWrapper<sbyte>
    {
        public override void Pack(BoxedBuffer buffer, ref sbyte value)
        {
            buffer.Buffer.PushSByte(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out sbyte value)
        {
            buffer.Buffer.PopSByte(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref sbyte value)
        {
            buffer.Buffer.PopSByte(out value);
        }
    }
}
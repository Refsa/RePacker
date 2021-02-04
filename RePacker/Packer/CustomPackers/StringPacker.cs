
using System;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(int))]
    public class StringWrapper : RePackerWrapper<string>
    {
        public override void Pack(BoxedBuffer buffer, ref string value)
        {
            buffer.Buffer.PackString(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, ref string value)
        {
            buffer.Buffer.UnpackString(out value);
        }
    }
}
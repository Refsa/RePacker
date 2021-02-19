using System.Runtime.CompilerServices;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class StringPacker : RePackerWrapper<string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(BoxedBuffer buffer, ref string value)
        {
            buffer.Buffer.PackString(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(BoxedBuffer buffer, out string value)
        {
            buffer.Buffer.UnpackString(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(BoxedBuffer buffer, ref string value)
        {
            buffer.Buffer.UnpackString(out value);
        }
    }
}
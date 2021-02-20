using System.Runtime.CompilerServices;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class IntPacker : RePackerWrapper<int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(BoxedBuffer buffer, ref int value)
        {
            buffer.Buffer.PushInt(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(BoxedBuffer buffer, out int value)
        {
            buffer.Buffer.PopInt(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(BoxedBuffer buffer, ref int value)
        {
            buffer.Buffer.PopInt(out value);
        }
    }
}
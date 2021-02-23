using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class SBytePacker : RePackerWrapper<sbyte>
    {
        public static new bool IsCopyable = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(BoxedBuffer buffer, ref sbyte value)
        {
            buffer.Buffer.PushSByte(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(BoxedBuffer buffer, out sbyte value)
        {
            buffer.Buffer.PopSByte(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(BoxedBuffer buffer, ref sbyte value)
        {
            buffer.Buffer.PopSByte(out value);
        }
    }
}
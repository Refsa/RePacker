using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class BytePacker : RePackerWrapper<byte>
    {
        public static new bool IsCopyable = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(BoxedBuffer buffer, ref byte value)
        {
            buffer.Buffer.PushByte(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(BoxedBuffer buffer, out byte value)
        {
            buffer.Buffer.PopByte(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(BoxedBuffer buffer, ref byte value)
        {
            buffer.Buffer.PopByte(out value);
        }
    }
}
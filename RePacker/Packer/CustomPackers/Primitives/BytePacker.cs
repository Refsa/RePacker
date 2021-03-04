using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class BytePacker : RePackerWrapper<byte>
    {
        public static new bool IsCopyable = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref byte value)
        {
            buffer.PushByte(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out byte value)
        {
            buffer.PopByte(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref byte value)
        {
            buffer.PopByte(out value);
        }
    }
}
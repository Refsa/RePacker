using System.Runtime.CompilerServices;
using RePacker.Buffers;
using RePacker.Buffers.Extra;

namespace RePacker.Builder
{
    internal class SBytePacker : RePackerWrapper<sbyte>
    {
        public static new bool IsCopyable = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref sbyte value)
        {
            buffer.PackSByte(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out sbyte value)
        {
            buffer.UnpackSByte(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref sbyte value)
        {
            buffer.UnpackSByte(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref sbyte value)
        {
            return sizeof(sbyte);
        }
    }
}
using System.Runtime.CompilerServices;
using RePacker.Buffers;
using RePacker.Buffers.Extra;

namespace RePacker.Builder
{
    internal class SBytePacker : RePackerWrapper<sbyte>
    {
        public static new bool IsCopyable = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref sbyte value)
        {
            buffer.PackSByte(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out sbyte value)
        {
            buffer.UnpackSByte(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref sbyte value)
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
using System.Runtime.CompilerServices;
using RePacker.Buffers;
using RePacker.Buffers.Extra;

namespace RePacker.Builder
{
    internal class ULongPacker : RePackerWrapper<ulong>
    {
        public static new bool IsCopyable = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref ulong value)
        {
            buffer.PackULong(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out ulong value)
        {
            buffer.UnpackULong(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref ulong value)
        {
            buffer.UnpackULong(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref ulong value)
        {
            return sizeof(ulong);
        }
    }
}
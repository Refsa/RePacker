using System.Runtime.CompilerServices;
using RePacker.Buffers;
using RePacker.Buffers.Extra;

namespace RePacker.Builder
{
    internal class DecimalPacker : RePackerWrapper<decimal>
    {
        public static new bool IsCopyable = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref decimal value)
        {
            buffer.PackDecimal(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out decimal value)
        {
            buffer.UnpackDecimal(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref decimal value)
        {
            buffer.UnpackDecimal(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref decimal value)
        {
            return sizeof(decimal);
        }
    }
}
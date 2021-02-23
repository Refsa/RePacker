using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class DecimalPacker : RePackerWrapper<decimal>
    {
        public static new bool IsCopyable = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(BoxedBuffer buffer, ref decimal value)
        {
            buffer.Buffer.PushDecimal(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(BoxedBuffer buffer, out decimal value)
        {
            buffer.Buffer.PopDecimal(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(BoxedBuffer buffer, ref decimal value)
        {
            buffer.Buffer.PopDecimal(out value);
        }
    }
}
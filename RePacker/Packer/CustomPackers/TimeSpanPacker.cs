using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class TimeSpanPacker : RePackerWrapper<System.TimeSpan>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref System.TimeSpan value)
        {
            long ticks = value.Ticks;
            buffer.Pack(ref ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out System.TimeSpan value)
        {
            buffer.Unpack(out long ticks);
            value = new System.TimeSpan(ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref System.TimeSpan value)
        {
            buffer.Unpack(out long ticks);
            value = new System.TimeSpan(ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.TimeSpan value)
        {
            return sizeof(long);
        }
    }
}
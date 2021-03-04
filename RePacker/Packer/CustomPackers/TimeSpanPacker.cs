using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class TimeSpanPacker : RePackerWrapper<System.TimeSpan>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref System.TimeSpan value)
        {
            long ticks = value.Ticks;
            buffer.PushLong(ref ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out System.TimeSpan value)
        {
            buffer.PopLong(out long ticks);
            value = new System.TimeSpan(ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref System.TimeSpan value)
        {
            buffer.PopLong(out long ticks);
            value = new System.TimeSpan(ticks);
        }
    }
}
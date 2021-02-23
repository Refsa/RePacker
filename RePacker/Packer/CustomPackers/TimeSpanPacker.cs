using System;
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class TimeSpanPacker : RePackerWrapper<TimeSpan>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(BoxedBuffer buffer, ref TimeSpan value)
        {
            long ticks = value.Ticks;
            buffer.Buffer.PushLong(ref ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(BoxedBuffer buffer, out TimeSpan value)
        {
            buffer.Buffer.PopLong(out long ticks);
            value = new TimeSpan(ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(BoxedBuffer buffer, ref TimeSpan value)
        {
            buffer.Buffer.PopLong(out long ticks);
            value = new TimeSpan(ticks);
        }
    }
}
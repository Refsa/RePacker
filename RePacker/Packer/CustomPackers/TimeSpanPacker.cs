using System;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class TimeSpanPacker : RePackerWrapper<TimeSpan>
    {
        public override void Pack(BoxedBuffer buffer, ref TimeSpan value)
        {
            long ticks = value.Ticks;
            buffer.Buffer.PushLong(ref ticks);
        }

        public override void Unpack(BoxedBuffer buffer, out TimeSpan value)
        {
            buffer.Buffer.PopLong(out long ticks);
            value = new TimeSpan(ticks);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref TimeSpan value)
        {
            buffer.Buffer.PopLong(out long ticks);
            value = new TimeSpan(ticks);
        }
    }
}
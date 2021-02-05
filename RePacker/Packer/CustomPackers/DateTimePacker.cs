
using System;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(int))]
    public class DateTimeWrapper : RePackerWrapper<DateTime>
    {
        public override void Pack(BoxedBuffer buffer, ref DateTime value)
        {
            long ticks = value.Ticks;
            buffer.Buffer.PushLong(ref ticks);
        }

        public override void Unpack(BoxedBuffer buffer, out DateTime value)
        {
            buffer.Buffer.PopLong(out long ticks);
            value = new DateTime(ticks);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref DateTime value)
        {
            buffer.Buffer.PopLong(out long ticks);
            value = new DateTime(ticks);
        }
    }
}
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class DateTimePacker : RePackerWrapper<System.DateTime>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref System.DateTime value)
        {
            long ticks = value.Ticks;
            buffer.Pack(ref ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out System.DateTime value)
        {
            buffer.Unpack(out long ticks);
            value = new System.DateTime(ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref System.DateTime value)
        {
            buffer.Unpack(out long ticks);
            value = new System.DateTime(ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.DateTime value)
        {
            return sizeof(long);
        }
    }
}
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class DoublePacker : RePackerWrapper<double>
    {
        public static new bool IsCopyable = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref double value)
        {
            buffer.PushDouble(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out double value)
        {
            buffer.PopDouble(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref double value)
        {
            buffer.PopDouble(out value);
        }
    }
}
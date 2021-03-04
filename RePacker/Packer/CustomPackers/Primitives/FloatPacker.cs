using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class FloatPacker : RePackerWrapper<float>
    {
        public static new bool IsCopyable = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref float value)
        {
            buffer.PushFloat(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out float value)
        {
            buffer.PopFloat(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref float value)
        {
            buffer.PopFloat(out value);
        }
    }
}
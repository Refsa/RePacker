using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class BufferPacker : RePackerWrapper<ReBuffer>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref ReBuffer value)
        {
            buffer.PackBuffer(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out ReBuffer value)
        {
           value = buffer.UnpackBuffer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref ReBuffer value)
        {
            Unpack(buffer, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref ReBuffer value)
        {
            return value.Length() + sizeof(ulong) + sizeof(int) * 2;
        }
    }
}
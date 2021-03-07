using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class BufferPacker : RePackerWrapper<Buffer>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref Buffer value)
        {
            buffer.Copy(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out Buffer value)
        {
            value = new Buffer(buffer.Length());
            value.Copy(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref Buffer value)
        {
            value.Copy(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref Buffer value)
        {
            return value.Length();
        }
    }
}
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class StringPacker : RePackerWrapper<string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref string value)
        {
            buffer.PackString(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out string value)
        {
            buffer.UnpackString(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref string value)
        {
            buffer.UnpackString(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref string value)
        {
            if (value == null)
            {
                return sizeof(ulong);
            }
            else
            {
                return System.Text.Encoding.UTF8.GetByteCount(value) + sizeof(ulong);
            }
        }
    }
}
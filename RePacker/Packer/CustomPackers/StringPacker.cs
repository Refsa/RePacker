using System.Runtime.CompilerServices;
using RePacker.Buffers;
using RePacker.Unsafe;

namespace RePacker.Builder
{
    internal class StringPacker : RePackerWrapper<string>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref string value)
        {
            buffer.PackString(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out string value)
        {
            buffer.UnpackString(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref string value)
        {
            buffer.UnpackString(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref string value)
        {
            return StringHelper.SizeOf(value) + sizeof(ulong);
        }
    }
}
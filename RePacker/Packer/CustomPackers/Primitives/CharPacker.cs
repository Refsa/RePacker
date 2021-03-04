using System.Runtime.CompilerServices;
using RePacker.Buffers;
using RePacker.Buffers.Extra;

namespace RePacker.Builder
{
    internal class CharPacker : RePackerWrapper<char>
    {
        public static new bool IsCopyable = true;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref char value)
        {
            buffer.PackChar(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out char value)
        {
            buffer.UnpackChar(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref char value)
        {
            buffer.UnpackChar(out value);
        }
    }
}
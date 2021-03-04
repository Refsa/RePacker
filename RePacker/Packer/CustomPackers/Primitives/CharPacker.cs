using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class CharPacker : RePackerWrapper<char>
    {
        public static new bool IsCopyable = true;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref char value)
        {
            buffer.PushChar(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out char value)
        {
            buffer.PopChar(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref char value)
        {
            buffer.PopChar(out value);
        }
    }
}
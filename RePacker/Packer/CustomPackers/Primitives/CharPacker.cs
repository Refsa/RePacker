using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class CharPacker : RePackerWrapper<char>
    {
        public static new bool IsCopyable = true;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(BoxedBuffer buffer, ref char value)
        {
            buffer.Buffer.PushChar(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(BoxedBuffer buffer, out char value)
        {
            buffer.Buffer.PopChar(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(BoxedBuffer buffer, ref char value)
        {
            buffer.Buffer.PopChar(out value);
        }
    }
}
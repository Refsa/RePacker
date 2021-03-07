using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class BufferPacker : RePackerWrapper<ReBuffer>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref ReBuffer value)
        {
            int readCursor = value.ReadCursor();
            int writeCursor = value.WriteCursor();
            buffer.Pack(ref readCursor);
            buffer.Pack(ref writeCursor);
            buffer.PackArray(value.Array, value.ReadCursor(), value.Length());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out ReBuffer value)
        {
            buffer.Unpack(out int readCursor);
            buffer.Unpack(out int writeCursor);

            buffer.UnpackArray<byte>(out var data);

            value = new ReBuffer(data);
            value.SetReadCursor(readCursor);
            value.SetWriteCursor(writeCursor);
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
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class BufferPacker : RePackerWrapper<Buffer>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref Buffer value)
        {
            int readCursor = value.ReadCursor();
            int writeCursor = value.WriteCursor();
            buffer.Pack(ref readCursor);
            buffer.Pack(ref writeCursor);
            buffer.PackArray(value.Array, value.ReadCursor(), value.Length());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out Buffer value)
        {
            buffer.Unpack(out int readCursor);
            buffer.Unpack(out int writeCursor);

            buffer.UnpackArray<byte>(out var data);

            value = new Buffer(data);
            value.SetReadCursor(readCursor);
            value.SetWriteCursor(writeCursor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref Buffer value)
        {
            Unpack(buffer, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref Buffer value)
        {
            return value.Length() + sizeof(ulong) + sizeof(int) * 2;
        }
    }
}
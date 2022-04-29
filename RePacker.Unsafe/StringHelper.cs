using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace RePacker.Unsafe
{
    public unsafe static class StringHelper
    {
        static readonly UTF8Encoding stringEncoder;

        static StringHelper()
        {
            stringEncoder = new UTF8Encoding();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string GetString(byte[] data, int offset, int length)
        {
            fixed (byte* src = &data[offset])
            {
                return stringEncoder.GetString(src, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe string GetString(ref NativeBlob blob, int offset, int length)
        {
            return stringEncoder.GetString(blob.Data + offset, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CopyString(string value, byte[] data, int offset)
        {
            return stringEncoder.GetBytes(value, 0, value.Length, data, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CopyString(string value, ref NativeBlob dst, int offset)
        {
            int bytes = SizeOf(value);
            var dstPtr = dst.Data + offset;
            fixed(char* chars = value)
            {
                stringEncoder.GetBytes(chars, value.Length, dstPtr, bytes);
            }

            return bytes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SizeOf(string value)
        {
            if (value == null) return 0;

            return stringEncoder.GetByteCount(value);

            /* fixed (char* asC = value)
            {
                return stringEncoder.GetByteCount(asC, value.Length);
            } */
        }
    }
}
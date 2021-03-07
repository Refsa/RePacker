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
        public static int CopyString(string value, byte[] data, int offset)
        {
            return stringEncoder.GetBytes(value, 0, value.Length, data, offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SizeOf(string value)
        {
            if (value == null) return 0;

            fixed (char* asC = value)
            {
                return stringEncoder.GetByteCount(asC, value.Length);
            }
        }
    }
}
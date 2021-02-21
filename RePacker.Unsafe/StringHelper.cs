using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Numerics;

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
            // char* target = stackalloc char[charCount];
            // fixed (byte* src = &data[offset])
            // {
            //     stringEncoder.GetChars(src, length, target, charCount);
            // }

            // fixed (byte* src = &data[offset])
            // {
            //     sbyte* ssrc = (sbyte*)src;
            //     return new string(ssrc, 0, length);
            // }

            fixed (byte* src = &data[offset])
            {
                return stringEncoder.GetString(src, length);
            }

            // return stringEncoder.GetString(data, offset, length);

            /* int charCount = stringEncoder.GetCharCount(data, offset, length);

            char* dest = stackalloc char[charCount];

            fixed (byte* src = &data[offset])
            {
                widen_bytes_simd(dest, src, length);
            }

            return new string(dest); */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CopyString(string value, byte[] data, int offset)
        {
            return stringEncoder.GetBytes(value, 0, value.Length, data, offset);
        }
    }
}
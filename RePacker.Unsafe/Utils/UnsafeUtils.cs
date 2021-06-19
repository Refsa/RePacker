using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RePacker.Unsafe
{
    public static class UnsafeUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SizeOf<T>() where T : unmanaged
        {
            return sizeof(T);
            // if (typeof(T) == typeof(char))
            // {
            //     return 2;
            // }
            // else if (typeof(T) == typeof(decimal))
            // {
            //     return 16;
            // }
            // else if (typeof(T).IsEnum)
            // {
            //     return sizeof(T);
            // }

            // return Marshal.SizeOf<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe T ToBigEndian<T>(T value) where T : unmanaged
        {
            byte* asP = (byte*)&value;

            int size = sizeof(T);
            int halfSize = size / 2;

            for (int i = 0; i < halfSize; i++)
            {
                byte temp = *(asP + size - i - 1);
                *(asP + size - i - 1) = *(asP + i);
                *(asP + i) = temp;
            }

            return value;
        }
    }
}
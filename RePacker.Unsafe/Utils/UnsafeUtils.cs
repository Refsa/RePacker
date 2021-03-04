using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RePacker.Unsafe
{
    public static class UnsafeUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int SizeOf<T>() where T : unmanaged
        {
            if (typeof(T) == typeof(char))
            {
                return 2;
            }
            else if (typeof(T) == typeof(decimal))
            {
                return 16;
            }
            else if (typeof(T).IsEnum)
            {
                return sizeof(T);
            }

            return Marshal.SizeOf<T>();
        }
    }
}
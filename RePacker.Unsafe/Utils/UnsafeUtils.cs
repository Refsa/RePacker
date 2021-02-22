

using System.Runtime.InteropServices;

namespace RePacker.Unsafe
{
    public static class UnsafeUtils
    {
        public static int SizeOf<T>() where T : unmanaged
        {
            if (typeof(T) == typeof(char))
            {
                return 2;
            }
            else if (typeof(T) == typeof(decimal))
            {
                return 16;
            }
            
            return Marshal.SizeOf<T>();
        }
    }
}
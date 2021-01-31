

using System;

namespace Refsa.RePacker.Utils
{
    public static class TypeExt
    {
        ref struct UnmanagedProxy<T> where T : unmanaged { }

        public static bool IsUnmanaged(this Type self)
        {
            try
            {
                typeof(UnmanagedProxy<>).MakeGenericType(self);

                if (!self.IsValueType) return false;
                if (Nullable.GetUnderlyingType(self) != null) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
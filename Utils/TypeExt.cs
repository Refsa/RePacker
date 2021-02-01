

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

        public static bool IsUnmanagedStruct(this Type self)
        {
            try
            {
                typeof(UnmanagedProxy<>).MakeGenericType(self);
                return true;
            }
            catch
            {
                return false;
            }
        }

        ref struct IsStructProxy<T> where T : struct { }
        public static bool IsStruct(this Type self)
        {
            try
            {
                typeof(IsStructProxy<>).MakeGenericType(self);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
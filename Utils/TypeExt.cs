

using System;

namespace Refsa.RePacker.Utils
{
    public static class TypeExt
    {
        struct BlittableTester<T> where T : unmanaged { }

        public static bool IsBlittable(this Type self)
        {
            if (self.BaseType == typeof(ValueType))
            {
                try
                {
                    object instance = Activator.CreateInstance(
                        typeof(BlittableTester<>).MakeGenericType(self));

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                if (!self.IsValueType) return false;
                if (Nullable.GetUnderlyingType(self) != null) return true;
                return false;
            }
        }


    }
}
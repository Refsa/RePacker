using System;
using System.Collections.Generic;

namespace RePacker.Utils
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

        public static bool HasInterface<T>(this Type type)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType == type)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasInterface(this Type type, Type interfaceType)
        {
            RePacking.Settings.Log.Log(type + " - " + interfaceType.MakeGenericType(type.GenericTypeArguments[0]));
            foreach (var it in type.GetInterfaces())
            {
                if (interfaceType == it)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
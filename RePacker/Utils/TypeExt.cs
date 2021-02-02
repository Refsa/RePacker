

using System;
using System.Collections.Generic;

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
            Console.WriteLine(type + " - " + interfaceType.MakeGenericType(type.GenericTypeArguments[0]));
            foreach (var it in type.GetInterfaces())
            {
                if (interfaceType == it)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsOfIList(this Type type)
        {
            var genericListType = typeof(IList<>).MakeGenericType(type.GenericTypeArguments[0]);

            return
                type == genericListType ||
                typeof(IList<>).MakeGenericType(type.GenericTypeArguments[0]).IsAssignableFrom(type);
        }

        public static bool IsOfIEnumerable(this Type type)
        {
            var genericListType = typeof(IEnumerable<>).MakeGenericType(type.GenericTypeArguments[0]);

            return
                type == genericListType ||
                genericListType.IsAssignableFrom(type);
        }

        public static bool IsOfDictionary(this Type type)
        {
            var genericDictType = typeof(Dictionary<,>).MakeGenericType(type.GenericTypeArguments);

            return
                type == genericDictType ||
                genericDictType.IsAssignableFrom(type);
        }
    }
}
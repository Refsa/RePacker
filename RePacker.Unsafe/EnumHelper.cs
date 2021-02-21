using System;
using System.Linq.Expressions;

namespace RePacker.Unsafe
{
    public static class EnumHelper
    {
        public static unsafe TDest ToUnderlyingType<TEnum, TDest>(this TEnum self)
            where TEnum : unmanaged, Enum
            where TDest : unmanaged
        {
            return (TDest)(object)self;
        }

        public static unsafe TEnum ToEnum<TVal, TEnum>(ref TVal val)
            where TVal : unmanaged
            where TEnum : unmanaged, Enum
        {
            return (TEnum)(object)val;
        }
    }
}
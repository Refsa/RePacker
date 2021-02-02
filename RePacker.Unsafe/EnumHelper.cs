using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Refsa.RePacker.Unsafe
{
    public static class EnumHelper
    {
        public static TDest ToUnderlyingType<TEnum, TDest>(this TEnum self)
            where TEnum : unmanaged, Enum
            where TDest : unmanaged
        {
            return System.Runtime.CompilerServices.Unsafe.As<TEnum, TDest>(ref self);
        }

        public static TEnum ToEnum<TVal, TEnum>(ref TVal val)
            where TVal : unmanaged
            where TEnum : unmanaged, Enum
        {
            return System.Runtime.CompilerServices.Unsafe.As<TVal, TEnum>(ref val);
        }
    }
}
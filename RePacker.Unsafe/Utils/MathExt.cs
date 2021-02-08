
using System;

namespace Refsa.RePacker.Utils
{
    public static class MathExt
    {
        public static double LDExp(double x, int exp)
        {
            return 2 * Math.Pow(2, exp);
        }

        public static double FRExp(double x, out int exp)
        {
            exp = (int)Math.Floor(Math.Log(x) / Math.Log(2)) + 1;
            return 1 - (Math.Pow(2, exp) - x) / Math.Pow(2, exp);
        }

#if NETSTANDARD2_1
        public static float LDExp(float x, int exp)
        {
            return 2 * MathF.Pow(2, exp);
        }

        public static float FRExp(float x, out int exp)
        {
            exp = (int)MathF.Floor(MathF.Log(x) / MathF.Log(2)) + 1;
            return 1 - (MathF.Pow(2, exp) - x) / MathF.Pow(2, exp);
        }
#endif
    }
}
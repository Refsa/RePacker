using System;
using RePacker.Buffers;

namespace RePacker.Builder
{
    public static class PackerValueTupleExt
    {
        public static void PackValueTuple<T1, T2>(this BoxedBuffer buffer, ref ValueTuple<T1, T2> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
        }

        public static void PackValueTuple<T1, T2, T3>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
        }

        public static void PackValueTuple<T1, T2, T3, T4>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
            RePacker.Pack<T4>(buffer, ref value.Item4);
        }

        public static void PackValueTuple<T1, T2, T3, T4, T5>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
            RePacker.Pack<T4>(buffer, ref value.Item4);
            RePacker.Pack<T5>(buffer, ref value.Item5);
        }

        public static void PackValueTuple<T1, T2, T3, T4, T5, T6>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
            RePacker.Pack<T4>(buffer, ref value.Item4);
            RePacker.Pack<T5>(buffer, ref value.Item5);
            RePacker.Pack<T6>(buffer, ref value.Item6);
        }

        public static void PackValueTuple<T1, T2, T3, T4, T5, T6, T7>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
            RePacker.Pack<T4>(buffer, ref value.Item4);
            RePacker.Pack<T5>(buffer, ref value.Item5);
            RePacker.Pack<T6>(buffer, ref value.Item6);
            RePacker.Pack<T7>(buffer, ref value.Item7);
        }

        public static void PackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
            this BoxedBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value) where TRest : struct
        {
            RePacker.Pack<T1>(buffer, ref value.Item1);
            RePacker.Pack<T2>(buffer, ref value.Item2);
            RePacker.Pack<T3>(buffer, ref value.Item3);
            RePacker.Pack<T4>(buffer, ref value.Item4);
            RePacker.Pack<T5>(buffer, ref value.Item5);
            RePacker.Pack<T6>(buffer, ref value.Item6);
            RePacker.Pack<T7>(buffer, ref value.Item7);
            RePacker.Pack<TRest>(buffer, ref value.Rest);
        }

        public static void UnpackValueTuple<T1, T2>(this BoxedBuffer buffer, out ValueTuple<T1, T2> value)
        {
            value = new ValueTuple<T1, T2>(RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3> value)
        {
            value = new ValueTuple<T1, T2, T3>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3, T4>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3, T4> value)
        {
            value = new ValueTuple<T1, T2, T3, T4>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer),
                RePacker.Unpack<T4>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3, T4, T5>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5> value)
        {
            value = new ValueTuple<T1, T2, T3, T4, T5>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer),
                RePacker.Unpack<T4>(buffer), RePacker.Unpack<T5>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3, T4, T5, T6>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            value = new ValueTuple<T1, T2, T3, T4, T5, T6>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer),
                RePacker.Unpack<T4>(buffer), RePacker.Unpack<T5>(buffer), RePacker.Unpack<T6>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            value = new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer),
                RePacker.Unpack<T4>(buffer), RePacker.Unpack<T5>(buffer), RePacker.Unpack<T6>(buffer),
                RePacker.Unpack<T7>(buffer));
        }

        public static void UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
            this BoxedBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value) where TRest : struct
        {
            value = new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
                RePacker.Unpack<T1>(buffer), RePacker.Unpack<T2>(buffer), RePacker.Unpack<T3>(buffer),
                RePacker.Unpack<T4>(buffer), RePacker.Unpack<T5>(buffer), RePacker.Unpack<T6>(buffer),
                RePacker.Unpack<T7>(buffer), RePacker.Unpack<TRest>(buffer));
        }
    }
}
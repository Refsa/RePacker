using System;
using RePacker.Buffers;

using ReBuffer = RePacker.Buffers.ReBuffer;

namespace RePacker.Builder
{
    public static class PackerValueTupleExt
    {
        /// <summary>
        /// Packs a managed (T1) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void PackValueTuple<T1>(
            this ReBuffer buffer, ref ValueTuple<T1> value)
        {
            RePacking.Pack<T1>(buffer, ref value.Item1);
        }

        /// <summary>
        /// Packs a managed (T1, T2) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void PackValueTuple<T1, T2>(
            this ReBuffer buffer, ref ValueTuple<T1, T2> value)
        {
            RePacking.Pack<T1>(buffer, ref value.Item1);
            RePacking.Pack<T2>(buffer, ref value.Item2);
        }

        /// <summary>
        /// Packs a managed (T1, T2, T3) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void PackValueTuple<T1, T2, T3>(
            this ReBuffer buffer,
            ref ValueTuple<T1, T2, T3> value)
        {
            RePacking.Pack<T1>(buffer, ref value.Item1);
            RePacking.Pack<T2>(buffer, ref value.Item2);
            RePacking.Pack<T3>(buffer, ref value.Item3);
        }

        /// <summary>
        /// Packs a managed (T1, T2, T3, T4) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void PackValueTuple<T1, T2, T3, T4>(
            this ReBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4> value)
        {
            RePacking.Pack<T1>(buffer, ref value.Item1);
            RePacking.Pack<T2>(buffer, ref value.Item2);
            RePacking.Pack<T3>(buffer, ref value.Item3);
            RePacking.Pack<T4>(buffer, ref value.Item4);
        }

        /// <summary>
        /// Packs a managed (T1, T2, T3, T4, T5) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void PackValueTuple<T1, T2, T3, T4, T5>(
            this ReBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5> value)
        {
            RePacking.Pack<T1>(buffer, ref value.Item1);
            RePacking.Pack<T2>(buffer, ref value.Item2);
            RePacking.Pack<T3>(buffer, ref value.Item3);
            RePacking.Pack<T4>(buffer, ref value.Item4);
            RePacking.Pack<T5>(buffer, ref value.Item5);
        }

        /// <summary>
        /// Packs a managed (T1, T2, T3, T4, T5, T6) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void PackValueTuple<T1, T2, T3, T4, T5, T6>(
            this ReBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            RePacking.Pack<T1>(buffer, ref value.Item1);
            RePacking.Pack<T2>(buffer, ref value.Item2);
            RePacking.Pack<T3>(buffer, ref value.Item3);
            RePacking.Pack<T4>(buffer, ref value.Item4);
            RePacking.Pack<T5>(buffer, ref value.Item5);
            RePacking.Pack<T6>(buffer, ref value.Item6);
        }

        /// <summary>
        /// Packs a managed (T1, T2, T3, T4, T5, T6, T7) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void PackValueTuple<T1, T2, T3, T4, T5, T6, T7>(
            this ReBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            RePacking.Pack<T1>(buffer, ref value.Item1);
            RePacking.Pack<T2>(buffer, ref value.Item2);
            RePacking.Pack<T3>(buffer, ref value.Item3);
            RePacking.Pack<T4>(buffer, ref value.Item4);
            RePacking.Pack<T5>(buffer, ref value.Item5);
            RePacking.Pack<T6>(buffer, ref value.Item6);
            RePacking.Pack<T7>(buffer, ref value.Item7);
        }

        /// <summary>
        /// Packs a managed (T1, T2, T3, T4, T5, T6, T7, TRest) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void PackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
            this ReBuffer buffer,
            ref ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value) where TRest : struct
        {
            RePacking.Pack<T1>(buffer, ref value.Item1);
            RePacking.Pack<T2>(buffer, ref value.Item2);
            RePacking.Pack<T3>(buffer, ref value.Item3);
            RePacking.Pack<T4>(buffer, ref value.Item4);
            RePacking.Pack<T5>(buffer, ref value.Item5);
            RePacking.Pack<T6>(buffer, ref value.Item6);
            RePacking.Pack<T7>(buffer, ref value.Item7);
            RePacking.Pack<TRest>(buffer, ref value.Rest);
        }

        /// <summary>
        /// Unpacks a managed (T1) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void UnpackValueTuple<T1>(
            this ReBuffer buffer, out ValueTuple<T1> value)
        {
            value = new ValueTuple<T1>(RePacking.Unpack<T1>(buffer));
        }

        /// <summary>
        /// Unpacks a managed (T1, T2) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void UnpackValueTuple<T1, T2>(
            this ReBuffer buffer, out ValueTuple<T1, T2> value)
        {
            value = new ValueTuple<T1, T2>(RePacking.Unpack<T1>(buffer), RePacking.Unpack<T2>(buffer));
        }

        /// <summary>
        /// Unpacks a managed (T1, T2, T3) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void UnpackValueTuple<T1, T2, T3>(
            this ReBuffer buffer,
            out ValueTuple<T1, T2, T3> value)
        {
            value = new ValueTuple<T1, T2, T3>(
                RePacking.Unpack<T1>(buffer), RePacking.Unpack<T2>(buffer), RePacking.Unpack<T3>(buffer));
        }

        /// <summary>
        /// Unpacks a managed (T1, T2, T3, T4) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void UnpackValueTuple<T1, T2, T3, T4>(
            this ReBuffer buffer,
            out ValueTuple<T1, T2, T3, T4> value)
        {
            value = new ValueTuple<T1, T2, T3, T4>(
                RePacking.Unpack<T1>(buffer), RePacking.Unpack<T2>(buffer), RePacking.Unpack<T3>(buffer),
                RePacking.Unpack<T4>(buffer));
        }

        /// <summary>
        /// Unpacks a managed (T1, T2, T3, T4, T5) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void UnpackValueTuple<T1, T2, T3, T4, T5>(
            this ReBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5> value)
        {
            value = new ValueTuple<T1, T2, T3, T4, T5>(
                RePacking.Unpack<T1>(buffer), RePacking.Unpack<T2>(buffer), RePacking.Unpack<T3>(buffer),
                RePacking.Unpack<T4>(buffer), RePacking.Unpack<T5>(buffer));
        }

        /// <summary>
        /// Unpacks a managed (T1, T2, T3, T4, T5, T6) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void UnpackValueTuple<T1, T2, T3, T4, T5, T6>(
            this ReBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            value = new ValueTuple<T1, T2, T3, T4, T5, T6>(
                RePacking.Unpack<T1>(buffer), RePacking.Unpack<T2>(buffer), RePacking.Unpack<T3>(buffer),
                RePacking.Unpack<T4>(buffer), RePacking.Unpack<T5>(buffer), RePacking.Unpack<T6>(buffer));
        }

        /// <summary>
        /// Unpacks a managed (T1, T2, T3, T4, T5, T6, T7) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7>(
            this ReBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            value = new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(
                RePacking.Unpack<T1>(buffer), RePacking.Unpack<T2>(buffer), RePacking.Unpack<T3>(buffer),
                RePacking.Unpack<T4>(buffer), RePacking.Unpack<T5>(buffer), RePacking.Unpack<T6>(buffer),
                RePacking.Unpack<T7>(buffer));
        }

        /// <summary>
        /// Unpacks a managed (T1, T2, T3, T4, T5, T6, T7, TRest) tuple
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <typeparam name="T1"></typeparam>
        public static void UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
            this ReBuffer buffer,
            out ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value) where TRest : struct
        {
            value = new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(
                RePacking.Unpack<T1>(buffer), RePacking.Unpack<T2>(buffer), RePacking.Unpack<T3>(buffer),
                RePacking.Unpack<T4>(buffer), RePacking.Unpack<T5>(buffer), RePacking.Unpack<T6>(buffer),
                RePacking.Unpack<T7>(buffer), RePacking.Unpack<TRest>(buffer));
        }
    }
}
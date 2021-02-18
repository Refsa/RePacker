using System;
using System.Collections.Generic;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    public static class BoxedBufferExt
    {
        public static void PackString(this BoxedBuffer buffer, ref string str)
        {
            buffer.Buffer.PackString(ref str);
        }

        public static string UnpackString(this BoxedBuffer buffer)
        {
            return buffer.Buffer.UnpackString();
        }

        public static void UnpackString(this BoxedBuffer buffer, out string str)
        {
            buffer.Buffer.UnpackString(out str);
        }

        public static void PackDateTime(this BoxedBuffer buffer, ref DateTime value)
        {
            long ticks = value.Ticks;
            buffer.Buffer.PushLong(ref ticks);
        }

        public static void UnpackDateTime(this BoxedBuffer buffer, out DateTime value)
        {
            buffer.Buffer.PopLong(out long ticks);
            value = new DateTime(ticks);
        }

        public static void PackKeyValuePair<T1, T2>(this BoxedBuffer buffer, ref KeyValuePair<T1, T2> value)
        {
            var k = value.Key;
            RePacker.Pack<T1>(buffer, ref k);

            var v = value.Value;
            RePacker.Pack<T2>(buffer, ref v);
        }

        public static void UnpackKeyValuePair<T1, T2>(this BoxedBuffer buffer, out KeyValuePair<T1, T2> value)
        {
            value = new KeyValuePair<T1, T2>(
                RePacker.Unpack<T1>(buffer),
                RePacker.Unpack<T2>(buffer)
            );
        }

        public static void Pack<T>(this BoxedBuffer self, ref T value)
        {
            RePacker.Pack(self, ref value);
        }

        public static T Unpack<T>(this BoxedBuffer self)
        {
            return RePacker.Unpack<T>(self);
        }
    }
}
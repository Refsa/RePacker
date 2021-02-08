using System;
using System.Collections.Generic;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    public static class BoxedBufferExt
    {
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
    }
}
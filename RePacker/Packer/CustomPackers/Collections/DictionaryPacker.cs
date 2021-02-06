
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(int))]
    public class DictionaryWrapper<TKey, TValue> : RePackerWrapper<Dictionary<TKey, TValue>>
    {
        public override void Pack(BoxedBuffer buffer, ref Dictionary<TKey, TValue> value)
        {
            buffer.PackIEnumerable(value.Keys);
            buffer.PackIEnumerable(value.Values);
        }

        public override void Unpack(BoxedBuffer buffer, out Dictionary<TKey, TValue> value)
        {
            buffer.UnpackIEnumerable<TKey>(IEnumerableType.None, out var keys);
            buffer.UnpackIEnumerable<TValue>(IEnumerableType.None, out var values);

            value = new Dictionary<TKey, TValue>(keys.Zip(values, (k, v) => new KeyValuePair<TKey, TValue>(k, v)));
        }
    }
}
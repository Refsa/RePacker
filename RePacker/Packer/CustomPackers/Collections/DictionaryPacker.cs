
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Refsa.RePacker.Buffers;
using static Refsa.RePacker.PackerCollectionsExt;

namespace Refsa.RePacker.Builder
{
    // [RePackerWrapper(typeof(int))]
    public class DictionaryWrapper<TKey, TValue> : RePackerWrapper<Dictionary<TKey, TValue>>
    {
        public override void Pack(BoxedBuffer buffer, ref Dictionary<TKey, TValue> value)
        {
            // buffer.PackIEnumerable(value.AsEnumerable());
            buffer.PackIEnumerable(value.Keys);
            buffer.PackIEnumerable(value.Values);
        }

        public override void Unpack(BoxedBuffer buffer, out Dictionary<TKey, TValue> value)
        {
            buffer.UnpackIEnumerable<TKey>(IEnumerableType.None, out var keys);
            buffer.UnpackIEnumerable<TValue>(IEnumerableType.None, out var values);
            value = keys.Zip(values, (k, v) => new KeyValuePair<TKey, TValue>(k, v)).ToDictionary(x => x.Key, x => x.Value);

            // buffer.UnpackIEnumerable<KeyValuePair<TKey, TValue>>(IEnumerableType.None, out var kvCollection);
            // value = kvCollection.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
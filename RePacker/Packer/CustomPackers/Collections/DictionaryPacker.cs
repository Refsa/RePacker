using System.Linq;
using System.Collections.Generic;
using RePacker.Buffers;
using static RePacker.Builder.PackerCollectionsExt;

namespace RePacker.Builder
{
    internal class DictionaryPacker<TKey, TValue> : RePackerWrapper<Dictionary<TKey, TValue>>
    {
        public override void Pack(BoxedBuffer buffer, ref Dictionary<TKey, TValue> value)
        {
            // buffer.PackIEnumerable(value.Keys);
            // buffer.PackIEnumerable(value.Values);

            var kvs = value.AsEnumerable();
            buffer.PackIEnumerable(kvs);
        }

        public override void Unpack(BoxedBuffer buffer, out Dictionary<TKey, TValue> value)
        {
            // buffer.UnpackIEnumerable<TKey>(out var keys);
            // buffer.UnpackIEnumerable<TValue>(out var values);
            // value = keys.Zip(values, (k, v) => new KeyValuePair<TKey, TValue>(k, v)).ToDictionary(x => x.Key, x => x.Value);

            buffer.UnpackIEnumerable(out IEnumerable<KeyValuePair<TKey, TValue>> kvCollection);
            value = kvCollection.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
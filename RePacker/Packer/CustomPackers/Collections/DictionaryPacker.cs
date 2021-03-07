using System.Linq;
using System.Collections.Generic;
using RePacker.Buffers;
using static RePacker.Builder.PackerCollectionsExt;
using System.Runtime.CompilerServices;

namespace RePacker.Builder
{
    internal class DictionaryPacker<TKey, TValue> : RePackerWrapper<Dictionary<TKey, TValue>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref Dictionary<TKey, TValue> value)
        {
            // buffer.PackIEnumerable(value.Keys);
            // buffer.PackIEnumerable(value.Values);

            var kvs = value.AsEnumerable();
            buffer.PackIEnumerable(kvs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out Dictionary<TKey, TValue> value)
        {
            // buffer.UnpackIEnumerable<TKey>(out var keys);
            // buffer.UnpackIEnumerable<TValue>(out var values);
            // value = keys.Zip(values, (k, v) => new KeyValuePair<TKey, TValue>(k, v)).ToDictionary(x => x.Key, x => x.Value);

            buffer.UnpackIEnumerable(out IEnumerable<KeyValuePair<TKey, TValue>> kvCollection);
            value = kvCollection.ToDictionary(x => x.Key, x => x.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref Dictionary<TKey, TValue> value)
        {
            return PackerCollectionsExt.SizeOfColleciton<KeyValuePair<TKey, TValue>>(value.GetEnumerator());
        }
    }
}
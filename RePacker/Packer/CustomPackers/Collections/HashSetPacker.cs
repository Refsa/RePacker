
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    // [RePackerWrapper(typeof(int))]
    public class HashSetWrapper<TElement> : RePackerWrapper<HashSet<TElement>>
    {
        public override void Pack(BoxedBuffer buffer, ref HashSet<TElement> value)
        {
            buffer.PackIEnumerable(value);
        }

        public override void Unpack(BoxedBuffer buffer, out HashSet<TElement> value)
        {
            buffer.UnpackIEnumerable<TElement>(IEnumerableType.HashSet, out var ien);
            value = (HashSet<TElement>)ien;
        }
    }
}
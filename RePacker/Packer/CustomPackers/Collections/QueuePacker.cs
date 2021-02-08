
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    // [RePackerWrapper(typeof(int))]
    public class QueueWrapper<TElement> : RePackerWrapper<Queue<TElement>>
    {
        public override void Pack(BoxedBuffer buffer, ref Queue<TElement> value)
        {
            buffer.PackIEnumerable(value);
        }

        public override void Unpack(BoxedBuffer buffer, out Queue<TElement> value)
        {
            buffer.UnpackIEnumerable<TElement>(PackerCollectionsExt.IEnumerableType.Queue, out var ien);
            value = (Queue<TElement>)ien;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    // [RePackerWrapper(typeof(int))]
    public class ListWrapper<TElement> : RePackerWrapper<List<TElement>>
    {
        public override void Pack(BoxedBuffer buffer, ref List<TElement> value)
        {
            buffer.PackIList(value);
        }

        public override void Unpack(BoxedBuffer buffer, out List<TElement> value)
        {
            buffer.UnpackIList<TElement>(out var ilist);
            value = (List<TElement>)ilist;
        }
    }
}
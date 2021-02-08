
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    // [RePackerWrapper(typeof(int))]
    public class StackWrapper<TElement> : RePackerWrapper<Stack<TElement>>
    {
        public override void Pack(BoxedBuffer buffer, ref Stack<TElement> value)
        {
            buffer.PackIEnumerable(value);
        }

        public override void Unpack(BoxedBuffer buffer, out Stack<TElement> value)
        {
            buffer.UnpackIEnumerable<TElement>(IEnumerableType.Stack, out var ien);
            value = (Stack<TElement>)ien;
        }
    }
}
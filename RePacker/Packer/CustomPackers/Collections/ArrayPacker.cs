
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    // [RePackerWrapper(typeof(int))]
    public class ArrayWrapper : RePackerWrapper<Array>
    {
        Dictionary<Type, Action<BoxedBuffer, Array>> packLookup = new Dictionary<Type, Action<BoxedBuffer, Array>>();
        Dictionary<Type, Func<BoxedBuffer, Array>> unpackLookup = new Dictionary<Type, Func<BoxedBuffer, Array>>();

        public override void Pack(BoxedBuffer buffer, ref Array value)
        {
            var elementType = value.GetValue(0).GetType();

            if (!packLookup.ContainsKey(elementType))
            {
                MakePacker(elementType);
            }

            packLookup[elementType](buffer, value);
        }

        void MakePacker(Type elementType)
        {
            var param1 = Expression.Parameter(typeof(BoxedBuffer));
            var param2 = Expression.Parameter(typeof(Array));

            var body = Expression.Call(
                typeof(PackerExtensions),
                nameof(PackerExtensions.PackArray),
                new[] { elementType },
                param1,
                Expression.Convert(param2, elementType.MakeArrayType())
            );

            var lambda = Expression.Lambda<Action<BoxedBuffer, Array>>(body, new[] { param1, param2 }).Compile();

            packLookup.Add(elementType, lambda);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Array value)
        {
            var elementType = value.GetType().GetElementType();

            if (!unpackLookup.ContainsKey(elementType))
            {
                MakeUnpacker(value.GetType(), elementType);
            }

            value = unpackLookup[elementType](buffer);
        }

        void MakeUnpacker(Type arrayType, Type elementType)
        {
            var param1 = Expression.Parameter(typeof(BoxedBuffer));

            var body = Expression.Call(
                typeof(PackerExtensions),
                nameof(PackerExtensions.UnpackArrayAsRet),
                new[] { elementType },
                param1
            );

            var act = Expression.GetFuncType(typeof(BoxedBuffer), arrayType.GetType());
            var lambda = (Func<BoxedBuffer, Array>)Expression.Lambda<Func<BoxedBuffer, Array>>(body, param1).Compile();

            unpackLookup.Add(elementType, lambda);
        }

        public override void Unpack(BoxedBuffer buffer, out Array value)
        {
            throw new NotImplementedException();
        }
    }
}
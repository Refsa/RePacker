
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Refsa.RePacker.Builder
{
    public class ArrayProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Array);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetElementType();
            var instance = Activator.CreateInstance(typeof(ArrayWrapper<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }
}
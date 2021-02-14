using System;

namespace Refsa.RePacker.Builder
{
    internal class ArrayProducer : GenericProducer
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
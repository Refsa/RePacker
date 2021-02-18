using System;
using Refsa.RePacker.Utils;

namespace Refsa.RePacker.Builder
{
    internal class ArrayProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Array);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetElementType();

            ITypePacker instance = null;

            if (TypeCache.TryGetTypeInfo(elementType, out var _))
            {
                instance = (ITypePacker)Activator
                    .CreateInstance(typeof(ArrayPacker<>)
                    .MakeGenericType(elementType));
            }
            else if (elementType.IsValueType || elementType.IsUnmanagedStruct())
            {
                instance = (ITypePacker)Activator
                    .CreateInstance(typeof(ArrayUnmanagedPacker<>)
                    .MakeGenericType(elementType));
            }

            return instance;
        }
    }
}
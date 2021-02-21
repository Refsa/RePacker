using System;
using RePacker.Utils;

namespace RePacker.Builder
{
    internal class ArrayProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Array);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetElementType();

            ITypePacker instance = null;

            if (TypeCache.TryGetTypeInfo(elementType, out var ti) && !ti.IsDirectlyCopyable)
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
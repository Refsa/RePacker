using System;
using System.Collections.Generic;

namespace Refsa.RePacker.Builder
{
    internal class NullableProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Nullable<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementTypes = type.GetGenericArguments();
            var instance = Activator.CreateInstance(typeof(NullablePacker<>).MakeGenericType(elementTypes));
            return (ITypePacker)instance;
        }
    }
}
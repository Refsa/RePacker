using System;
using System.Collections.Generic;

namespace Refsa.RePacker.Builder
{
    internal class KeyValuePairProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(KeyValuePair<,>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementTypes = type.GetGenericArguments();
            var instance = Activator.CreateInstance(typeof(KeyValuePairWrapper<,>).MakeGenericType(elementTypes));
            return (ITypePacker)instance;
        }
    }
}
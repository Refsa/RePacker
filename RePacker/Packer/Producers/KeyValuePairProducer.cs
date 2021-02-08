
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Refsa.RePacker.Builder
{
    public class KeyValuePairProducer : GenericProducer
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
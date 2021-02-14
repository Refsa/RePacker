using System;
using System.Collections.Generic;

namespace Refsa.RePacker.Builder
{
    internal class DictionaryProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Dictionary<,>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementTypes = type.GetGenericArguments();
            var instance = Activator.CreateInstance(typeof(DictionaryWrapper<,>).MakeGenericType(elementTypes));
            return (ITypePacker)instance;
        }
    }
}
using System;
using System.Collections.Generic;

namespace RePacker.Builder
{
    internal class DictionaryProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Dictionary<,>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementTypes = type.GetGenericArguments();
            var instance = Activator.CreateInstance(typeof(DictionaryPacker<,>).MakeGenericType(elementTypes));
            return (ITypePacker)instance;
        }
    }
}
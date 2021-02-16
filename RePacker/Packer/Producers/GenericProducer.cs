using System;
using System.Collections.Generic;

namespace Refsa.RePacker.Builder
{
    public abstract class GenericProducer
    {
        public virtual IEnumerable<Type> ProducerForAll { get; }
        public abstract Type ProducerFor { get; }
        public abstract ITypePacker GetProducer(Type type);
    }
}
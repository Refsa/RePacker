using System;

namespace Refsa.RePacker.Builder
{
    public abstract class GenericProducer
    {
        public abstract Type ProducerFor { get; }
        public abstract ITypePacker GetProducer(Type type);
    }
}
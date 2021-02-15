using System;
using System.Collections.Generic;

namespace Refsa.RePacker.Builder
{
    internal class ListProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(List<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(ListPacker<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    internal class StackProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Stack<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(StackPacker<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    internal class QueueProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Queue<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(QueuePacker<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    internal class HashSetProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(HashSet<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(HashSetPacker<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }
}
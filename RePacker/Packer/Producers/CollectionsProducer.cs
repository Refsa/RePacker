
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Refsa.RePacker.Builder
{
    public class ListProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(List<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(ListWrapper<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    public class StackProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Stack<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(StackWrapper<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    public class QueueProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Queue<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(QueueWrapper<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    public class HashSetProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(HashSet<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(HashSetWrapper<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }
}
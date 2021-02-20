using System;
using System.Collections.Generic;
using RePacker.Utils;

namespace RePacker.Builder
{
    internal class IListProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(IList<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];

            if (TypeCache.TryGetTypeInfo(elementType, out var _))
            {
                return (ITypePacker)Activator.CreateInstance(typeof(IListPacker<>).MakeGenericType(elementType));
            }
            else if (elementType.IsValueType || elementType.IsUnmanagedStruct())
            {
                return (ITypePacker)Activator.CreateInstance(typeof(IListUnmanagedPacker<>).MakeGenericType(elementType));
            }

            return null;
        }
    }

    internal class ListProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(List<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments();
            var instance = Activator.CreateInstance(typeof(ListPacker<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    internal class StackProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Stack<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments();
            var instance = Activator.CreateInstance(typeof(StackPacker<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    internal class QueueProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Queue<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments();
            var instance = Activator.CreateInstance(typeof(QueuePacker<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    internal class HashSetProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(HashSet<>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments();
            var instance = Activator.CreateInstance(typeof(HashSetPacker<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }
}
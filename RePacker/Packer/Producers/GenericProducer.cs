
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Refsa.RePacker
{
    public abstract class GenericProducer
    {
        public abstract Type ProducerFor { get; }
        public abstract ITypePacker GetProducer(Type type);
    }

    public class ArrayProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Array);

        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetElementType();
            var instance = Activator.CreateInstance(typeof(ArrayWrapper<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

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

    public class DictionaryProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Dictionary<,>);

        public override ITypePacker GetProducer(Type type)
        {
            var elementTypes = type.GetGenericArguments();
            var instance = Activator.CreateInstance(typeof(DictionaryWrapper<,>).MakeGenericType(elementTypes));
            return (ITypePacker)instance;
        }
    }

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

    public class ValueTupleProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(ITuple);

        public override ITypePacker GetProducer(Type type)
        {
            var elementTypes = type.GetGenericArguments();

            Type genType =
                elementTypes.Length switch
                {
                    2 => typeof(ValueTupleWrapper<,>),
                    3 => typeof(ValueTupleWrapper<,,>),
                    4 => typeof(ValueTupleWrapper<,,,>),
                    5 => typeof(ValueTupleWrapper<,,,,>),
                    6 => typeof(ValueTupleWrapper<,,,,,>),
                    7 => typeof(ValueTupleWrapper<,,,,,,>),
                    8 => typeof(ValueTupleWrapper<,,,,,,,>),
                    _ => null
                };

            if (genType == null)
            {
                throw new ArgumentException();
            }

            var instance = Activator.CreateInstance(genType.MakeGenericType(elementTypes));
            return (ITypePacker)instance;
        }
    }
}
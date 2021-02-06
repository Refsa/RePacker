
using System;
using System.Collections.Generic;

namespace Refsa.RePacker
{
    public abstract class GenericProducer
    {
        public abstract ITypePacker GetProducer(Type type);
    }

    public class ArrayProducer : GenericProducer
    {
        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetElementType();
            var instance = Activator.CreateInstance(typeof(ArrayWrapper<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    public class ListProducer : GenericProducer
    {
        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(ListWrapper<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    public class StackProducer : GenericProducer
    {
        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(StackWrapper<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    public class QueueProducer : GenericProducer
    {
        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(QueueWrapper<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    public class HashSetProducer : GenericProducer
    {
        public override ITypePacker GetProducer(Type type)
        {
            var elementType = type.GetGenericArguments()[0];
            var instance = Activator.CreateInstance(typeof(HashSetWrapper<>).MakeGenericType(elementType));
            return (ITypePacker)instance;
        }
    }

    public class DictionaryProducer : GenericProducer
    {
        public override ITypePacker GetProducer(Type type)
        {
            var elementTypes = type.GetGenericArguments();
            var instance = Activator.CreateInstance(typeof(DictionaryWrapper<,>).MakeGenericType(elementTypes));
            return (ITypePacker)instance;
        }
    }

    public class KeyValuePairProducer : GenericProducer
    {
        public override ITypePacker GetProducer(Type type)
        {
            var elementTypes = type.GetGenericArguments();
            var instance = Activator.CreateInstance(typeof(KeyValuePairWrapper<,>).MakeGenericType(elementTypes));
            return (ITypePacker)instance;
        }
    }

    public class ValueTupleProducer : GenericProducer
    {
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
                    _ => null,
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
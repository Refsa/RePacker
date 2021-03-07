using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RePacker.Builder
{
    internal class ValueTupleProducer : GenericProducer
    {
#if NET461
        public override Type ProducerFor => typeof(ValueTuple<,>);
        public override IEnumerable<Type> ProducerForAll => new List<Type>()
        {
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>),
        };
#else
        public override Type ProducerFor => typeof(ITuple);
#endif

        public override ITypePacker GetProducer(Type type)
        {
            var elementTypes = type.GetGenericArguments();

            Type genType = null;

            switch (elementTypes.Length)
            {
                case 1:
                    genType = typeof(ValueTuplePacker<>);
                    break;
                case 2:
                    genType = typeof(ValueTuplePacker<,>);
                    break;
                case 3:
                    genType = typeof(ValueTuplePacker<,,>);
                    break;
                case 4:
                    genType = typeof(ValueTuplePacker<,,,>);
                    break;
                case 5:
                    genType = typeof(ValueTuplePacker<,,,,>);
                    break;
                case 6:
                    genType = typeof(ValueTuplePacker<,,,,,>);
                    break;
                case 7:
                    genType = typeof(ValueTuplePacker<,,,,,,>);
                    break;
                case 8:
                    genType = typeof(ValueTuplePacker<,,,,,,,>);
                    break;
            }

            if (genType == null)
            {
                throw new ArgumentException($"ValueTuple of length {elementTypes.Length} is not supported");
            }

            var instance = Activator.CreateInstance(genType.MakeGenericType(elementTypes));
            return (ITypePacker)instance;
        }
    }
}
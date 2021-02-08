
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Refsa.RePacker.Builder
{
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
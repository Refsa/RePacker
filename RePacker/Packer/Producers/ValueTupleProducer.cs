
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

            Type genType = null;

            switch (elementTypes.Length)
            {
                case 2:
                    genType = typeof(ValueTupleWrapper<,>);
                    break;
                case 3:
                    genType = typeof(ValueTupleWrapper<,,>);
                    break;
                case 4:
                    genType = typeof(ValueTupleWrapper<,,,>);
                    break;
                case 5:
                    genType = typeof(ValueTupleWrapper<,,,,>);
                    break;
                case 6:
                    genType = typeof(ValueTupleWrapper<,,,,,>);
                    break;
                case 7:
                    genType = typeof(ValueTupleWrapper<,,,,,,>);
                    break;
                case 8:
                    genType = typeof(ValueTupleWrapper<,,,,,,,>);
                    break;
            }

            if (genType == null)
            {
                throw new ArgumentException();
            }

            var instance = Activator.CreateInstance(genType.MakeGenericType(elementTypes));
            return (ITypePacker)instance;
        }
    }
}
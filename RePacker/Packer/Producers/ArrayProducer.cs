using System;
using System.Collections.Generic;
using RePacker.Utils;

namespace RePacker.Builder
{
    internal class ArrayProducer : GenericProducer
    {
        public override Type ProducerFor => typeof(Array);

        public override ITypePacker GetProducer(Type type)
        {
            Type elementType = type.GetElementType();

            switch (type.GetArrayRank())
            {
                case 1:
                    return SingleDimensionArray(elementType);
                case 2:
                    return TwoDimensionalArray(elementType);
                case 3:
                    return ThreeDimensionalArray(elementType);
                case 4:
                    return FourDimensionalArray(elementType);
                default:
                    RePacking.Logger.Warn($"Array of rank {type.GetArrayRank()} is not supported");
                    break;
            }

            return null;
        }

        private ITypePacker FourDimensionalArray(Type elementType)
        {
            if (TypeCache.TryGetTypeInfo(elementType, out var ti))
            {
                return (ITypePacker)Activator
                    .CreateInstance(typeof(Array4DPacker<>).MakeGenericType(elementType));
            }

            return null;
        }

        private ITypePacker ThreeDimensionalArray(Type elementType)
        {
            if (TypeCache.TryGetTypeInfo(elementType, out var ti))
            {
                return (ITypePacker)Activator
                    .CreateInstance(typeof(Array3DPacker<>).MakeGenericType(elementType));
            }

            return null;
        }

        private ITypePacker TwoDimensionalArray(Type elementType)
        {
            if (TypeCache.TryGetTypeInfo(elementType, out var ti))
            {
                return (ITypePacker)Activator
                    .CreateInstance(typeof(Array2DPacker<>).MakeGenericType(elementType));
            }

            return null;
        }

        ITypePacker SingleDimensionArray(Type elementType)
        {
            if (TypeCache.TryGetTypeInfo(elementType, out var ti) && !ti.IsDirectlyCopyable)
            {
                return (ITypePacker)Activator
                    .CreateInstance(typeof(ArrayPacker<>).MakeGenericType(elementType));
            }
            else if (elementType.IsValueType || elementType.IsUnmanagedStruct())
            {
                return (ITypePacker)Activator
                    .CreateInstance(typeof(ArrayUnmanagedPacker<>).MakeGenericType(elementType));
            }

            return null;
        }
    }
}
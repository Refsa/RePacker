

using System;
using System.Collections.Generic;

namespace Refsa.RePacker.Generator
{
    public static class GeneratorLookup
    {
        static Dictionary<GeneratorType, Dictionary<Type, IGenerator>> generators =
            new Dictionary<GeneratorType, Dictionary<Type, IGenerator>>
            {
                {GeneratorType.None, new Dictionary<Type, IGenerator>{}},
                {GeneratorType.Unmanaged, new Dictionary<Type, IGenerator>{
                    {Type.Missing.GetType(), new UnmanagedGenerator()}
                }},
                {GeneratorType.Struct, new Dictionary<Type, IGenerator>{
                    {Type.Missing.GetType(), new UnmanagedStructGenerator()}
                }},
                {GeneratorType.String, new Dictionary<Type, IGenerator>{
                    {typeof(string), new StringGenerator()}
                }},
                {GeneratorType.Object, new Dictionary<Type, IGenerator>() {
                    {typeof(Array), new ArrayGenerator()},
                    {typeof(IList<>), new IListGenerator()},
                }},
                {GeneratorType.RePacker, new Dictionary<Type, IGenerator>() {
                    {Type.Missing.GetType(), new RePackerGenerator()}
                }}
            };

        public static IGenerator Get(GeneratorType generatorType, Type targetType = null)
        {
            if (targetType == null)
            {
                targetType = Type.Missing.GetType();
            }

            if (generators.TryGetValue(generatorType, out var generator))
            {
                if (generator.TryGetValue(targetType, out var gen))
                {
                    return gen;
                }
            }

            return null;
        }
    }
}
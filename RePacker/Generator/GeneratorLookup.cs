

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Refsa.RePacker.Builder
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
                    {Type.Missing.GetType(), new StringGenerator()},
                    {typeof(string), new StringGenerator()}
                }},
                {GeneratorType.Object, new Dictionary<Type, IGenerator>() {
                    {typeof(Array), new ArrayGenerator()},
                    {typeof(IList<>), new IListGenerator()},
                    {typeof(IEnumerable<>), new IEnumerableGenerator()},
                    {typeof(Dictionary<,>), new DictionaryGenerator()},
                    {typeof(List<>), new IListGenerator()},
                    {typeof(HashSet<>), new IEnumerableGenerator()},
                    {typeof(Queue<>), new IEnumerableGenerator()},
                    {typeof(Stack<>), new IEnumerableGenerator()},

                    {typeof(KeyValuePair<,>), new KeyValuePairGenerator()},

                    {typeof(ITuple), new ValueTupleGenerator()},
                }},
                {GeneratorType.RePacker, new Dictionary<Type, IGenerator>() {
                    {Type.Missing.GetType(), new RePackerGenerator()}
                }},
                {GeneratorType.DateTime, new Dictionary<Type, IGenerator>() {
                    {Type.Missing.GetType(), new DateTimeGenerator()}
                }}
            };

        public static bool TryGet(GeneratorType generatorType, Type targetType, out IGenerator generator)
        {
            if (targetType == null)
            {
                targetType = Type.Missing.GetType();
            }

            if (generators.TryGetValue(generatorType, out var _generator))
            {
                if (_generator.TryGetValue(targetType, out var dirGen))
                {
                    generator = dirGen;
                    return true;
                }
                else
                {
                    foreach (var intf in targetType.GetInterfaces())
                    {
                        if (_generator.TryGetValue(intf, out var intfGen))
                        {
                            generator = intfGen;
                            return true;
                        }
                    }
                }
            }

            generator = null;
            return false;
        }

        public static void RegisterGenerator(Type type, IGenerator generator)
        {
            if (generators.TryGetValue(GeneratorType.Object, out var gens))
            {
                if (!gens.ContainsKey(type))
                {
                    gens.Add(type, generator);
                }
                else
                {
                    throw new ArgumentException($"Generator for type {type} already exists");
                }
            }
        }
    }
}
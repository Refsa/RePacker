using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RePacker.Builder
{
    internal static class GeneratorLookup
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
                    {typeof(HashSet<>), new HashSetGenerator()},
                    {typeof(Queue<>), new QueueGenerator()},
                    {typeof(Stack<>), new StackGenerator()},

                    {typeof(KeyValuePair<,>), new KeyValuePairGenerator()},

                    {typeof(Nullable<>), new NullableGenerator()},

#if NET461
                    {typeof(ValueTuple<,>), new ValueTupleGenerator()},
                    {typeof(ValueTuple<,,>), new ValueTupleGenerator()},
                    {typeof(ValueTuple<,,,>), new ValueTupleGenerator()},
                    {typeof(ValueTuple<,,,,>), new ValueTupleGenerator()},
                    {typeof(ValueTuple<,,,,,>), new ValueTupleGenerator()},
                    {typeof(ValueTuple<,,,,,,>), new ValueTupleGenerator()},
                    {typeof(ValueTuple<,,,,,,,>), new ValueTupleGenerator()},
#else
                    {typeof(ITuple), new ValueTupleGenerator()},    
#endif
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

            if (generators == null)
            {
                generator = null;
                return false;
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
    }
}
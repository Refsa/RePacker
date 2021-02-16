using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Refsa.RePacker.Builder
{
    internal class TypeResolver
    {
        public Func<Type, int> Resolver;
    }

    internal static class TypeResolverBuilder
    {
        public static TypeResolver BuildHandler(Dictionary<Type, TypePackerHandler> handlers)
        {
            Type[] typeParams = new Type[] { typeof(Type) };

            var solverBuilder = new DynamicMethod(
                $"TypeResolver_Method",
                typeof(int),
                typeParams,
                typeof(TypeResolverBuilder),
                true
            );

            MethodInfo getHashCode = typeof(Type).GetMethod(nameof(Type.GetHashCode));

            var ilGen = solverBuilder.GetILGenerator();
            {
                var wasEqualLabel = ilGen.DefineLabel();

                int index = 0;
                foreach (var handler in handlers)
                {
                    ilGen.Emit(OpCodes.Ldc_I4, index);

                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Callvirt, getHashCode);

                    ilGen.Emit(OpCodes.Ldc_I4, handler.Key.GetHashCode());
                    ilGen.Emit(OpCodes.Beq, wasEqualLabel);

                    ilGen.Emit(OpCodes.Pop);

                    index++;
                }

                ilGen.Emit(OpCodes.Ldc_I4, -1);
                ilGen.MarkLabel(wasEqualLabel);
                ilGen.Emit(OpCodes.Ret);
            }

            var res = (Func<Type, int>)solverBuilder.CreateDelegate(typeof(Func<Type, int>));

            var resolver = new TypeResolver { Resolver = res };
            return resolver;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Refsa.RePacker.Builder
{
    public class TypeResolver
    {
        public Func<Type, int> Resolver;
    }

    public static class TypeResolverBuilder
    {
        static AssemblyBuilder asmBuilder;
        static ModuleBuilder moduleBuilder;
        static bool isSetup = false;

        public static void Begin()
        {
            if (isSetup) return;

            asmBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.Run);

            moduleBuilder = asmBuilder
                .DefineDynamicModule($"TypeResolvers");

            isSetup = true;
        }

        public static void Complete()
        {
            moduleBuilder.CreateGlobalFunctions();
        }

        public static TypeResolver BuildHandler(Dictionary<Type, TypePackerHandler> handlers)
        {
            Type[] typeParams = new Type[] { typeof(Type) };

            var deserBuilder = moduleBuilder.DefineGlobalMethod(
                $"TypeResolver_Method",
                MethodAttributes.Public | MethodAttributes.Static |
                MethodAttributes.HideBySig,
                typeof(int),
                typeParams
            );
            deserBuilder.SetImplementationFlags(MethodImplAttributes.Managed);

            var paramBuilder = deserBuilder.DefineParameter(0, ParameterAttributes.None, "type");

            var ilGen = deserBuilder.GetILGenerator();
            {
                var wasEqualLabel = ilGen.DefineLabel();

                int index = 0;
                foreach ((Type key, TypePackerHandler handler) in handlers)
                {
                    ilGen.Emit(OpCodes.Ldc_I4, index);

                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Ldtoken, key);
                    ilGen.Emit(OpCodes.Beq, wasEqualLabel);

                    ilGen.Emit(OpCodes.Pop);

                    index++;
                }

                ilGen.Emit(OpCodes.Ldc_I4, -1);
                ilGen.MarkLabel(wasEqualLabel);
                ilGen.Emit(OpCodes.Ret);
            }

            Complete();

            var res = (Func<Type, int>)moduleBuilder.GetMethod(deserBuilder.Name).CreateDelegate(typeof(Func<Type, int>));

            var resolver = new TypeResolver { Resolver = res };
            return resolver;
        }
    }
}

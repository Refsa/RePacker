using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using RePacker.Utils;

namespace RePacker.Builder
{
    internal class ValueTupleGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;

#if NET461
        public Type ForType => typeof(ValueTuple<>);
#else
        public Type ForType => typeof(ITuple);
#endif

        static readonly MethodInfo sizeOfMethod = typeof(RePacking).GetMethod(nameof(RePacking.SizeOf));

        static readonly Dictionary<Type, MethodInfo[]> itemGetterLookup = new Dictionary<Type, MethodInfo[]>()
        {
            {typeof(ValueTuple<>), new MethodInfo[] {GetItemMethod("Item1")}},
            {typeof(ValueTuple<,>), new MethodInfo[] {GetItemMethod("Item1"), GetItemMethod("Item2")}},
            {typeof(ValueTuple<,,>), new MethodInfo[] {GetItemMethod("Item1"), GetItemMethod("Item2"), GetItemMethod("Item3")}},
            {typeof(ValueTuple<,,,>), new MethodInfo[] {GetItemMethod("Item1"), GetItemMethod("Item2"), GetItemMethod("Item3"), GetItemMethod("Item4")}},
            {typeof(ValueTuple<,,,,>), new MethodInfo[] {GetItemMethod("Item1"), GetItemMethod("Item2"), GetItemMethod("Item3"), GetItemMethod("Item4"), GetItemMethod("Item5")}},
            {typeof(ValueTuple<,,,,,>), new MethodInfo[] {GetItemMethod("Item1"), GetItemMethod("Item2"), GetItemMethod("Item3"), GetItemMethod("Item4"), GetItemMethod("Item5"), GetItemMethod("Item6")}},
            {typeof(ValueTuple<,,,,,,>), new MethodInfo[] {GetItemMethod("Item1"), GetItemMethod("Item2"), GetItemMethod("Item3"), GetItemMethod("Item4"), GetItemMethod("Item5"), GetItemMethod("Item6"), GetItemMethod("Item7")}},
            {typeof(ValueTuple<,,,,,,,>), new MethodInfo[] {GetItemMethod("Item1"), GetItemMethod("Item2"), GetItemMethod("Item3"), GetItemMethod("Item4"), GetItemMethod("Item5"), GetItemMethod("Item6"), GetItemMethod("Item7"), GetItemMethod("Rest")}},
        };

        static MethodInfo GetItemMethod(string propName)
        {
            return typeof(ValueTuple<>).GetProperty(propName).GetMethod;
        }

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsUnpack(fieldInfo);

            var genArgs = fieldInfo.FieldType.GetGenericArguments();

            var meth = typeof(PackerValueTupleExt)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(e =>
                    e.Name.Contains(nameof(PackerValueTupleExt.UnpackValueTuple)) &&
                    e.GetGenericArguments().Length == genArgs.Length
                ).First()
                .MakeGenericMethod(genArgs);

            ilGen.Emit(OpCodes.Call, meth);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsPack(fieldInfo);

            var genArgs = fieldInfo.FieldType.GetGenericArguments();

            var meth = typeof(PackerValueTupleExt)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(e =>
                    e.Name.Contains(nameof(PackerValueTupleExt.PackValueTuple)) &&
                    e.GetGenericArguments().Length == genArgs.Length
                ).First()
                .MakeGenericMethod(genArgs);

            ilGen.Emit(OpCodes.Call, meth);
        }

        public void GenerateGetSizer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Ldc_I4, 0);

            if (itemGetterLookup.TryGetValue(fieldInfo.FieldType.GetGenericTypeDefinition(), out var getters))
            {
                foreach (var getter in getters)
                {
                    if (fieldInfo.FieldType.IsValueType)
                    {
                        ilGen.Emit(OpCodes.Ldarga_S, 0);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Ldarg_S, 0);
                    }
                    ilGen.Emit(OpCodes.Ldfld, fieldInfo);

                    ilGen.Emit(OpCodes.Call, getter);
                    ilGen.Emit(OpCodes.Add);
                }
            }
        }
    }
}
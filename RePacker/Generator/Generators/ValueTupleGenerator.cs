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

        static readonly MethodInfo sizeOfMethod;
        static readonly Dictionary<Type, FieldInfo[]> itemGetterLookup;

        static ValueTupleGenerator()
        {
            sizeOfMethod = typeof(RePacking).GetMethod(nameof(RePacking.SizeOf));
            itemGetterLookup = new Dictionary<Type, FieldInfo[]>()
                {
                    {typeof(ValueTuple<>), new FieldInfo[] {GetItemField(typeof(ValueTuple<>), "Item1")}},
                    {typeof(ValueTuple<,>), new FieldInfo[] {GetItemField(typeof(ValueTuple<,>), "Item1"), GetItemField(typeof(ValueTuple<,>), "Item2")}},
                    {typeof(ValueTuple<,,>), new FieldInfo[] {GetItemField(typeof(ValueTuple<,,>), "Item1"), GetItemField(typeof(ValueTuple<,,>), "Item2"), GetItemField(typeof(ValueTuple<,,>), "Item3")}},
                    {typeof(ValueTuple<,,,>), new FieldInfo[] {GetItemField(typeof(ValueTuple<,,,>), "Item1"), GetItemField(typeof(ValueTuple<,,,>), "Item2"), GetItemField(typeof(ValueTuple<,,,>), "Item3"), GetItemField(typeof(ValueTuple<,,,>), "Item4")}},
                    {typeof(ValueTuple<,,,,>), new FieldInfo[] {GetItemField(typeof(ValueTuple<,,,,>), "Item1"), GetItemField(typeof(ValueTuple<,,,,>), "Item2"), GetItemField(typeof(ValueTuple<,,,,>), "Item3"), GetItemField(typeof(ValueTuple<,,,,>), "Item4"), GetItemField(typeof(ValueTuple<,,,,>), "Item5")}},
                    {typeof(ValueTuple<,,,,,>), new FieldInfo[] {GetItemField(typeof(ValueTuple<,,,,,>), "Item1"), GetItemField(typeof(ValueTuple<,,,,,>), "Item2"), GetItemField(typeof(ValueTuple<,,,,,>), "Item3"), GetItemField(typeof(ValueTuple<,,,,,>), "Item4"), GetItemField(typeof(ValueTuple<,,,,,>), "Item5"), GetItemField(typeof(ValueTuple<,,,,,>), "Item6")}},
                    {typeof(ValueTuple<,,,,,,>), new FieldInfo[] {GetItemField(typeof(ValueTuple<,,,,,,>), "Item1"), GetItemField(typeof(ValueTuple<,,,,,,>), "Item2"), GetItemField(typeof(ValueTuple<,,,,,,>), "Item3"), GetItemField(typeof(ValueTuple<,,,,,,>), "Item4"), GetItemField(typeof(ValueTuple<,,,,,,>), "Item5"), GetItemField(typeof(ValueTuple<,,,,,,>), "Item6"), GetItemField(typeof(ValueTuple<,,,,,,>), "Item7")}},
                    {typeof(ValueTuple<,,,,,,,>), new FieldInfo[] {GetItemField(typeof(ValueTuple<,,,,,,,>), "Item1"), GetItemField(typeof(ValueTuple<,,,,,,,>), "Item2"), GetItemField(typeof(ValueTuple<,,,,,,,>), "Item3"), GetItemField(typeof(ValueTuple<,,,,,,,>), "Item4"), GetItemField(typeof(ValueTuple<,,,,,,,>), "Item5"), GetItemField(typeof(ValueTuple<,,,,,,,>), "Item6"), GetItemField(typeof(ValueTuple<,,,,,,,>), "Item7"), GetItemField(typeof(ValueTuple<,,,,,,,>), "Rest")}},
                };
        }

        static FieldInfo GetItemField(Type valueTupleType, string propName)
        {
            return valueTupleType.GetField(propName);
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

            if (itemGetterLookup.TryGetValue(fieldInfo.FieldType.GetGenericTypeDefinition(), out var itemFields))
            {
                foreach (var itemField in itemFields)
                {
                    var genSizeOf = sizeOfMethod.MakeGenericMethod(itemField.FieldType);

                    if (fieldInfo.FieldType.IsValueType)
                    {
                        ilGen.Emit(OpCodes.Ldarga_S, 0);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Ldarg_S, 0);
                    }
                    ilGen.Emit(OpCodes.Ldfld, fieldInfo);
                    ilGen.Emit(OpCodes.Ldfld, itemField);

                    ilGen.Emit(OpCodes.Call, genSizeOf);

                    ilGen.Emit(OpCodes.Add);
                }
            }
        }
    }
}
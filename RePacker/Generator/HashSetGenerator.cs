using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Buffers;
using RePacker.Utils;
using static RePacker.Builder.PackerCollectionsExt;

namespace RePacker.Builder
{
    internal class HashSetGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(HashSet<>);

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo deserializeHashSetMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackHashSet));
        // MethodInfo deserializeHashSetBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackHashSetBlittable));

        MethodInfo serializeHashSetMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackHashSet));
        // MethodInfo serializeHashSetBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackHashSetBlittable));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            Type elementType = fieldInfo.FieldType.GenericTypeArguments[0];

            ilGen.Emit(OpCodes.Ldarg_0);

            ilGen.Emit(OpCodes.Ldloca_S, 0);
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);

            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                var enumerableDeserializer = deserializeHashSetMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, enumerableDeserializer);
            }
            // else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            // {
            //     var enumerableDeserializer = deserializeHashSetBlittableMethod.MakeGenericMethod(elementType);
            //     ilGen.Emit(OpCodes.Call, enumerableDeserializer);
            // }
            else
            {
                ilGen.Emit(OpCodes.Pop);
                ilGen.Emit(OpCodes.Pop);
                ilGen.EmitLog($"RePacker - Unpack: Array of type {fieldInfo.FieldType.Name} is not supported");
                return;
            }
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            ilGen.Emit(OpCodes.Ldarg_0);

            ilGen.Emit(OpCodes.Ldarga_S, 1);
            ilGen.Emit(OpCodes.Ldfld, fieldInfo);

            Type elementType = fieldInfo.FieldType.GenericTypeArguments[0];
            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                var enumerableSerializer = serializeHashSetMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, enumerableSerializer);
            }
            // else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            // {
            //     var enumerableSerializer = serializeHashSetBlittableMethod.MakeGenericMethod(elementType);
            //     ilGen.Emit(OpCodes.Call, enumerableSerializer);
            // }
            else
            {
                ilGen.Emit(OpCodes.Pop);
                ilGen.Emit(OpCodes.Pop);
                ilGen.EmitLog($"RePacker - Pack: HashSet of type {fieldInfo.FieldType.Name} is not supported");
            }
        }
    }
}
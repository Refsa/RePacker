using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Buffers;
using RePacker.Utils;
using static RePacker.Builder.PackerCollectionsExt;

namespace RePacker.Builder
{
    internal class QueueGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(Queue<>);

        MethodInfo deserializeQueueMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackQueue));

        MethodInfo serializeQueueMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackQueue));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            Type elementType = fieldInfo.FieldType.GenericTypeArguments[0];

            ilGen.Emit(OpCodes.Ldarg_0);

            ilGen.Emit(OpCodes.Ldloca_S, 0);
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);

            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                var enumerableDeserializer = deserializeQueueMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, enumerableDeserializer);
            }
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
            ilGen.Emit(OpCodes.Ldarg_0);

            ilGen.Emit(OpCodes.Ldarga_S, 1);
            ilGen.Emit(OpCodes.Ldfld, fieldInfo);

            Type elementType = fieldInfo.FieldType.GenericTypeArguments[0];
            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                var enumerableSerializer = serializeQueueMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, enumerableSerializer);
            }
            else
            {
                ilGen.Emit(OpCodes.Pop);
                ilGen.Emit(OpCodes.Pop);
                ilGen.EmitLog($"RePacker - Pack: Queue of type {fieldInfo.FieldType.Name} is not supported");
            }
        }

        public void GenerateGetSizer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var elementType = fieldInfo.FieldType.GetElementType();

            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo) && !typeInfo.IsDirectlyCopyable)
            {
                var sizeMethod = typeof(PackerCollectionsExt)
                    .GetMethod(nameof(PackerCollectionsExt.SizeOfCollection))
                    .MakeGenericMethod(elementType);

                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Call, fieldInfo.FieldType.GetMethod("GetEnumerator"));
                ilGen.Emit(OpCodes.Call, sizeMethod);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldc_I4, 0);
            }
        }
    }
}
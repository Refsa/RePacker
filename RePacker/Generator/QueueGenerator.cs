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

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo deserializeQueueMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackQueue));
        // MethodInfo deserializeQueueBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackQueueBlittable));

        MethodInfo serializeQueueMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackQueue));
        // MethodInfo serializeQueueBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackQueueBlittable));

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
            // else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            // {
            //     var enumerableDeserializer = deserializeQueueBlittableMethod.MakeGenericMethod(elementType);
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
            ilGen.Emit(OpCodes.Ldarg_0);

            ilGen.Emit(OpCodes.Ldarga_S, 1);
            ilGen.Emit(OpCodes.Ldfld, fieldInfo);

            Type elementType = fieldInfo.FieldType.GenericTypeArguments[0];
            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                var enumerableSerializer = serializeQueueMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, enumerableSerializer);
            }
            // else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            // {
            //     var enumerableSerializer = serializeQueueBlittableMethod.MakeGenericMethod(elementType);
            //     ilGen.Emit(OpCodes.Call, enumerableSerializer);
            // }
            else
            {
                ilGen.Emit(OpCodes.Pop);
                ilGen.Emit(OpCodes.Pop);
                ilGen.EmitLog($"RePacker - Pack: Queue of type {fieldInfo.FieldType.Name} is not supported");
            }
        }
    }
}
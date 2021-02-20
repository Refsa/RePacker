using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Buffers;
using RePacker.Utils;

namespace RePacker.Builder
{
    internal class IListGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(IList<>);

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo deserializeIListMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackIList));
        MethodInfo deserializeIListBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackIListBlittable));

        MethodInfo serializeIListMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackIList));
        MethodInfo serializeIListBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackIListBlittable));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            ilGen.Emit(OpCodes.Ldarg_0);

            ilGen.Emit(OpCodes.Ldloca_S, 0);
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);

            Type elementType = fieldInfo.FieldType.GenericTypeArguments[0];
            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                var listSerializer = deserializeIListMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, listSerializer);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                var arraySerializer = deserializeIListBlittableMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, arraySerializer);
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
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            ilGen.Emit(OpCodes.Ldarg_0);

            ilGen.Emit(OpCodes.Ldarga_S, 1);
            ilGen.Emit(OpCodes.Ldfld, fieldInfo);

            Type elementType = fieldInfo.FieldType.GenericTypeArguments[0];
            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                var listSerializer = serializeIListMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, listSerializer);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                var arraySerializer = serializeIListBlittableMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, arraySerializer);
            }
            else
            {
                ilGen.Emit(OpCodes.Pop);
                ilGen.Emit(OpCodes.Pop);
                ilGen.EmitLog($"RePacker - Pack: IList of type {fieldInfo.FieldType.Name} is not supported");
            }
        }
    }
}
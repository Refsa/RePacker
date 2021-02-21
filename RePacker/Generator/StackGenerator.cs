using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Buffers;
using RePacker.Utils;
using static RePacker.Builder.PackerCollectionsExt;

namespace RePacker.Builder
{
    internal class StackGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(Stack<>);

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo deserializeStackMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackStack));
        // MethodInfo deserializeStackBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackStackBlittable));

        MethodInfo serializeStackMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackStack));
        // MethodInfo serializeStackBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackStackBlittable));

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
                var enumerableDeserializer = deserializeStackMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, enumerableDeserializer);
            }
            // else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            // {
            //     var enumerableDeserializer = deserializeStackBlittableMethod.MakeGenericMethod(elementType);
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
                var enumerableSerializer = serializeStackMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, enumerableSerializer);
            }
            // else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            // {
            //     var enumerableSerializer = serializeStackBlittableMethod.MakeGenericMethod(elementType);
            //     ilGen.Emit(OpCodes.Call, enumerableSerializer);
            // }
            else
            {
                ilGen.Emit(OpCodes.Pop);
                ilGen.Emit(OpCodes.Pop);
                ilGen.EmitLog($"RePacker - Pack: Stack of type {fieldInfo.FieldType.Name} is not supported");
            }
        }
    }
}
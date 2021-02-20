using System;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Buffers;
using RePacker.Utils;
using Buffer = RePacker.Buffers.Buffer;

namespace RePacker.Builder
{
    internal class ArrayGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(Array);

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo serializeBlittableArrayMethod = typeof(BufferExt)
            .GetMethod(nameof(BufferExt.PackBlittableArray));
        MethodInfo serializeArrayMethod = typeof(PackerCollectionsExt)
            .GetMethod(nameof(PackerCollectionsExt.PackArray));

        MethodInfo deserializeBlittableArrayMethod = typeof(BufferExt)
            .GetMethod(nameof(BufferExt.UnpackUnmanagedArrayOut));
        MethodInfo deserializeArrayMethod = typeof(PackerCollectionsExt)
            .GetMethod(nameof(PackerCollectionsExt.UnpackArray));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            var elementType = fieldInfo.FieldType.GetElementType();

            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldloca_S, 0);
                ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                var arrayUnpacker = deserializeArrayMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, arrayUnpacker);
            }
            else if (elementType.IsValueType || elementType.IsUnmanagedStruct())
            {
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);

                ilGen.Emit(OpCodes.Ldloca_S, 0);
                ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                var arrayUnpacker = deserializeBlittableArrayMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, arrayUnpacker);
            }
            else
            {
                ilGen.EmitLog($"RePacker - Unpack: Array of type {fieldInfo.FieldType.Name} is not supported");
            }
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            var elementType = fieldInfo.FieldType.GetElementType();

            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldarga_S, 1);
                ilGen.Emit(OpCodes.Ldfld, fieldInfo);

                var arraySerializer = serializeArrayMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, arraySerializer);
            }
            else if (elementType.IsValueType || elementType.IsUnmanagedStruct())
            {
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);

                ilGen.Emit(OpCodes.Ldarga_S, 1);
                ilGen.Emit(OpCodes.Ldfld, fieldInfo);

                var arraySerializer = serializeBlittableArrayMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, arraySerializer);
            }
            else
            {
                ilGen.EmitLog($"RePacker - Pack: Array of type {fieldInfo.FieldType.Name} is not supported");
            }
        }
    }
}
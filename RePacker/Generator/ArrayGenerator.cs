using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
{
    public class ArrayGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(Array);

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo serializeBlittableArrayMethod = typeof(PackerExtensions).GetMethod(nameof(PackerExtensions.EncodeBlittableArray));
        MethodInfo serializeArrayMethod = typeof(PackerExtensions).GetMethod(nameof(PackerExtensions.PackArray));

        MethodInfo deserializeBlittableArrayMethod = typeof(PackerExtensions).GetMethod(nameof(PackerExtensions.UnpackBlittableArray));
        MethodInfo deserializeArrayMethod = typeof(PackerExtensions).GetMethod(nameof(PackerExtensions.UnpackArray));

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

                var arraySerializer = deserializeArrayMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, arraySerializer, Type.EmptyTypes);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);

                ilGen.Emit(OpCodes.Ldloca_S, 0);
                ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                var arraySerializer = deserializeBlittableArrayMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, arraySerializer, Type.EmptyTypes);
            }
            else
            {
                ilGen.EmitWriteLine($"RePacker - Unpack: Array of type {fieldInfo.FieldType.Name} is not supported");
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
                ilGen.EmitCall(OpCodes.Call, arraySerializer, Type.EmptyTypes);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);

                ilGen.Emit(OpCodes.Ldarga_S, 1);
                ilGen.Emit(OpCodes.Ldfld, fieldInfo);

                var arraySerializer = serializeBlittableArrayMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, arraySerializer, Type.EmptyTypes);
            }
            else
            {
                ilGen.EmitWriteLine($"RePacker - Pack: Array of type {fieldInfo.FieldType.Name} is not supported");
            }
        }
    }
}
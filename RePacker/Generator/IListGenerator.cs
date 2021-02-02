using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Generator
{
    public class IListGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(IList<>);

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo deserializeIListMethod = typeof(Serializer).GetMethod(nameof(Serializer.UnpackIList));
        MethodInfo deserializeIListBlittableMethod = typeof(Serializer).GetMethod(nameof(Serializer.UnpackIListBlittable));

        MethodInfo serializeIListMethod = typeof(Serializer).GetMethod(nameof(Serializer.PackIList));
        MethodInfo serializeIListBlittableMethod = typeof(Serializer).GetMethod(nameof(Serializer.PackIListBlittable));

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
                ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                var arraySerializer = deserializeIListBlittableMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, arraySerializer, Type.EmptyTypes);
            }
            else
            {
                ilGen.Emit(OpCodes.Pop);
                ilGen.Emit(OpCodes.Pop);
                ilGen.EmitWriteLine($"RePacker - Unpack: Array of type {fieldInfo.FieldType.Name} is not supported");
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
                ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                var arraySerializer = serializeIListBlittableMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, arraySerializer, Type.EmptyTypes);
            }
            else
            {
                ilGen.Emit(OpCodes.Pop);
                ilGen.Emit(OpCodes.Pop);
                ilGen.EmitWriteLine($"RePacker - Pack: IList of type {fieldInfo.FieldType.Name} is not supported");
            }
        }
    }
}
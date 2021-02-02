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

            Type listType = fieldInfo.FieldType.GenericTypeArguments[0];

            if (TypeCache.TryGetTypeInfo(listType, out var typeInfo))
            {
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldloca_S, 0);
                ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                var listSerializer = deserializeIListMethod.MakeGenericMethod(listType);
                ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
            }
            else if (listType.IsValueType || (listType.IsStruct() && listType.IsUnmanagedStruct()))
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldloca_S, 0);
                ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                var arraySerializer = deserializeIListBlittableMethod.MakeGenericMethod(listType);
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

            Type listType = fieldInfo.FieldType.GenericTypeArguments[0];

            if (TypeCache.TryGetTypeInfo(listType, out var typeInfo))
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldarga_S, 1);
                ilGen.Emit(OpCodes.Ldfld, fieldInfo);

                var listSerializer = serializeIListMethod.MakeGenericMethod(listType);
                ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
            }
            else if (listType.IsValueType || (listType.IsStruct() && listType.IsUnmanagedStruct()))
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldarga_S, 1);
                ilGen.Emit(OpCodes.Ldfld, fieldInfo);

                var arraySerializer = serializeIListBlittableMethod.MakeGenericMethod(listType);
                ilGen.EmitCall(OpCodes.Call, arraySerializer, Type.EmptyTypes);
            }
            else
            {
                ilGen.EmitWriteLine($"RePacker - Pack: IList of type {fieldInfo.FieldType.Name} is not supported");
            }
        }
    }
}
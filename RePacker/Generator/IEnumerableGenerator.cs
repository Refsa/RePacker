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
    public class IEnumerableGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(IEnumerable<>);

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo deserializeIEnumerableMethod = typeof(Serializer).GetMethod(nameof(Serializer.UnpackIEnumerable));
        MethodInfo deserializeIEnumerableBlittableMethod = typeof(Serializer).GetMethod(nameof(Serializer.UnpackIEnumerableBlittable));

        MethodInfo serializeIEnumerableMethod = typeof(Serializer).GetMethod(nameof(Serializer.PackIEnumerable));
        MethodInfo serializeIEnumerableBlittableMethod = typeof(Serializer).GetMethod(nameof(Serializer.PackIEnumerableBlittable));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            Type elementType = fieldInfo.FieldType.GenericTypeArguments[0];
            IEnumerableType listType = GetActualType(fieldInfo.FieldType, elementType);

            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldc_I4, (int)(byte)listType);

                ilGen.Emit(OpCodes.Ldloca_S, 0);
                ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                var listSerializer = deserializeIEnumerableMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldc_I4, (int)(byte)listType);

                ilGen.Emit(OpCodes.Ldloca_S, 0);
                ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                var arraySerializer = deserializeIEnumerableBlittableMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, arraySerializer, Type.EmptyTypes);
            }
            else
            {
                ilGen.EmitWriteLine($"RePacker - Unpack: Array of type {fieldInfo.FieldType.Name} is not supported");
                return;
            }

            // Convert into actual type
            // ilGen.Emit(OpCodes.Ldloca_S, 0);
            // ilGen.Emit(OpCodes.Ldflda, fieldInfo);
            // ilGen.Emit(OpCodes.Castclass, fieldInfo.FieldType);
            // ilGen.Emit(OpCodes.Pop);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            Type elementType = fieldInfo.FieldType.GenericTypeArguments[0];

            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldarga_S, 1);
                ilGen.Emit(OpCodes.Ldfld, fieldInfo);

                var listSerializer = serializeIEnumerableMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldarga_S, 1);
                ilGen.Emit(OpCodes.Ldfld, fieldInfo);

                var arraySerializer = serializeIEnumerableBlittableMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, arraySerializer, Type.EmptyTypes);
            }
            else
            {
                ilGen.EmitWriteLine($"RePacker - Pack: IEnumerable of type {fieldInfo.FieldType.Name} is not supported");
            }
        }

        IEnumerableType GetActualType(Type type, Type elementType)
        {
            return type switch
            {
                var x when type == typeof(HashSet<>).MakeGenericType(elementType) => IEnumerableType.HashSet,
                var x when type == typeof(Queue<>).MakeGenericType(elementType) => IEnumerableType.Queue,
                var x when type == typeof(Stack<>).MakeGenericType(elementType) => IEnumerableType.Stack,
                _ => IEnumerableType.None,
            };
        }
    }
}
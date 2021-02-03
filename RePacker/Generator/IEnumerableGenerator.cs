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

        MethodInfo deserializeIEnumerableMethod = typeof(PackerExtensions).GetMethod(nameof(PackerExtensions.UnpackIEnumerable));
        MethodInfo deserializeIEnumerableBlittableMethod = typeof(PackerExtensions).GetMethod(nameof(PackerExtensions.UnpackIEnumerableBlittable));

        MethodInfo serializeIEnumerableMethod = typeof(PackerExtensions).GetMethod(nameof(PackerExtensions.PackIEnumerable));
        MethodInfo serializeIEnumerableBlittableMethod = typeof(PackerExtensions).GetMethod(nameof(PackerExtensions.PackIEnumerableBlittable));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            Type elementType = fieldInfo.FieldType.GenericTypeArguments[0];
            IEnumerableType listType = GetActualType(fieldInfo.FieldType, elementType);

            ilGen.Emit(OpCodes.Ldarg_0);

            ilGen.Emit(OpCodes.Ldc_I4, (int)(byte)listType);

            ilGen.Emit(OpCodes.Ldloca_S, 0);
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);

            if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
            {
                var enumerableDeserializer = deserializeIEnumerableMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, enumerableDeserializer, Type.EmptyTypes);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                var enumerableDeserializer = deserializeIEnumerableBlittableMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, enumerableDeserializer, Type.EmptyTypes);
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
                var enumerableSerializer = serializeIEnumerableMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, enumerableSerializer, Type.EmptyTypes);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                var enumerableSerializer = serializeIEnumerableBlittableMethod.MakeGenericMethod(elementType);
                ilGen.EmitCall(OpCodes.Call, enumerableSerializer, Type.EmptyTypes);
            }
            else
            {
                ilGen.Emit(OpCodes.Pop);
                ilGen.Emit(OpCodes.Pop);
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
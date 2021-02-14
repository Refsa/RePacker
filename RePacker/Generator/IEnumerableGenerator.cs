using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using static Refsa.RePacker.Builder.PackerCollectionsExt;

namespace Refsa.RePacker.Builder
{
    internal class IEnumerableGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(IEnumerable<>);

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo deserializeIEnumerableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackIEnumerable));
        MethodInfo deserializeIEnumerableBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackIEnumerableBlittable));

        MethodInfo serializeIEnumerableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackIEnumerable));
        MethodInfo serializeIEnumerableBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackIEnumerableBlittable));

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
                ilGen.Emit(OpCodes.Call, enumerableDeserializer);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                var enumerableDeserializer = deserializeIEnumerableBlittableMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, enumerableDeserializer);
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
                ilGen.Emit(OpCodes.Call, enumerableSerializer);
            }
            else if (elementType.IsValueType || (elementType.IsStruct() && elementType.IsUnmanagedStruct()))
            {
                var enumerableSerializer = serializeIEnumerableBlittableMethod.MakeGenericMethod(elementType);
                ilGen.Emit(OpCodes.Call, enumerableSerializer);
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
            if (type == typeof(HashSet<>).MakeGenericType(elementType))
            {
                return IEnumerableType.HashSet;
            }
            if (type == typeof(Queue<>).MakeGenericType(elementType))
            {
                return IEnumerableType.Queue;
            }
            if (type == typeof(Stack<>).MakeGenericType(elementType))
            {
                return IEnumerableType.Stack;
            }

            return IEnumerableType.None;
        }
    }
}
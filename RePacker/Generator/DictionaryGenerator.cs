using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;

namespace Refsa.RePacker.Builder
{
    internal class DictionaryGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(Dictionary<,>);

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo deserializeIListMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackIList));
        MethodInfo deserializeIListBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.UnpackIListBlittable));

        MethodInfo serializeIEnumerableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackIEnumerable));
        MethodInfo serializeIEnumerableBlittableMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.PackIEnumerableBlittable));

        MethodInfo recreateDictMethod = typeof(PackerCollectionsExt).GetMethod(nameof(PackerCollectionsExt.RecreateDictionary));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            // [Key Key Key Value Value Value]
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            Type[] kvTypes = fieldInfo.FieldType.GenericTypeArguments;
            (Type keyType, Type valueType) = (kvTypes[0], kvTypes[1]);

            var keyConst = typeof(List<>).MakeGenericType(keyType).GetConstructor(Type.EmptyTypes);
            var valConst = typeof(List<>).MakeGenericType(valueType).GetConstructor(Type.EmptyTypes);

            ilGen.DeclareLocal(typeof(List<>).MakeGenericType(keyType));
            ilGen.DeclareLocal(typeof(List<>).MakeGenericType(valueType));

            ilGen.Emit(OpCodes.Newobj, keyConst);
            ilGen.Emit(OpCodes.Stloc_1);

            ilGen.Emit(OpCodes.Newobj, valConst);
            ilGen.Emit(OpCodes.Stloc_2);

            // Keys
            {
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldloca_S, 1);

                if (TypeCache.TryGetTypeInfo(keyType, out var typeInfo))
                {
                    var listDeserializer = deserializeIListMethod.MakeGenericMethod(keyType);
                    ilGen.Emit(OpCodes.Call, listDeserializer);
                }
                else if (keyType.IsValueType || (keyType.IsStruct() && keyType.IsUnmanagedStruct()))
                {
                    var listDeserializer = deserializeIListBlittableMethod.MakeGenericMethod(keyType);
                    ilGen.Emit(OpCodes.Call, listDeserializer);
                }
                else
                {
                    ilGen.EmitLog($"RePacker - Pack: Dictionary of type {fieldInfo.FieldType.Name} is not supported");
                }
            }

            // Values
            {
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldloca_S, 2);

                if (TypeCache.TryGetTypeInfo(valueType, out var typeInfo))
                {
                    var listDeserializer = deserializeIListMethod.MakeGenericMethod(valueType);
                    ilGen.Emit(OpCodes.Call, listDeserializer);
                }
                else if (valueType.IsValueType || (valueType.IsStruct() && valueType.IsUnmanagedStruct()))
                {
                    var listDeserializer = deserializeIListBlittableMethod.MakeGenericMethod(valueType);
                    ilGen.Emit(OpCodes.Call, listDeserializer);
                }
                else
                {
                    ilGen.EmitLog($"RePacker - Pack: Dictionary of type {fieldInfo.FieldType.Name} is not supported");
                }
            }

            // Recreate Dictionary
            {
                var genericDict = recreateDictMethod.MakeGenericMethod(kvTypes);

                ilGen.Emit(OpCodes.Ldloc, 1);
                ilGen.Emit(OpCodes.Ldloc, 2);

                ilGen.Emit(OpCodes.Ldloca_S, 0);
                ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                ilGen.Emit(OpCodes.Call, genericDict);
            }
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            Type[] kvTypes = fieldInfo.FieldType.GenericTypeArguments;
            (Type keyType, Type valueType) = (kvTypes[0], kvTypes[1]);

            var genDict = fieldInfo.FieldType;

            /* ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldfld, fieldInfo);
            ilGen.Emit(OpCodes.Castclass, typeof(IEnumerable<>).MakeGenericType(typeof(KeyValuePair<,>).MakeGenericType(kvTypes)));
            ilGen.Emit(OpCodes.Pop); */

            var getKeys = genDict.GetProperty("Keys").GetMethod;
            var getValues = genDict.GetProperty("Values").GetMethod;

            // Keys
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldarga_S, 1);
                ilGen.Emit(OpCodes.Ldfld, fieldInfo);
                ilGen.Emit(OpCodes.Callvirt, getKeys);

                if (TypeCache.TryGetTypeInfo(keyType, out var typeInfo))
                {
                    var listSerializer = serializeIEnumerableMethod.MakeGenericMethod(keyType);
                    ilGen.Emit(OpCodes.Call, listSerializer);
                }
                else if (keyType.IsValueType || (keyType.IsStruct() && keyType.IsUnmanagedStruct()))
                {
                    var listSerializer = serializeIEnumerableBlittableMethod.MakeGenericMethod(keyType);
                    ilGen.Emit(OpCodes.Call, listSerializer);
                }
                else
                {
                    ilGen.Emit(OpCodes.Pop);
                    ilGen.Emit(OpCodes.Pop);
                    ilGen.EmitLog($"RePacker - Pack: Dictionary with keys of type {keyType} is not supported");
                }
            }

            // TODO: Unroll Keys if values arent supported

            // Values
            {
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldarga_S, 1);
                ilGen.Emit(OpCodes.Ldfld, fieldInfo);
                ilGen.Emit(OpCodes.Callvirt, getValues);

                if (TypeCache.TryGetTypeInfo(valueType, out var typeInfo))
                {
                    var listSerializer = serializeIEnumerableMethod.MakeGenericMethod(valueType);
                    ilGen.Emit(OpCodes.Call, listSerializer);
                }
                else if (valueType.IsValueType || (valueType.IsStruct() && valueType.IsUnmanagedStruct()))
                {
                    var listSerializer = serializeIEnumerableBlittableMethod.MakeGenericMethod(valueType);
                    ilGen.Emit(OpCodes.Call, listSerializer);
                }
                else
                {
                    ilGen.Emit(OpCodes.Pop);
                    ilGen.Emit(OpCodes.Pop);
                    ilGen.EmitLog($"RePacker - Pack: Dictionary with values of type {valueType} is not supported");
                }
            }
        }
    }
}
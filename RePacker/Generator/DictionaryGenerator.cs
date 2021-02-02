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
    public class DictionaryGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(Dictionary<,>);

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo deserializeIListMethod = typeof(Serializer).GetMethod(nameof(Serializer.UnpackIList));
        MethodInfo deserializeIListBlittableMethod = typeof(Serializer).GetMethod(nameof(Serializer.UnpackIListBlittable));

        MethodInfo serializeIEnumerableMethod = typeof(Serializer).GetMethod(nameof(Serializer.PackIEnumerable));
        MethodInfo serializeIEnumerableBlittableMethod = typeof(Serializer).GetMethod(nameof(Serializer.PackIEnumerableBlittable));

        MethodInfo recreateDictMethod = typeof(Serializer).GetMethod(nameof(Serializer.RecreateDictionary));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            // [Key Key Key Value Value Value]
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            Type[] kvTypes = fieldInfo.FieldType.GenericTypeArguments;

            var keyConst = typeof(List<>).MakeGenericType(kvTypes[0]).GetConstructor(Type.EmptyTypes);
            var valConst = typeof(List<>).MakeGenericType(kvTypes[1]).GetConstructor(Type.EmptyTypes);

            ilGen.DeclareLocal(typeof(List<>).MakeGenericType(kvTypes[0]));
            ilGen.DeclareLocal(typeof(List<>).MakeGenericType(kvTypes[1]));

            ilGen.Emit(OpCodes.Newobj, keyConst);
            ilGen.Emit(OpCodes.Stloc_1);

            ilGen.Emit(OpCodes.Newobj, valConst);
            ilGen.Emit(OpCodes.Stloc_2);

            // Keys
            {
                Type keyType = kvTypes[0];

                if (TypeCache.TryGetTypeInfo(keyType, out var typeInfo))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);

                    ilGen.Emit(OpCodes.Ldloca_S, 1);
                    ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                    var listDeserializer = deserializeIListMethod.MakeGenericMethod(keyType);
                    ilGen.EmitCall(OpCodes.Call, listDeserializer, Type.EmptyTypes);
                }
                else if (keyType.IsValueType || (keyType.IsStruct() && keyType.IsUnmanagedStruct()))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);

                    ilGen.Emit(OpCodes.Ldloca_S, 1);
                    ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                    var listDeserializer = deserializeIListBlittableMethod.MakeGenericMethod(keyType);
                    ilGen.EmitCall(OpCodes.Call, listDeserializer, Type.EmptyTypes);
                }
                else
                {
                    ilGen.EmitWriteLine($"RePacker - Pack: Dictionary of type {fieldInfo.FieldType.Name} is not supported");
                }
            }

            // Values
            {
                Type valueType = kvTypes[1];

                if (TypeCache.TryGetTypeInfo(valueType, out var typeInfo))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);

                    ilGen.Emit(OpCodes.Ldloca_S, 2);
                    ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                    var listDeserializer = deserializeIListMethod.MakeGenericMethod(valueType);
                    ilGen.EmitCall(OpCodes.Call, listDeserializer, Type.EmptyTypes);
                }
                else if (valueType.IsValueType || (valueType.IsStruct() && valueType.IsUnmanagedStruct()))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);

                    ilGen.Emit(OpCodes.Ldloca_S, 2);
                    ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                    var listDeserializer = deserializeIListBlittableMethod.MakeGenericMethod(valueType);
                    ilGen.EmitCall(OpCodes.Call, listDeserializer, Type.EmptyTypes);
                }
                else
                {
                    ilGen.EmitWriteLine($"RePacker - Pack: Dictionary of type {fieldInfo.FieldType.Name} is not supported");
                }
            }

            // Recreate Dictionary
            {
                var genericDict = recreateDictMethod.MakeGenericMethod(kvTypes);

                ilGen.Emit(OpCodes.Ldloc_1);
                ilGen.Emit(OpCodes.Ldloc_2);

                ilGen.Emit(OpCodes.Ldloca_S, 0);
                ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                ilGen.EmitCall(OpCodes.Call, genericDict, Type.EmptyTypes);
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
                if (TypeCache.TryGetTypeInfo(keyType, out var typeInfo))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);

                    ilGen.Emit(OpCodes.Ldarga_S, 1);
                    ilGen.Emit(OpCodes.Ldfld, fieldInfo);
                    ilGen.Emit(OpCodes.Callvirt, getKeys);

                    var listSerializer = serializeIEnumerableMethod.MakeGenericMethod(keyType);
                    ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
                }
                else if (keyType.IsValueType || (keyType.IsStruct() && keyType.IsUnmanagedStruct()))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);

                    ilGen.Emit(OpCodes.Ldarga_S, 1);
                    ilGen.Emit(OpCodes.Ldfld, fieldInfo);
                    ilGen.Emit(OpCodes.Callvirt, getKeys);
                    ilGen.Emit(OpCodes.Castclass, typeof(IEnumerable<>).MakeGenericType(keyType));

                    var listSerializer = serializeIEnumerableBlittableMethod.MakeGenericMethod(keyType);
                    ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
                }
                else
                {
                    ilGen.EmitWriteLine($"RePacker - Pack: Dictionary of type {fieldInfo.FieldType.Name} is not supported");
                }
            }

            // Values
            {
                if (TypeCache.TryGetTypeInfo(valueType, out var typeInfo))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);

                    ilGen.Emit(OpCodes.Ldarga_S, 1);
                    ilGen.Emit(OpCodes.Ldfld, fieldInfo);
                    ilGen.Emit(OpCodes.Callvirt, getValues);

                    var listSerializer = serializeIEnumerableMethod.MakeGenericMethod(valueType);
                    ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
                }
                else if (valueType.IsValueType || (valueType.IsStruct() && valueType.IsUnmanagedStruct()))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);

                    ilGen.Emit(OpCodes.Ldarga_S, 1);
                    ilGen.Emit(OpCodes.Ldfld, fieldInfo);
                    ilGen.Emit(OpCodes.Callvirt, getValues);
                    ilGen.Emit(OpCodes.Castclass, typeof(IEnumerable<>).MakeGenericType(valueType));

                    var listSerializer = serializeIEnumerableBlittableMethod.MakeGenericMethod(valueType);
                    ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
                }
                else
                {
                    ilGen.EmitWriteLine($"RePacker - Pack: Dictionary of type {fieldInfo.FieldType.Name} is not supported");
                }
            }
        }
    }
}
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
            (Type keyType, Type valueType) = (kvTypes[0], kvTypes[1]);

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
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldloca_S, 1);
                ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                if (TypeCache.TryGetTypeInfo(keyType, out var typeInfo))
                {
                    var listDeserializer = deserializeIListMethod.MakeGenericMethod(keyType);
                    ilGen.EmitCall(OpCodes.Call, listDeserializer, Type.EmptyTypes);
                }
                else if (keyType.IsValueType || (keyType.IsStruct() && keyType.IsUnmanagedStruct()))
                {
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
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldloca_S, 2);
                ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                if (TypeCache.TryGetTypeInfo(valueType, out var typeInfo))
                {
                    var listDeserializer = deserializeIListMethod.MakeGenericMethod(valueType);
                    ilGen.EmitCall(OpCodes.Call, listDeserializer, Type.EmptyTypes);
                }
                else if (valueType.IsValueType || (valueType.IsStruct() && valueType.IsUnmanagedStruct()))
                {
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
                ilGen.Emit(OpCodes.Ldarg_0);

                ilGen.Emit(OpCodes.Ldarga_S, 1);
                ilGen.Emit(OpCodes.Ldfld, fieldInfo);
                ilGen.Emit(OpCodes.Callvirt, getKeys);

                if (TypeCache.TryGetTypeInfo(keyType, out var typeInfo))
                {
                    var listSerializer = serializeIEnumerableMethod.MakeGenericMethod(keyType);
                    ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
                }
                else if (keyType.IsValueType || (keyType.IsStruct() && keyType.IsUnmanagedStruct()))
                {
                    var listSerializer = serializeIEnumerableBlittableMethod.MakeGenericMethod(keyType);
                    ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
                }
                else
                {
                    ilGen.Emit(OpCodes.Pop);
                    ilGen.Emit(OpCodes.Pop);
                    ilGen.EmitWriteLine($"RePacker - Pack: Dictionary with keys of type {keyType} is not supported");
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
                    ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
                }
                else if (valueType.IsValueType || (valueType.IsStruct() && valueType.IsUnmanagedStruct()))
                {
                    var listSerializer = serializeIEnumerableBlittableMethod.MakeGenericMethod(valueType);
                    ilGen.EmitCall(OpCodes.Call, listSerializer, Type.EmptyTypes);
                }
                else
                {
                    ilGen.Emit(OpCodes.Pop);
                    ilGen.Emit(OpCodes.Pop);
                    ilGen.EmitWriteLine($"RePacker - Pack: Dictionary with values of type {valueType} is not supported");
                }
            }
        }
    }
}
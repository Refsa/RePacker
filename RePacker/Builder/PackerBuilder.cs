using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Buffers;
using RePacker.Utils;
using Buffer = RePacker.Buffers.Buffer;

namespace RePacker.Builder
{
    internal static class PackerBuilder
    {
        public static MethodInfo CreateUnpacker(TypeCache.Info info)
        {
            Type[] typeParams = new Type[] { typeof(Buffer) };

            var deserBuilder = new DynamicMethod(
                $"{info.Type.FullName}_Deserialize",
                info.Type,
                typeParams,
                typeof(PackerBuilder),
                true
            );

            var paramBuilder = deserBuilder.DefineParameter(0, ParameterAttributes.None, "buffer");

            MethodInfo bufferPop = typeof(BufferUtils).GetMethod(nameof(BufferUtils.Unpack));

            var parameters = new Type[1];
            MethodInfo bufferPopGeneric = null;

            var ilGen = deserBuilder.GetILGenerator();
            {
                ilGen.DeclareLocal(info.Type);
                // ilGen.EmitLog($"Deserializing {info.Type.Name}");

                if (info.IsUnmanaged && !info.HasCustomSerializer)
                {
                    goto Blittable;
                }
                else
                {
                    // Create class instance
                    if (!info.Type.IsValueType)
                    {
                        var ci = info.Type.GetConstructor(Type.EmptyTypes);
                        ilGen.Emit(OpCodes.Newobj, ci);
                        ilGen.Emit(OpCodes.Stloc_0);
                    }

                    goto PerField;
                }

            Blittable:
                // Buffer -> buffer -> caller
                ilGen.Emit(OpCodes.Ldarg_0);

                // Load output target
                ilGen.Emit(OpCodes.Ldloca_S, 0);

                parameters[0] = info.Type;
                bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);

                ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);

                goto Finished;

            PerField:
                for (int i = 0; i < info.SerializedFields.Length; i++)
                {
                    var field = info.SerializedFields[i];

                    (GeneratorType gt, Type t) = (GeneratorType.None, null);

                    switch (Type.GetTypeCode(field.FieldType))
                    {
                        // MANAGED DATA
                        case TypeCode.Empty:
                            break;
                        case TypeCode.DateTime:
                            (gt, t) = (GeneratorType.DateTime, null);
                            break;
                        case TypeCode.Object:
                            if (TypeCache.TryGetTypeInfo(field.FieldType, out var nestedTypeInfo))
                            {
                                (gt, t) = (GeneratorType.RePacker, null);
                            }
                            else if (field.FieldType.IsGenericType)
                            {
                                var genType = field.FieldType.GetGenericTypeDefinition();
                                (gt, t) = (GeneratorType.Object, genType);
                            }
                            else if (field.FieldType.IsStruct() && field.FieldType.IsUnmanagedStruct())
                            {
                                (gt, t) = (GeneratorType.Struct, null);
                            }
                            else if (field.FieldType.IsArray)
                            {
                                (gt, t) = (GeneratorType.Object, typeof(Array));
                            }
                            break;
                        case TypeCode.String:
                            (gt, t) = (GeneratorType.String, typeof(string));
                            break;

                        // UNMANAGED
                        default:
                            (gt, t) = (GeneratorType.Unmanaged, null);
                            break;
                    }

                    if (gt != GeneratorType.None && GeneratorLookup.TryGet(gt, t, out var generator) && generator != null)
                    {
                        generator.GenerateDeserializer(ilGen, field);
                    }
                    else
                    {
                        RePacking.Logger.Warn($"RePacker - Unpack: Type {field.FieldType.Name} on {info.Type.Name} is not supported");
                    }
                }
                goto Finished;

            Finished:
                // ilGen.EmitLog($"{info.Type.Name} is Deserialized");
                ilGen.Emit(OpCodes.Ldloc_0);
                ilGen.Emit(OpCodes.Ret);
            }

            return deserBuilder;
        }

        public static MethodInfo CreatePacker(TypeCache.Info info)
        {
            Type[] typeParams = new Type[2] { typeof(Buffer), info.Type };

            var serBuilder = new DynamicMethod(
                $"{info.Type.FullName}_Serialize",
                typeof(void),
                typeParams,
                typeof(PackerBuilder),
                true
            );

            serBuilder.DefineParameter(0, ParameterAttributes.None, "buffer");
            serBuilder.DefineParameter(1, ParameterAttributes.In, info.Type.Name.ToLower());

            MethodInfo bufferPush = typeof(BufferUtils).GetMethod(nameof(BufferUtils.Pack));

            var parameters = new Type[1];
            MethodInfo bufferPushGeneric = null;

            var ilGen = serBuilder.GetILGenerator();
            {
                // ilGen.EmitLog($"Serializing {info.Type.Name}");

                if (info.IsUnmanaged && !info.HasCustomSerializer)
                {
                    goto Blittable;
                }
                else
                {
                    goto PerField;
                }

            Blittable:
                parameters[0] = info.Type;
                bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);

                // Load Buffer
                ilGen.Emit(OpCodes.Ldarg_0);

                // Load target for serialization
                ilGen.Emit(OpCodes.Ldarga_S, 1);

                // Call serializer
                ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);

                goto Finished;

            PerField:
                for (int i = 0; i < info.SerializedFields.Length; i++)
                {
                    var field = info.SerializedFields[i];

                    (GeneratorType gt, Type t) = (GeneratorType.None, null);

                    switch (Type.GetTypeCode(field.FieldType))
                    {
                        // MANAGED DATA AND STRUCTS
                        case TypeCode.Empty:
                            break;
                        case TypeCode.DateTime:
                            (gt, t) = (GeneratorType.DateTime, null);
                            break;
                        case TypeCode.Object:
                            if (TypeCache.TryGetTypeInfo(field.FieldType, out var nestedTypeInfo))
                            {
                                (gt, t) = (GeneratorType.RePacker, null);
                            }
                            else if (field.FieldType.IsGenericType)
                            {
                                var genType = field.FieldType.GetGenericTypeDefinition();
                                (gt, t) = (GeneratorType.Object, genType);
                            }
                            else if (field.FieldType.IsStruct() && field.FieldType.IsUnmanagedStruct())
                            {
                                (gt, t) = (GeneratorType.Struct, null);
                            }
                            else if (field.FieldType.IsArray)
                            {
                                (gt, t) = (GeneratorType.Object, typeof(Array));
                            }
                            break;
                        case TypeCode.String:
                            (gt, t) = (GeneratorType.String, typeof(string));
                            break;

                        // UNMANAGED DATA
                        default:
                            (gt, t) = (GeneratorType.Unmanaged, null);
                            break;
                    }

                    if (gt != GeneratorType.None && GeneratorLookup.TryGet(gt, t, out var generator) && generator != null)
                    {
                        generator.GenerateSerializer(ilGen, field);
                    }
                    else
                    {
                        RePacking.Logger.Warn($"RePacker - Pack: Type {field.FieldType.Name} on {info.Type.Name} is not supported");
                    }
                }
                goto Finished;

            // ilGen.EmitLog($"{info.Type.Name} is Serialized");
            Finished:
                ilGen.Emit(OpCodes.Ret);
            }

            return serBuilder;
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
{
    internal static class PackerBuilder
    {
        public static MethodInfo CreateUnpacker(TypeCache.Info info)
        {
            Type[] typeParams = new Type[] { typeof(Refsa.RePacker.Buffers.BoxedBuffer) };

            var deserBuilder = new DynamicMethod(
                $"{info.Type.FullName}_Deserialize",
                info.Type,
                typeParams,
                typeof(PackerBuilder),
                true
            );

            var paramBuilder = deserBuilder.DefineParameter(0, ParameterAttributes.None, "buffer");

            FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

            // MethodInfo bufferPop = typeof(Buffer)
            //     .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            //     .Where(mi => mi.Name == "Pop" && mi.GetParameters().Length == 1).First();

            MethodInfo bufferPop = typeof(Buffer).GetMethod(nameof(Buffer.Unpack));

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
                // BoxedBuffer -> buffer -> caller
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);

                // Load output target
                ilGen.Emit(OpCodes.Ldloca_S, 0);

                parameters[0] = info.Type;
                bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);

                ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                ilGen.Emit(OpCodes.Pop);

                goto Finished;

            PerField:
                for (int i = 0; i < info.SerializedFields.Length; i++)
                {
                    var field = info.SerializedFields[i];

                    // PERF: Do we need to push/pop this from the stack??
                    // BoxedBuffer -> buffer -> caller
                    ilGen.Emit(OpCodes.Ldarg_0);
                    if (Type.GetTypeCode(field.FieldType) != TypeCode.Object)
                    {
                        ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);
                    }

                    // Byte -> output field
                    if (info.Type.IsValueType)
                    {
                        ilGen.Emit(OpCodes.Ldloca_S, 0);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Ldloc_S, 0);
                    }
                    ilGen.Emit(OpCodes.Ldflda, field);

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

                    if (gt != GeneratorType.None && GeneratorLookup.TryGet(gt, t, out var generator))
                    {
                        generator.GenerateDeserializer(ilGen, field);
                    }
                    else
                    {
                        ilGen.EmitLog($"RePacker - Unpack: Type {field.FieldType.Name} on {info.Type.Name} is not supported");
                        ilGen.Emit(OpCodes.Pop);
                        ilGen.Emit(OpCodes.Pop);
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
            Type[] typeParams = new Type[2] { typeof(BoxedBuffer), info.Type };

            var serBuilder = new DynamicMethod(
                $"{info.Type.FullName}_Serialize",
                typeof(void),
                typeParams,
                typeof(PackerBuilder),
                true
            );

            serBuilder.DefineParameter(0, ParameterAttributes.None, "buffer");
            serBuilder.DefineParameter(1, ParameterAttributes.In, info.Type.Name.ToLower());

            // MethodInfo bufferPush = typeof(Buffer)
                // .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                // .Where(mi => mi.Name == "Push" && mi.GetParameters().Length == 1).First();

            MethodInfo bufferPush = typeof(Buffer).GetMethod(nameof(Buffer.Pack));

            FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

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
                ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);

                // Load target for serialization
                ilGen.Emit(OpCodes.Ldarga_S, 1);

                // Call serializer
                ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                ilGen.Emit(OpCodes.Pop);

                goto Finished;

            PerField:
                for (int i = 0; i < info.SerializedFields.Length; i++)
                {
                    var field = info.SerializedFields[i];

                    ilGen.Emit(OpCodes.Ldarg_0);
                    if (Type.GetTypeCode(field.FieldType) != TypeCode.Object)
                    {
                        ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);
                    }

                    if (info.Type.IsValueType)
                    {
                        ilGen.Emit(OpCodes.Ldarga_S, 1);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Ldarg_S, 1);
                    }
                    ilGen.Emit(OpCodes.Ldflda, field);

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

                    if (gt != GeneratorType.None && GeneratorLookup.TryGet(gt, t, out var generator))
                    {
                        generator.GenerateSerializer(ilGen, field);
                    }
                    else
                    {
                        ilGen.EmitLog($"RePacker - Pack: Type {field.FieldType.Name} on {info.Type.Name} is not supported");
                        ilGen.Emit(OpCodes.Pop);
                        ilGen.Emit(OpCodes.Pop);
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
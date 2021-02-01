

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

using Refsa.RePacker.Buffers;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
{
    public static class SerializerBuilder
    {
        static AssemblyBuilder asmBuilder;
        static ModuleBuilder moduleBuilder;
        static bool isSetup = false;

        public static void Setup()
        {
            if (isSetup) return;

            asmBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.Run);

            moduleBuilder = asmBuilder
                .DefineDynamicModule($"RePackerSerializers");

            isSetup = true;
        }

        public static void Complete()
        {
            moduleBuilder.CreateGlobalFunctions();
        }

        public static Func<MethodInfo> CreateDeserializer(TypeCache.Info info)
        {
            // TODO: MakeByRefType ??
            Type[] typeParams = new Type[] { typeof(Refsa.RePacker.Buffers.BoxedBuffer) };

            var deserBuilder = moduleBuilder.DefineGlobalMethod(
                $"{info.Type.Name}_Deserialize",
                MethodAttributes.Public | MethodAttributes.Static |
                MethodAttributes.HideBySig,
                info.Type,
                typeParams
            );
            deserBuilder.SetImplementationFlags(MethodImplAttributes.Managed);

            var paramBuilder = deserBuilder.DefineParameter(0, ParameterAttributes.None, "buffer");

            FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

            MethodInfo bufferPop = typeof(Buffer)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.Name == "Pop" && mi.GetParameters().Length == 1).First();

            MethodInfo deserializeTypeMethod = typeof(TypeCache).GetMethod(nameof(TypeCache.DeserializeOut));

            var parameters = new Type[1];
            MethodInfo bufferPopGeneric = null;

            var ilGen = deserBuilder.GetILGenerator();
            {
                ilGen.DeclareLocal(info.Type);
                // ilGen.EmitWriteLine($"Deserializing {info.Type.Name}");

                if (info.IsUnmanaged)
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

                    switch (Type.GetTypeCode(field.FieldType))
                    {
                        // MANAGED DATA
                        case TypeCode.Empty:
                            ilGen.EmitWriteLine("RePacker - Unpack: Empty is currently unsupported");
                            ilGen.Emit(OpCodes.Pop);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.DateTime:
                            ilGen.EmitWriteLine("RePacker - Unpack: DateTime is currently unsupported");
                            ilGen.Emit(OpCodes.Pop);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Object:
                            if (TypeCache.TryGetTypeInfo(field.FieldType, out var nestedTypeInfo))
                            {
                                var genericSerializer = deserializeTypeMethod.MakeGenericMethod(field.FieldType);
                                ilGen.EmitCall(OpCodes.Call, genericSerializer, Type.EmptyTypes);
                                ilGen.Emit(OpCodes.Pop);
                            }
                            else
                            {
                                ilGen.EmitWriteLine($"RePacker - Unpack: Object of type {field.FieldType.Name} is not supported");
                                ilGen.Emit(OpCodes.Pop);
                                ilGen.Emit(OpCodes.Pop);
                            }
                            break;
                        case TypeCode.String:
                            var stringDecParams = new Type[] { typeof(Buffer).MakeByRefType(), typeof(string).MakeByRefType() };
                            var decodeString = typeof(Serializer).GetMethod(nameof(Serializer.DecodeString), stringDecParams);

                            ilGen.EmitCall(OpCodes.Call, decodeString, Type.EmptyTypes);
                            break;

                        // UNMANAGED
                        case TypeCode.Boolean:
                            parameters[0] = typeof(bool);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Char:
                            parameters[0] = typeof(char);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.SByte:
                            parameters[0] = typeof(sbyte);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Byte:
                            parameters[0] = typeof(byte);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int16:
                            parameters[0] = typeof(short);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt16:
                            parameters[0] = typeof(ushort);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int32:
                            parameters[0] = typeof(int);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt32:
                            parameters[0] = typeof(uint);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int64:
                            parameters[0] = typeof(long);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt64:
                            parameters[0] = typeof(ulong);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Single:
                            parameters[0] = typeof(float);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Double:
                            parameters[0] = typeof(double);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Decimal:
                            parameters[0] = typeof(decimal);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                    }
                }
                goto Finished;

            Finished:
                // ilGen.EmitWriteLine($"{info.Type.Name} is Deserialized");
                ilGen.Emit(OpCodes.Ldloc_0);
                ilGen.Emit(OpCodes.Ret);
            }

            return () =>
            {
                MethodInfo asGlobal = moduleBuilder.GetMethod(deserBuilder.Name);
                return asGlobal;
                // return asGlobal.CreateDelegate(CreateFunc(info.Type, typeParams));
            };
        }

        public static Func<MethodInfo> CreateSerializer(TypeCache.Info info)
        {
            // Type[] typeParams = info.SerializedFields.Select(e => e.FieldType).ToArray();
            Type[] typeParams = new Type[] { typeof(BoxedBuffer), info.Type };

            var serBuilder = moduleBuilder.DefineGlobalMethod(
                $"{info.Type.Name}_Serialize",
                MethodAttributes.Public | MethodAttributes.Static |
                MethodAttributes.HideBySig,
                null,
                typeParams
            );
            serBuilder.SetImplementationFlags(MethodImplAttributes.Managed);

            serBuilder.DefineParameter(0, ParameterAttributes.None, "buffer");
            serBuilder.DefineParameter(1, ParameterAttributes.In, info.Type.Name.ToLower());

            MethodInfo bufferPush = typeof(Buffer)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.Name == "Push" && mi.GetParameters().Length == 1).First();

            FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

            MethodInfo serializeTypeMethod = typeof(TypeCache).GetMethod(nameof(TypeCache.Serialize));

            var parameters = new Type[1];
            MethodInfo bufferPushGeneric = null;

            var ilGen = serBuilder.GetILGenerator();
            {
                // ilGen.EmitWriteLine($"Serializing {info.Type.Name}");

                if (info.IsUnmanaged)
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

                    switch (Type.GetTypeCode(field.FieldType))
                    {
                        // MANAGED DATA AND STRUCTS
                        case TypeCode.Empty:
                            ilGen.EmitWriteLine("RePacker - Pack: Empty is currently unsupported");
                            ilGen.Emit(OpCodes.Pop);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.DateTime:
                            ilGen.EmitWriteLine("RePacker - Pack: DateTime is currently unsupported");
                            ilGen.Emit(OpCodes.Pop);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Object:
                            if (TypeCache.TryGetTypeInfo(field.FieldType, out var nestedTypeInfo))
                            {
                                var genericSerializer = serializeTypeMethod.MakeGenericMethod(field.FieldType);
                                ilGen.EmitCall(OpCodes.Call, genericSerializer, Type.EmptyTypes);
                            }
                            else
                            {
                                ilGen.EmitWriteLine($"RePacker - Pack: Object of type {field.FieldType.Name} is not supported");
                                ilGen.Emit(OpCodes.Pop);
                                ilGen.Emit(OpCodes.Pop);
                            }
                            break;
                        case TypeCode.String:
                            var encodeString = typeof(Serializer).GetMethod(nameof(Serializer.EncodeString));
                            var encodeStringParams = new Type[] { typeof(string) };
                            ilGen.EmitCall(OpCodes.Call, encodeString, Type.EmptyTypes);
                            // ilGen.Emit(OpCodes.Pop);
                            break;

                        // UNMANAGED DATA
                        case TypeCode.Boolean:
                            parameters[0] = typeof(bool);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Char:
                            parameters[0] = typeof(char);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.SByte:
                            parameters[0] = typeof(sbyte);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Byte:
                            parameters[0] = typeof(byte);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int16:
                            parameters[0] = typeof(short);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt16:
                            parameters[0] = typeof(ushort);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int32:
                            parameters[0] = typeof(int);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt32:
                            parameters[0] = typeof(uint);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int64:
                            parameters[0] = typeof(long);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt64:
                            parameters[0] = typeof(ulong);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Single:
                            parameters[0] = typeof(float);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Double:
                            parameters[0] = typeof(double);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Decimal:
                            parameters[0] = typeof(decimal);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                    }
                }
                goto Finished;

            // ilGen.EmitWriteLine($"{info.Type.Name} is Serialized");
            Finished:
                ilGen.Emit(OpCodes.Ret);
            }

            return () =>
            {
                MethodInfo asGlobal = moduleBuilder.GetMethod(serBuilder.Name);
                return asGlobal;
                // return asGlobal.CreateDelegate(CreateFunc(typeof(Buffer), typeParams));
            };
        }

        public static Func<Delegate> CreateTestMethod(TypeCache.Info info)
        {
            string name = $"{info.Type.Name}_Test";

            var serBuilder = moduleBuilder.DefineGlobalMethod(
                name,
                MethodAttributes.Public | MethodAttributes.Static,
                null,
                null
            );
            serBuilder.SetImplementationFlags(MethodImplAttributes.Managed);

            var ilGen = serBuilder.GetILGenerator();
            {
                ilGen.EmitWriteLine($"{info.Type.Name}: Hello World");
                ilGen.Emit(OpCodes.Ret);
            }

            return () =>
            {
                var del = Expression.GetActionType(new Type[0] { });
                return moduleBuilder.GetMethod(name).CreateDelegate(del);
            };
        }

        public static Func<MethodInfo> CreateDataLogger(TypeCache.Info info)
        {
            Type[] typeParams = new Type[] { info.Type };

            string name = $"{info.Type.Name}_Logger";

            var loggerBuilder = moduleBuilder.DefineGlobalMethod(
                name,
                MethodAttributes.Public | MethodAttributes.Static,
                null,
                typeParams
            );
            loggerBuilder.SetImplementationFlags(MethodImplAttributes.Managed);

            MethodInfo stringAdder = typeof(SerializerBuilder).GetMethod(
                nameof(SerializerBuilder.AddStrings));

            MethodInfo fieldString = typeof(SerializerBuilder).GetMethod(
                nameof(SerializerBuilder.BuildFieldString));

            var ilGen = loggerBuilder.GetILGenerator();
            {
                var lb = ilGen.DeclareLocal(typeof(string));

                // Separator
                ilGen.Emit(OpCodes.Ldstr, ", ");

                // Gather all fields info in string array
                ilGen.Emit(OpCodes.Ldc_I4, info.SerializedFields.Length);
                ilGen.Emit(OpCodes.Newarr, typeof(string));
                ilGen.Emit(OpCodes.Dup);
                for (int i = 0; i < info.SerializedFields.Length; i++)
                {
                    var field = info.SerializedFields[i];

                    // Set array element
                    ilGen.Emit(OpCodes.Ldc_I4, i);
                    ilGen.Emit(OpCodes.Ldstr, field.Name);

                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Ldfld, field);

                    // Box our data
                    if (field.FieldType.IsValueType)
                    {
                        ilGen.Emit(OpCodes.Box, field.FieldType);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Castclass, typeof(object));
                    }

                    // Create string of Field Name + Field Value
                    ilGen.EmitCall(OpCodes.Call, fieldString, Type.EmptyTypes);

                    ilGen.Emit(OpCodes.Stelem_Ref);

                    if (i != info.SerializedFields.Length - 1)
                        ilGen.Emit(OpCodes.Dup);
                }

                // Create string from data
                ilGen.EmitCall(OpCodes.Call, stringAdder, Type.EmptyTypes);
                ilGen.Emit(OpCodes.Stloc_0);

                // Separator
                ilGen.Emit(OpCodes.Ldstr, "");

                // Create Array to hold final string
                ilGen.Emit(OpCodes.Ldc_I4_4);
                ilGen.Emit(OpCodes.Newarr, typeof(string));
                ilGen.Emit(OpCodes.Dup);

                ilGen.Emit(OpCodes.Ldc_I4_0);
                ilGen.Emit(OpCodes.Ldstr, $"{info.Type.Name} {{ ");
                ilGen.Emit(OpCodes.Stelem_Ref);
                ilGen.Emit(OpCodes.Dup);

                ilGen.Emit(OpCodes.Ldc_I4_1);
                ilGen.Emit(OpCodes.Ldloc_0);
                ilGen.Emit(OpCodes.Stelem_Ref);
                ilGen.Emit(OpCodes.Dup);

                ilGen.Emit(OpCodes.Ldc_I4_2);
                ilGen.Emit(OpCodes.Ldstr, $" }}");
                ilGen.Emit(OpCodes.Stelem_Ref);

                ilGen.EmitCall(OpCodes.Call, stringAdder, Type.EmptyTypes);
                ilGen.Emit(OpCodes.Stloc_0);

                ilGen.EmitWriteLine(lb);

                ilGen.Emit(OpCodes.Ret);
            }

            return () =>
            {
                return moduleBuilder.GetMethod(loggerBuilder.Name);
            };
        }

        static object[] genericParams = new object[1];
        public static string BuildFieldString(string name, object data)
        {
            string text = "";

            if (data == null)
            {
                return $"{name}: Unsupported";
            }

            if (TypeCache.TryGetTypeInfo(data.GetType(), out var _))
            {
                using (var reader = new StringWriter())
                {
                    var defaultOut = Console.Out;
                    Console.SetOut(reader);

                    MethodInfo toGeneric = typeof(TypeCache).GetMethod(nameof(TypeCache.LogData)).MakeGenericMethod(data.GetType());
                    genericParams[0] = data;
                    toGeneric.Invoke(null, genericParams);

                    text = name + ": " + reader.ToString().TrimEnd();

                    Console.SetOut(defaultOut);
                }
            }
            else
            {
                text = $"{name}: {data.ToString()}";
            }

            return text;
        }

        public static string AddStrings(string sep, params string[] data)
        {
            string result = "";
            // result += s1 + s2 + s3;
            for (int i = 0; i < data.Length; i++)
            {
                result += data[i];
                if (i != data.Length - 1)
                {
                    result += sep;
                }
            }
            return result;
        }
    }
}


using System;
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

        public static Delegate CreateGenerator(TypeCache.Info info)
        {
            Type[] typeParams = info.SerializedFields.Select(e => e.FieldType).ToArray();

            var generator = new DynamicMethod(
                $"{info.Type.Name}_Generator",
                info.Type,
                typeParams
            );

            for (int i = 0; i < info.SerializedFields.Length; i++)
            {
                generator.DefineParameter(0, ParameterAttributes.In, info.SerializedFields[i].Name);
            }

            var ilGen = generator.GetILGenerator();
            {
                ilGen.DeclareLocal(info.Type);

                ilGen.Emit(OpCodes.Ldloca_S, 0);
                ilGen.Emit(OpCodes.Initobj, info.Type);

                for (int i = 0; i < info.SerializedFields.Length; i++)
                {
                    ilGen.Emit(OpCodes.Ldloca_S, 0);
                    ilGen.Emit(OpCodes.Ldarg, i);
                    ilGen.Emit(OpCodes.Stfld, info.SerializedFields[i]);
                }

                ilGen.Emit(OpCodes.Ldloc_0);
                ilGen.Emit(OpCodes.Ret);
            }

            return generator.CreateDelegate(CreateFunc(info.Type, typeParams));
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

            var parameters = new Type[1];

            MethodInfo bufferPopGeneric = null;
            var ilGen = deserBuilder.GetILGenerator();
            {
                ilGen.DeclareLocal(info.Type);

                // ilGen.EmitWriteLine($"Deserializing {info.Type.Name}");

                for (int i = 0; i < info.SerializedFields.Length; i++)
                {
                    var field = info.SerializedFields[i];

                    // BoxedBuffer -> buffer -> caller
                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);
                    // Byte -> output field
                    ilGen.Emit(OpCodes.Ldloca_S, 0);
                    ilGen.Emit(OpCodes.Ldflda, field);

                    switch (Type.GetTypeCode(field.FieldType))
                    {
                        case TypeCode.Boolean:
                            parameters[0] = typeof(bool);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Char:
                            parameters[0] = typeof(char);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.SByte:
                            parameters[0] = typeof(sbyte);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Byte:
                            parameters[0] = typeof(byte);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int16:
                            parameters[0] = typeof(short);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt16:
                            parameters[0] = typeof(ushort);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int32:
                            parameters[0] = typeof(int);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt32:
                            parameters[0] = typeof(uint);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int64:
                            parameters[0] = typeof(long);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt64:
                            parameters[0] = typeof(ulong);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Single:
                            parameters[0] = typeof(float);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Double:
                            parameters[0] = typeof(double);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Decimal:
                            parameters[0] = typeof(decimal);
                            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                    }
                }

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

            var parameters = new Type[1];
            MethodInfo bufferPushGeneric = null;

            var ilGen = serBuilder.GetILGenerator();
            {
                // ilGen.EmitWriteLine($"Serializing {info.Type.Name}");

                for (int i = 0; i < info.SerializedFields.Length; i++)
                {
                    var field = info.SerializedFields[i];

                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);

                    ilGen.Emit(OpCodes.Ldarga_S, 1);
                    ilGen.Emit(OpCodes.Ldflda, field);

                    switch (Type.GetTypeCode(field.FieldType))
                    {
                        // MANAGED DATA
                        case TypeCode.Empty:
                            break;
                        case TypeCode.Object:
                            break;
                        case TypeCode.DateTime:
                            break;
                        case TypeCode.String:
                            break;

                        // UNMANAGED DATA
                        case TypeCode.Boolean:
                            parameters[0] = typeof(bool);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Char:
                            parameters[0] = typeof(char);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.SByte:
                            parameters[0] = typeof(sbyte);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Byte:
                            parameters[0] = typeof(byte);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int16:
                            parameters[0] = typeof(short);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt16:
                            parameters[0] = typeof(ushort);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int32:
                            parameters[0] = typeof(int);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt32:
                            parameters[0] = typeof(uint);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Int64:
                            parameters[0] = typeof(long);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.UInt64:
                            parameters[0] = typeof(ulong);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Single:
                            parameters[0] = typeof(float);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Double:
                            parameters[0] = typeof(double);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                        case TypeCode.Decimal:
                            parameters[0] = typeof(decimal);
                            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, parameters);
                            ilGen.Emit(OpCodes.Pop);
                            break;
                    }
                }

                // ilGen.EmitWriteLine($"{info.Type.Name} is Serialized");
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

        static Type CreateFunc(Type returnType, Type[] typeParams)
        {
            Type[] funcParams = new Type[typeParams.Length + 1];
            for (int i = 0; i < typeParams.Length; i++)
            {
                funcParams[i] = typeParams[i];
            }
            funcParams[typeParams.Length] = returnType;

            return Expression.GetFuncType(funcParams);
        }

        static Type CreateAction(Type[] typeParams)
        {
            return Expression.GetActionType(typeParams);
        }
    }
}
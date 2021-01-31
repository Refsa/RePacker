

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

                ilGen.EmitWriteLine($"Deserializing {info.Type.Name}");

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

        public static Func<Delegate> CreateSerializer(TypeCache.Info info)
        {
            // Type[] typeParams = info.SerializedFields.Select(e => e.FieldType).ToArray();
            Type[] typeParams = new Type[] { typeof(Refsa.RePacker.Buffers.Buffer), info.Type };

            var serBuilder = moduleBuilder.DefineGlobalMethod(
                $"{info.Type.Name}_Serialize",
                MethodAttributes.Public | MethodAttributes.Static |
                MethodAttributes.HideBySig,
                typeof(Buffer),
                typeParams
            );
            serBuilder.SetImplementationFlags(MethodImplAttributes.Managed);

            serBuilder.DefineParameter(0, ParameterAttributes.In, "buffer");
            serBuilder.DefineParameter(1, ParameterAttributes.In, info.Type.Name.ToLower());

            MethodInfo bufferPush = typeof(Buffer)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.Name == "Push" && mi.GetParameters().Length == 1).First();

            MethodInfo bufferPushGeneric = null;
            var ilGen = serBuilder.GetILGenerator();
            {
                ilGen.DeclareLocal(typeof(Buffer));

                ilGen.EmitWriteLine($"Serializing {info.Type.Name}");

                for (int i = 0; i < info.SerializedFields.Length; i++)
                {
                    var field = info.SerializedFields[i];

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
                            bufferPushGeneric = bufferPush.MakeGenericMethod(new Type[] { typeof(bool) });
                            break;
                        case TypeCode.Char:
                            bufferPushGeneric = bufferPush.MakeGenericMethod(new Type[] { typeof(char) });
                            break;
                        case TypeCode.SByte:
                            break;
                        case TypeCode.Byte:
                            var byteParams = new Type[] { typeof(byte) };
                            bufferPushGeneric = bufferPush.MakeGenericMethod(byteParams);

                            ilGen.Emit(OpCodes.Ldarga_S, 0);
                            ilGen.Emit(OpCodes.Ldarga_S, 1);
                            ilGen.Emit(OpCodes.Ldflda, field);
                            ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, byteParams);
                            ilGen.Emit(OpCodes.Stloc_0);
                            break;
                        case TypeCode.Int16:
                            break;
                        case TypeCode.UInt16:
                            break;
                        case TypeCode.Int32:
                            bufferPushGeneric = bufferPush.MakeGenericMethod(new Type[] { typeof(int) });
                            break;
                        case TypeCode.UInt32:
                            break;
                        case TypeCode.Int64:
                            break;
                        case TypeCode.UInt64:
                            break;
                        case TypeCode.Single:
                            bufferPushGeneric = bufferPush.MakeGenericMethod(new Type[] { typeof(float) });
                            break;
                        case TypeCode.Double:
                            break;
                        case TypeCode.Decimal:
                            break;
                    }
                }

                ilGen.Emit(OpCodes.Ldloc_0);
                ilGen.Emit(OpCodes.Ret);
            }


            return () =>
            {
                MethodInfo asGlobal = moduleBuilder.GetMethod(serBuilder.Name);
                return asGlobal.CreateDelegate(CreateFunc(typeof(Buffer), typeParams));
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
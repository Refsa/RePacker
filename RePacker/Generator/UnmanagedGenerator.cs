using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Generator
{
    public class UnmanagedGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Unmanaged;
        public Type ForType => null;

        MethodInfo bufferPushGeneric = null;
        MethodInfo bufferPush = typeof(Buffer)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.Name == "Push" && mi.GetParameters().Length == 1).First();

        MethodInfo bufferPop = typeof(Buffer)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.Name == "Pop" && mi.GetParameters().Length == 1).First();
        MethodInfo bufferPopGeneric = null;

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            Type[] parameters = new Type[1];

            switch (Type.GetTypeCode(fieldInfo.FieldType))
            {
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

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            Type[] parameters = new Type[1];

            switch (Type.GetTypeCode(fieldInfo.FieldType))
            {
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
    }
}
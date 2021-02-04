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
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopSByte), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.Byte:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopByte), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.Int16:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopShort), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.UInt16:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopUShort), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.Int32:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopInt), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.UInt32:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopUInt), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.Int64:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopLong), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.UInt64:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopULong), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
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
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushSByte), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Byte:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushByte), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Int16:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushShort), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.UInt16:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushShort), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Int32:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushInt), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.UInt32:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushUInt), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Int64:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushLong), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.UInt64:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushULong), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
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
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
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
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopBool), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.Char:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopChar), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
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
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopFloat), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.Double:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopDouble), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.Decimal:
                    bufferPopGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopDecimal), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPopGeneric, Type.EmptyTypes);
                    break;
            }
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            Type[] parameters = new Type[1];

            switch (Type.GetTypeCode(fieldInfo.FieldType))
            {
                case TypeCode.Boolean:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushBool), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.Char:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PopBool), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.SByte:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushSByte), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Byte:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushByte), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Int16:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushShort), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.UInt16:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushShort), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Int32:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushInt), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.UInt32:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushUInt), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Int64:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushLong), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.UInt64:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushULong), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Single:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushFloat), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.Double:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushDouble), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                    break;
                case TypeCode.Decimal:
                    bufferPushGeneric = typeof(Buffer).GetMethod(nameof(Buffer.PushDecimal), BindingFlags.Public | BindingFlags.Instance);
                    ilGen.EmitCall(OpCodes.Call, bufferPushGeneric, Type.EmptyTypes);
                    break;
            }
        }
    }
}
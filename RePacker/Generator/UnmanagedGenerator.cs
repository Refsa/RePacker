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
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopBool), typeof(bool));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Char:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopChar), typeof(char));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.SByte:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopSByte), typeof(sbyte));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Byte:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopByte), typeof(byte));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Int16:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopShort), typeof(short));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.UInt16:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopUShort), typeof(ushort));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Int32:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopInt), typeof(int));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.UInt32:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopUInt), typeof(uint));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Int64:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopLong), typeof(long));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.UInt64:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopULong), typeof(ulong));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Single:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopFloat), typeof(float));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Double:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopDouble), typeof(double));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Decimal:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopDecimal), typeof(decimal));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
            }
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            Type[] parameters = new Type[1];

            switch (Type.GetTypeCode(fieldInfo.FieldType))
            {
                case TypeCode.Boolean:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushBool), typeof(bool));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Char:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PopBool), typeof(char));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.SByte:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushSByte), typeof(sbyte));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Byte:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushByte), typeof(byte));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Int16:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushShort), typeof(short));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.UInt16:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushShort), typeof(ushort));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Int32:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushInt), typeof(int));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.UInt32:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushUInt), typeof(uint));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Int64:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushLong), typeof(long));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.UInt64:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushULong), typeof(ulong));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Single:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushFloat), typeof(float));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Double:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushDouble), typeof(double));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Decimal:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushDecimal), typeof(decimal));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
            }
        }

        MethodInfo GetPopMethod(string name, Type type)
        {
            return typeof(Buffer)
                .GetMethod(
                    name,
                    BindingFlags.Public | BindingFlags.Instance
                );
        }

        MethodInfo GetPushMethod(string name, Type type)
        {
            return typeof(Buffer)
                .GetMethod(
                    name,
                    BindingFlags.Public | BindingFlags.Instance
                );
        }
    }
}
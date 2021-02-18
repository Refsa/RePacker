using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
{
    internal class UnmanagedGenerator : IGenerator
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

        MethodInfo bufferPack = typeof(Buffer)
            .GetMethod(nameof(Buffer.Pack));
        MethodInfo bufferUnpack = typeof(Buffer)
            .GetMethod(nameof(Buffer.Unpack));

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            // bufferPopGeneric = bufferUnpack.MakeGenericMethod(fieldInfo.FieldType);
            // ilGen.Emit(OpCodes.Call, bufferPopGeneric);

            switch (Type.GetTypeCode(fieldInfo.FieldType))
            {
                case TypeCode.Boolean:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopBool));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Char:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopChar));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.SByte:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopSByte));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Byte:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopByte));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Int16:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopShort));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.UInt16:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopUShort));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Int32:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopInt));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.UInt32:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopUInt));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Int64:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopLong));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.UInt64:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopULong));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Single:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopFloat));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Double:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopDouble));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
                case TypeCode.Decimal:
                    bufferPopGeneric = GetPopMethod(nameof(Buffer.PopDecimal));
                    ilGen.Emit(OpCodes.Call, bufferPopGeneric);
                    break;
            }
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            // bufferPushGeneric = bufferPack.MakeGenericMethod(fieldInfo.FieldType);
            // ilGen.Emit(OpCodes.Call, bufferPushGeneric);

            switch (Type.GetTypeCode(fieldInfo.FieldType))
            {
                case TypeCode.Boolean:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushBool));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Char:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PopBool));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.SByte:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushSByte));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Byte:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushByte));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Int16:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushShort));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.UInt16:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushShort));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Int32:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushInt));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.UInt32:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushUInt));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Int64:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushLong));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.UInt64:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushULong));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Single:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushFloat));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Double:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushDouble));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
                case TypeCode.Decimal:
                    bufferPushGeneric = GetPushMethod(nameof(Buffer.PushDecimal));
                    ilGen.Emit(OpCodes.Call, bufferPushGeneric);
                    break;
            }
        }

        MethodInfo GetPopMethod(string name)
        {
            return typeof(Buffer)
                .GetMethod(
                    name,
                    BindingFlags.Public | BindingFlags.Instance
                );
        }

        MethodInfo GetPushMethod(string name)
        {
            return typeof(Buffer)
                .GetMethod(
                    name,
                    BindingFlags.Public | BindingFlags.Instance
                );
        }
    }
}
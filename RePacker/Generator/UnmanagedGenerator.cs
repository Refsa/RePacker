using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Buffers;
using RePacker.Utils;
using Buffer = RePacker.Buffers.Buffer;

namespace RePacker.Builder
{
    internal class UnmanagedGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Unmanaged;
        public Type ForType => null;

        MethodInfo bufferPushGeneric = null;
        MethodInfo bufferPopGeneric = null;

        MethodInfo bufferPack = typeof(Buffer).GetMethod(nameof(Buffer.Pack));
        MethodInfo bufferUnpack = typeof(Buffer).GetMethod(nameof(Buffer.Unpack));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsUnpack(fieldInfo);
            
            bufferPopGeneric = bufferUnpack.MakeGenericMethod(fieldInfo.FieldType);
            ilGen.Emit(OpCodes.Call, bufferPopGeneric);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsPack(fieldInfo);

            bufferPushGeneric = bufferPack.MakeGenericMethod(fieldInfo.FieldType);
            ilGen.Emit(OpCodes.Call, bufferPushGeneric);
        }
    }
}
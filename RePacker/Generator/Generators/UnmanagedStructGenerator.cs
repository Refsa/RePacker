using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Buffers;
using RePacker.Unsafe;
using RePacker.Utils;
using Buffer = RePacker.Buffers.Buffer;

namespace RePacker.Builder
{
    internal class UnmanagedStructGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Struct;
        public Type ForType => null;

        MethodInfo bufferPack = typeof(BufferUtils).GetMethod(nameof(BufferUtils.Pack));
        MethodInfo bufferUnpack = typeof(BufferUtils).GetMethod(nameof(BufferUtils.Unpack));
        MethodInfo bufferPopGeneric = null;
        MethodInfo bufferPushGeneric = null;

        MethodInfo unsafeSizeOfMethod = typeof(UnsafeUtils).GetMethod(nameof(UnsafeUtils.SizeOf));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            Type[] parameters = new Type[1];

            ilGen.Emit(OpCodes.Ldarg_0);

            ilGen.Emit(OpCodes.Ldloca_S, 0);
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);

            parameters[0] = fieldInfo.FieldType;
            bufferPopGeneric = bufferUnpack.MakeGenericMethod(parameters);
            ilGen.Emit(OpCodes.Call, bufferPopGeneric);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            Type[] parameters = new Type[1];

            ilGen.Emit(OpCodes.Ldarg_0);

            ilGen.Emit(OpCodes.Ldarga_S, 1);
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);

            parameters[0] = fieldInfo.FieldType;
            bufferPushGeneric = bufferPack.MakeGenericMethod(parameters);
            ilGen.Emit(OpCodes.Call, bufferPushGeneric);
        }

        public void GenerateGetSizer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var genMethod = unsafeSizeOfMethod.MakeGenericMethod(fieldInfo.FieldType);

            ilGen.Emit(OpCodes.Ldarga_S, 0);
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);
            ilGen.Emit(OpCodes.Call, genMethod);
        }
    }
}
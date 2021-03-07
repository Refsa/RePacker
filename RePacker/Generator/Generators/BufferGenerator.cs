using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Buffers;
using RePacker.Utils;

using ReBuffer = RePacker.Buffers.ReBuffer;

namespace RePacker.Builder
{
    internal class BufferGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(ReBuffer);

        static readonly MethodInfo packBufferMethod = typeof(BufferUtils).GetMethod(nameof(BufferUtils.PackBuffer));
        static readonly MethodInfo unpackBufferMethod = typeof(BufferUtils).GetMethod(nameof(BufferUtils.UnpackBuffer));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsUnpack(fieldInfo);

            ilGen.Emit(OpCodes.Call, unpackBufferMethod);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsPack(fieldInfo);

            ilGen.Emit(OpCodes.Call, packBufferMethod);
        }

        public void GenerateGetSizer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var lengthMethod = typeof(ReBuffer).GetMethod(nameof(ReBuffer.Length));

            if (fieldInfo.FieldType.IsValueType)
            {
                ilGen.Emit(OpCodes.Ldarga_S, 0);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldarg_S, 0);
            }
            ilGen.Emit(OpCodes.Ldfld, fieldInfo);

            ilGen.Emit(OpCodes.Call, lengthMethod);
        }
    }
}
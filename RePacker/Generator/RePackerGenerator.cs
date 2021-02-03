using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Generator
{
    public class RePackerGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.RePacker;
        public Type ForType => null;

        MethodInfo deserializeTypeMethod = typeof(TypeCache).GetMethod(nameof(TypeCache.UnpackOut));
        MethodInfo serializeTypeMethod = typeof(TypeCache).GetMethod(nameof(TypeCache.Pack));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var genericSerializer = deserializeTypeMethod.MakeGenericMethod(fieldInfo.FieldType);
            ilGen.EmitCall(OpCodes.Call, genericSerializer, Type.EmptyTypes);
            ilGen.Emit(OpCodes.Pop);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var genericSerializer = serializeTypeMethod.MakeGenericMethod(fieldInfo.FieldType);
            ilGen.EmitCall(OpCodes.Call, genericSerializer, Type.EmptyTypes);
        }
    }
}
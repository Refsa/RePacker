using System;
using System.Reflection;
using System.Reflection.Emit;

namespace RePacker.Builder
{
    internal class RePackerGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.RePacker;
        public Type ForType => null;

        MethodInfo deserializeTypeMethod = typeof(TypeCache).GetMethod(nameof(TypeCache.UnpackOut));
        MethodInfo serializeTypeMethod = typeof(TypeCache).GetMethod(nameof(TypeCache.Pack));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var genericSerializer = deserializeTypeMethod.MakeGenericMethod(fieldInfo.FieldType);
            ilGen.Emit(OpCodes.Call, genericSerializer);
            ilGen.Emit(OpCodes.Pop);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var genericSerializer = serializeTypeMethod.MakeGenericMethod(fieldInfo.FieldType);
            ilGen.Emit(OpCodes.Call, genericSerializer);
        }
    }
}
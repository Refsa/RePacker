using System;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Utils;

namespace RePacker.Builder
{
    internal class RePackerGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.RePacker;
        public Type ForType => null;

        static readonly MethodInfo deserializeTypeMethod = typeof(TypeCache).GetMethod(nameof(TypeCache.UnpackOut));
        static readonly MethodInfo serializeTypeMethod = typeof(TypeCache).GetMethod(nameof(TypeCache.Pack));
        static readonly MethodInfo getSizeMethod = typeof(TypeCache).GetMethod(nameof(TypeCache.GetSize));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsUnpack(fieldInfo);

            var genericSerializer = deserializeTypeMethod.MakeGenericMethod(fieldInfo.FieldType);
            ilGen.Emit(OpCodes.Call, genericSerializer);
            ilGen.Emit(OpCodes.Pop);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsPack(fieldInfo);

            var genericSerializer = serializeTypeMethod.MakeGenericMethod(fieldInfo.FieldType);
            ilGen.Emit(OpCodes.Call, genericSerializer);
        }

        public void GenerateGetSizer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var resolver = typeof(TypeResolver<,>).MakeGenericType(typeof(IPacker<>).MakeGenericType(fieldInfo.FieldType), fieldInfo.FieldType);
            var resolverGetPacker = resolver.GetProperty("Packer").GetMethod;
            var sizeOfMethod = typeof(IPacker<>).MakeGenericType(fieldInfo.FieldType).GetMethod("SizeOf");

            ilGen.Emit(OpCodes.Call, resolverGetPacker);

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);

            ilGen.Emit(OpCodes.Call, sizeOfMethod);
        }
    }
}
using System;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Utils;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class NullableGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(Nullable<>);

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsUnpack(fieldInfo);

            var genArgs = fieldInfo.FieldType.GetGenericArguments();
            var unpackMethod = typeof(BufferExt)
                .GetMethod(nameof(BufferExt.UnpackNullable))
                .MakeGenericMethod(genArgs);

            ilGen.Emit(OpCodes.Call, unpackMethod);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsPack(fieldInfo);

            var genArgs = fieldInfo.FieldType.GetGenericArguments();
            var packMethod = typeof(BufferExt)
                .GetMethod(nameof(BufferExt.PackNullable))
                .MakeGenericMethod(genArgs);

            ilGen.Emit(OpCodes.Call, packMethod);
        }

        public void GenerateGetSizer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Ldc_I4, 0);
        }
    }
}
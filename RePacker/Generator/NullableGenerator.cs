using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Utils;

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
            var unpackMethod = typeof(BoxedBufferExt)
                .GetMethod(nameof(BoxedBufferExt.UnpackNullable))
                .MakeGenericMethod(genArgs);

            ilGen.Emit(OpCodes.Call, unpackMethod);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsPack(fieldInfo);

            var genArgs = fieldInfo.FieldType.GetGenericArguments();
            var packMethod = typeof(BoxedBufferExt)
                .GetMethod(nameof(BoxedBufferExt.PackNullable))
                .MakeGenericMethod(genArgs);

            ilGen.Emit(OpCodes.Call, packMethod);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Refsa.RePacker.Builder
{
    internal class KeyValuePairGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(KeyValuePair<,>);

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var genArgs = fieldInfo.FieldType.GetGenericArguments();

            var unpackMethod = typeof(BoxedBufferExt)
                .GetMethod(nameof(BoxedBufferExt.UnpackKeyValuePair))
                .MakeGenericMethod(genArgs);

            ilGen.Emit(OpCodes.Call, unpackMethod);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var genArgs = fieldInfo.FieldType.GetGenericArguments();
            var packMethod = typeof(BoxedBufferExt)
                .GetMethod(nameof(BoxedBufferExt.PackKeyValuePair))
                .MakeGenericMethod(genArgs);

            ilGen.Emit(OpCodes.Call, packMethod);
        }
    }
}
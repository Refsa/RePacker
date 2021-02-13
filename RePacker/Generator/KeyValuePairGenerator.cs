using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
{
    public class KeyValuePairGenerator : IGenerator
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
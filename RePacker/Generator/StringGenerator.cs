using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
{
    public class StringGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.String;
        public Type ForType => typeof(string);

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var stringDecParams = new Type[] { typeof(Buffer).MakeByRefType(), typeof(string).MakeByRefType() };
            var decodeString = typeof(BufferExt).GetMethod(nameof(BufferExt.UnpackString), stringDecParams);
            ilGen.EmitCall(OpCodes.Call, decodeString, Type.EmptyTypes);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var encodeStringParams = new Type[] { typeof(Buffer).MakeByRefType(), typeof(string).MakeByRefType() };
            var encodeString = typeof(BufferExt).GetMethod(nameof(BufferExt.PackString), encodeStringParams);
            ilGen.EmitCall(OpCodes.Call, encodeString, Type.EmptyTypes);
            // ilGen.Emit(OpCodes.Pop);
        }
    }
}
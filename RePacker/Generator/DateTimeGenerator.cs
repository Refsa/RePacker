using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
{
    public class DateTimeGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.DateTime;
        public Type ForType => typeof(string);

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var dateTimeDecParams = new Type[] { typeof(Buffer).MakeByRefType(), typeof(DateTime).MakeByRefType() };
            var decodeDateTime = typeof(BufferExt).GetMethod(nameof(BufferExt.UnpackDateTime), dateTimeDecParams);
            ilGen.EmitCall(OpCodes.Call, decodeDateTime, Type.EmptyTypes);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var dateTimeDecParams = new Type[] { typeof(Buffer).MakeByRefType(), typeof(DateTime).MakeByRefType() };
            var encodeDateTime = typeof(BufferExt).GetMethod(nameof(BufferExt.PackDateTime), dateTimeDecParams);
            ilGen.EmitCall(OpCodes.Call, encodeDateTime, Type.EmptyTypes);
            // ilGen.Emit(OpCodes.Pop);
        }
    }
}
using System;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Utils;
using RePacker.Buffers;
using Buffer = RePacker.Buffers.Buffer;

namespace RePacker.Builder
{
    internal class DateTimeGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.DateTime;
        public Type ForType => typeof(string);

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsUnpack(fieldInfo);
            
            var dateTimeDecParams = new Type[] { typeof(Buffer), typeof(DateTime).MakeByRefType() };
            var decodeDateTime = typeof(BufferExt).GetMethod(nameof(BufferExt.UnpackDateTime), dateTimeDecParams);
            ilGen.Emit(OpCodes.Call, decodeDateTime);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsPack(fieldInfo);

            var dateTimeDecParams = new Type[] { typeof(Buffer), typeof(DateTime).MakeByRefType() };
            var encodeDateTime = typeof(BufferExt).GetMethod(nameof(BufferExt.PackDateTime), dateTimeDecParams);
            ilGen.Emit(OpCodes.Call, encodeDateTime);
            // ilGen.Emit(OpCodes.Pop);
        }
    }
}
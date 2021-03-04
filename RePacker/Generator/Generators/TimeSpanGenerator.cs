using System;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Utils;
using RePacker.Buffers;
using Buffer = RePacker.Buffers.Buffer;

namespace RePacker.Builder
{
    internal class TimeSpanGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.TimeSpan;
        public Type ForType => typeof(string);

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsUnpack(fieldInfo);
            
            var dateTimeDecParams = new Type[] { typeof(Buffer), typeof(TimeSpan).MakeByRefType() };
            var decodeTimeSpan = typeof(BufferExt).GetMethod(nameof(BufferExt.UnpackTimeSpan), dateTimeDecParams);
            ilGen.Emit(OpCodes.Call, decodeTimeSpan);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsPack(fieldInfo);

            var dateTimeDecParams = new Type[] { typeof(Buffer), typeof(TimeSpan).MakeByRefType() };
            var encodeTimeSpan = typeof(BufferExt).GetMethod(nameof(BufferExt.PackTimeSpan), dateTimeDecParams);
            ilGen.Emit(OpCodes.Call, encodeTimeSpan);
            // ilGen.Emit(OpCodes.Pop);
        }
    }
}
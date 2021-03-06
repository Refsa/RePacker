using System;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Utils;
using RePacker.Buffers;
using Buffer = RePacker.Buffers.Buffer;
using RePacker.Unsafe;

namespace RePacker.Builder
{
    internal class StringGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.String;
        public Type ForType => typeof(string);

        static readonly MethodInfo getUtf8Encoder;
        static readonly MethodInfo getStringLengthMethod;
        static readonly MethodInfo stringIsNullOrEmptyMethod;

        static StringGenerator()
        {
            getUtf8Encoder = typeof(System.Text.Encoding)
                .GetProperty(nameof(System.Text.Encoding.UTF8), BindingFlags.Static | BindingFlags.Public)
                .GetMethod;

            getStringLengthMethod = typeof(System.Text.Encoding)
                .GetMethod(nameof(System.Text.Encoding.GetByteCount), new Type[] { typeof(string) });

            stringIsNullOrEmptyMethod = typeof(string).GetMethod(nameof(string.IsNullOrEmpty));
        }

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsUnpack(fieldInfo);

            var stringDecParams = new Type[] { typeof(Buffer), typeof(string).MakeByRefType() };
            var decodeString = typeof(BufferExt).GetMethod(nameof(BufferExt.UnpackString), stringDecParams);
            ilGen.Emit(OpCodes.Call, decodeString);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsPack(fieldInfo);

            var encodeStringParams = new Type[] { typeof(Buffer), typeof(string).MakeByRefType() };
            var encodeString = typeof(BufferExt).GetMethod(nameof(BufferExt.PackString), encodeStringParams);
            ilGen.Emit(OpCodes.Call, encodeString);
        }

        public void GenerateGetSizer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var stringSizeOfMethod = typeof(StringHelper).GetMethod(nameof(StringHelper.SizeOf));

            ilGen.Emit(OpCodes.Ldc_I4, sizeof(long));

            if (fieldInfo.FieldType.IsValueType)
            {
                ilGen.Emit(OpCodes.Ldarga_S, 0);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldarg_S, 0);
            }
            ilGen.Emit(OpCodes.Ldfld, fieldInfo);
            ilGen.Emit(OpCodes.Call, stringSizeOfMethod);
            ilGen.Emit(OpCodes.Add);
        }
    }
}
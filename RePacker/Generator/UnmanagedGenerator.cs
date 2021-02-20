using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
{
    internal class UnmanagedGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Unmanaged;
        public Type ForType => null;

        MethodInfo bufferPushGeneric = null;
        MethodInfo bufferPush = typeof(Buffer)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.Name == "Push" && mi.GetParameters().Length == 1).First();

        MethodInfo bufferPop = typeof(Buffer)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.Name == "Pop" && mi.GetParameters().Length == 1).First();
        MethodInfo bufferPopGeneric = null;

        MethodInfo bufferPack = typeof(Buffer)
            .GetMethod(nameof(Buffer.Pack));
        MethodInfo bufferUnpack = typeof(Buffer)
            .GetMethod(nameof(Buffer.Unpack));

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            bufferPopGeneric = bufferUnpack.MakeGenericMethod(fieldInfo.FieldType);
            ilGen.Emit(OpCodes.Call, bufferPopGeneric);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            bufferPushGeneric = bufferPack.MakeGenericMethod(fieldInfo.FieldType);
            ilGen.Emit(OpCodes.Call, bufferPushGeneric);
        }

        MethodInfo GetPopMethod(string name)
        {
            return typeof(Buffer)
                .GetMethod(
                    name,
                    BindingFlags.Public | BindingFlags.Instance
                );
        }

        MethodInfo GetPushMethod(string name)
        {
            return typeof(Buffer)
                .GetMethod(
                    name,
                    BindingFlags.Public | BindingFlags.Instance
                );
        }
    }
}
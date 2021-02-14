using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
{
    internal class UnmanagedStructGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Struct;
        public Type ForType => null;

        MethodInfo bufferPushGeneric = null;
        MethodInfo bufferPush = typeof(Buffer)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.Name == "Push" && mi.GetParameters().Length == 1).First();

        MethodInfo bufferPop = typeof(Buffer)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.Name == "Pop" && mi.GetParameters().Length == 1).First();
        MethodInfo bufferPopGeneric = null;

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            Type[] parameters = new Type[1];

            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);

            ilGen.Emit(OpCodes.Ldloca_S, 0);
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);

            parameters[0] = fieldInfo.FieldType;
            bufferPopGeneric = bufferPop.MakeGenericMethod(parameters);
            ilGen.Emit(OpCodes.Call, bufferPopGeneric);
            ilGen.Emit(OpCodes.Pop);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            Type[] parameters = new Type[1];
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);

            ilGen.Emit(OpCodes.Ldarga_S, 1);
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);

            parameters[0] = fieldInfo.FieldType;
            bufferPushGeneric = bufferPush.MakeGenericMethod(parameters);
            ilGen.Emit(OpCodes.Call, bufferPushGeneric);
            ilGen.Emit(OpCodes.Pop);
        }
    }
}
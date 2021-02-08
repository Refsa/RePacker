using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
{
    public class ValueTupleGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(ITuple);

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var genArgs = fieldInfo.FieldType.GetGenericArguments();

            var meth = typeof(PackerValueTupleExt)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(e => 
                    e.Name.Contains(nameof(PackerValueTupleExt.UnpackValueTuple)) &&
                    e.GetGenericArguments().Length == genArgs.Length
                ).First()
                .MakeGenericMethod(genArgs);

            ilGen.EmitCall(OpCodes.Call, meth, Type.EmptyTypes);
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            var genArgs = fieldInfo.FieldType.GetGenericArguments();

            var meth = typeof(PackerValueTupleExt)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(e => 
                    e.Name.Contains(nameof(PackerValueTupleExt.PackValueTuple)) &&
                    e.GetGenericArguments().Length == genArgs.Length
                ).First()
                .MakeGenericMethod(genArgs);

            ilGen.EmitCall(OpCodes.Call, meth, Type.EmptyTypes);
        }
    }
}
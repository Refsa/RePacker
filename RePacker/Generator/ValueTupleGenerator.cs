using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Refsa.RePacker.Builder
{
    internal class ValueTupleGenerator : IGenerator
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

            ilGen.Emit(OpCodes.Call, meth);
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

            ilGen.Emit(OpCodes.Call, meth);
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using RePacker.Utils;

namespace RePacker.Builder
{
    internal class ValueTupleGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
    
#if NET461
        public Type ForType => typeof(ValueTuple<>);
#else
        public Type ForType => typeof(ITuple);
#endif

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.LoadArgsUnpack(fieldInfo);
            
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
            ilGen.LoadArgsPack(fieldInfo);

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

        public void GenerateGetSizer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Ldc_I4, 0);
        }
    }
}
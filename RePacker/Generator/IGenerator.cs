using System;
using System.Reflection;
using System.Reflection.Emit;

namespace RePacker.Builder
{
    internal enum GeneratorType
    {
        None = 0,
        Unmanaged,
        Struct,
        RePacker,
        Object,
        String,
        DateTime,
    }

    internal interface IGenerator
    {
        GeneratorType GeneratorType { get; }
        Type ForType { get; }

        void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo);
        void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo);
    }
}
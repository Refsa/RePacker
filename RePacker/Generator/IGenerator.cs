
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Refsa.RePacker.Builder
{
    public enum GeneratorType
    {
        None = 0,
        Unmanaged,
        Struct,
        RePacker,
        Object,
        String,
        DateTime,
    }

    public interface IGenerator
    {
        GeneratorType GeneratorType { get; }
        Type ForType { get; }

        void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo);
        void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo);
    }
}
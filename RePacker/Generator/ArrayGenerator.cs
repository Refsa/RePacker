using System;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Buffers;
using RePacker.Utils;
using Buffer = RePacker.Buffers.Buffer;

namespace RePacker.Builder
{
    internal class ArrayGenerator : IGenerator
    {
        public GeneratorType GeneratorType => GeneratorType.Object;
        public Type ForType => typeof(Array);

        FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        MethodInfo serializeBlittableArrayMethod = typeof(BufferExt)
            .GetMethod(nameof(BufferExt.PackBlittableArray));
        MethodInfo serializeArrayMethod = typeof(PackerCollectionsExt)
            .GetMethod(nameof(PackerCollectionsExt.PackArray));

        MethodInfo deserializeBlittableArrayMethod = typeof(BufferExt)
            .GetMethod(nameof(BufferExt.UnpackUnmanagedArrayOut));
        MethodInfo deserializeArrayMethod = typeof(PackerCollectionsExt)
            .GetMethod(nameof(PackerCollectionsExt.UnpackArray));

        public void GenerateDeserializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            var elementType = fieldInfo.FieldType.GetElementType();

            int rank = fieldInfo.FieldType.GetArrayRank();

            if (rank == 1)
            {
                if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);

                    ilGen.Emit(OpCodes.Ldloca_S, 0);
                    ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                    var arrayUnpacker = deserializeArrayMethod.MakeGenericMethod(elementType);
                    ilGen.Emit(OpCodes.Call, arrayUnpacker);
                }
                else if (elementType.IsValueType || elementType.IsUnmanagedStruct())
                {
                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);

                    ilGen.Emit(OpCodes.Ldloca_S, 0);
                    ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                    var arrayUnpacker = deserializeBlittableArrayMethod.MakeGenericMethod(elementType);
                    ilGen.Emit(OpCodes.Call, arrayUnpacker);
                }
                else
                {
                    ilGen.EmitLog($"RePacker - Unpack: Array of type {fieldInfo.FieldType.Name} is not supported");
                }
            }
            else
            {
                MethodInfo unpackMethod = null;
                switch (rank)
                {
                    case 2:
                        unpackMethod = typeof(PackerCollectionsExt)
                            .GetMethod(nameof(PackerCollectionsExt.UnpackArray2D))
                            .MakeGenericMethod(elementType);
                        break;
                    case 3:
                        unpackMethod = typeof(PackerCollectionsExt)
                            .GetMethod(nameof(PackerCollectionsExt.UnpackArray3D))
                            .MakeGenericMethod(elementType);
                        break;
                    case 4:
                        unpackMethod = typeof(PackerCollectionsExt)
                            .GetMethod(nameof(PackerCollectionsExt.UnpackArray4D))
                            .MakeGenericMethod(elementType);
                        break;
                }

                if (unpackMethod == null)
                {
                    ilGen.EmitLog($"RePacker - Unpack: Array of rank {rank} is not supported");
                    return;
                }

                if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);

                    ilGen.Emit(OpCodes.Ldloca_S, 0);
                    ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                    ilGen.Emit(OpCodes.Call, unpackMethod);
                }
                else
                {
                    ilGen.EmitLog($"RePacker - Unpack: Array of type {fieldInfo.FieldType.Name} is not supported");
                }
            }
        }

        public void GenerateSerializer(ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Pop);
            ilGen.Emit(OpCodes.Pop);

            var elementType = fieldInfo.FieldType.GetElementType();

            int rank = fieldInfo.FieldType.GetArrayRank();

            if (rank == 1)
            {
                if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Ldarga_S, 1);
                    ilGen.Emit(OpCodes.Ldfld, fieldInfo);

                    var arraySerializer = serializeArrayMethod.MakeGenericMethod(elementType);
                    ilGen.Emit(OpCodes.Call, arraySerializer);
                }
                else if (elementType.IsValueType || elementType.IsUnmanagedStruct())
                {
                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);

                    ilGen.Emit(OpCodes.Ldarga_S, 1);
                    ilGen.Emit(OpCodes.Ldfld, fieldInfo);

                    var arraySerializer = serializeBlittableArrayMethod.MakeGenericMethod(elementType);
                    ilGen.Emit(OpCodes.Call, arraySerializer);
                }
                else
                {
                    ilGen.EmitLog($"RePacker - Pack: Array of type {fieldInfo.FieldType.Name} is not supported");
                }
            }
            else
            {
                MethodInfo packMethod = null;
                switch (rank)
                {
                    case 2:
                        packMethod = typeof(PackerCollectionsExt)
                            .GetMethod(nameof(PackerCollectionsExt.PackArray2D))
                            .MakeGenericMethod(elementType);
                        break;
                    case 3:
                        packMethod = typeof(PackerCollectionsExt)
                            .GetMethod(nameof(PackerCollectionsExt.PackArray3D))
                            .MakeGenericMethod(elementType);
                        break;
                    case 4:
                        packMethod = typeof(PackerCollectionsExt)
                            .GetMethod(nameof(PackerCollectionsExt.PackArray4D))
                            .MakeGenericMethod(elementType);
                        break;
                }

                if (packMethod == null)
                {
                    ilGen.EmitLog($"RePacker - Pack: Array of rank {rank} is not supported");
                    return;
                }

                if (TypeCache.TryGetTypeInfo(elementType, out var typeInfo))
                {
                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Ldarga_S, 1);
                    ilGen.Emit(OpCodes.Ldflda, fieldInfo);

                    ilGen.Emit(OpCodes.Call, packMethod);
                }
                else
                {
                    ilGen.EmitLog($"RePacker - Pack: Array of type {fieldInfo.FieldType.Name} is not supported");
                }
            }
        }
    }
}
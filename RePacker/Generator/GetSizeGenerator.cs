using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Builder;
using RePacker.Utils;

namespace RePacker.Generator
{
    internal static class GetSizeGenerator<T>
    {
        static readonly MethodInfo getSizeMethod = typeof(TypeCache).GetMethod(nameof(TypeCache.GetSize));

        public static MethodInfo Create(TypeCache.Info info)
        {
            Type[] parameters = new Type[] { typeof(T) };

            var builder = new DynamicMethod(
                $"{info.Type.Name}_GetSize",
                typeof(int),
                parameters,
                typeof(RePacking),
                true
            );

            MethodInfo genericGetSize = null;

            var ilGen = builder.GetILGenerator();
            {
                if (info.IsUnmanaged && !info.HasCustomSerializer)
                {
                    goto Blittable;
                }
                else
                {
                    goto PerField;
                }

            Blittable:
                ilGen.Emit(OpCodes.Sizeof, info.Type);

                goto Finished;

            PerField:
                ilGen.Emit(OpCodes.Ldc_I4, 0);

                for (int i = 0; i < info.SerializedFields.Length; i++)
                {
                    var field = info.SerializedFields[i];

                    (GeneratorType gt, Type t) = (GeneratorType.None, null);

                    switch (Type.GetTypeCode(field.FieldType))
                    {
                        // MANAGED DATA AND STRUCTS
                        case TypeCode.Empty:
                            break;
                        case TypeCode.DateTime:
                            (gt, t) = (GeneratorType.DateTime, null);
                            break;
                        case TypeCode.Object:
                            if (TypeCache.TryGetTypeInfo(field.FieldType, out var nestedTypeInfo))
                            {
                                (gt, t) = (GeneratorType.RePacker, null);
                            }
                            else if (field.FieldType.IsGenericType)
                            {
                                var genType = field.FieldType.GetGenericTypeDefinition();
                                (gt, t) = (GeneratorType.Object, genType);
                            }
                            else if (field.FieldType.IsStruct() && field.FieldType.IsUnmanagedStruct())
                            {
                                (gt, t) = (GeneratorType.Struct, null);
                            }
                            else if (field.FieldType.IsArray)
                            {
                                (gt, t) = (GeneratorType.Object, typeof(Array));
                            }
                            break;
                        case TypeCode.String:
                            (gt, t) = (GeneratorType.String, typeof(string));
                            break;

                        // UNMANAGED DATA
                        default:
                            (gt, t) = (GeneratorType.Unmanaged, null);
                            break;
                    }

                    if (gt != GeneratorType.None && GeneratorLookup.TryGet(gt, t, out var generator) && generator != null)
                    {
                        generator.GenerateGetSizer(ilGen, field);
                        ilGen.Emit(OpCodes.Add);
                    }
                    else
                    {
                        RePacking.Logger.Warn($"RePacker - GetSizer: Type {field.FieldType.Name} on {info.Type.Name} is not supported");
                    }
                }
                goto Finished;

            Finished:
                ilGen.Emit(OpCodes.Ret);
            }

            return builder;
        }
    }
}
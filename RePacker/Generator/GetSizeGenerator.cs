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
                if (!info.HasCustomSerializer && info.IsUnmanaged)
                {
                    ilGen.Emit(OpCodes.Sizeof, info.Type);
                }
                else if (info.SerializedFields != null)
                {
                    ilGen.Emit(OpCodes.Ldc_I4, 0);

                    foreach (var field in info.SerializedFields)
                    {
                        Type fieldType = field.FieldType;
                        if (fieldType.IsSubclassOf(typeof(Enum)))
                        {
                            fieldType = Enum.GetUnderlyingType(fieldType);
                        }

                        if (TypeCache.TryGetTypeInfo(fieldType, out var typeInfo))
                        {
                            if (!typeInfo.HasCustomSerializer && typeInfo.IsUnmanaged)
                            {
                                ilGen.Emit(OpCodes.Sizeof, fieldType);
                            }
                            else
                            {
                                genericGetSize = getSizeMethod.MakeGenericMethod(fieldType);

                                ilGen.Emit(OpCodes.Ldarg_0);
                                ilGen.Emit(OpCodes.Ldflda, field);
                                ilGen.Emit(OpCodes.Call, genericGetSize);
                            }
                        }
                        else if (fieldType.IsUnmanaged() || fieldType.IsUnmanagedStruct())
                        {
                            ilGen.Emit(OpCodes.Sizeof, fieldType);
                        }
                        else
                        {
                            ilGen.Emit(OpCodes.Ldc_I4, 0);
                        }

                        ilGen.Emit(OpCodes.Add);
                    }
                }

                ilGen.Emit(OpCodes.Ret);
            }

            return builder;
        }
    }
}
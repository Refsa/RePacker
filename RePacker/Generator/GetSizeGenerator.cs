using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Builder;

namespace RePacker.Generator
{
    internal static class GetSizeGenerator<T>
    {
        static readonly MethodInfo getSizeMethod = typeof(TypeCache).GetMethod(nameof(TypeCache.GetSize));
        static readonly MethodInfo sizeOfMethod = typeof(RePacker.Unsafe.UnsafeUtils)
            .GetMethod(nameof(RePacker.Unsafe.UnsafeUtils.SizeOf));

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
                ilGen.Emit(OpCodes.Ldc_I4, 0);

                if (info.SerializedFields != null)
                {
                    foreach (var field in info.SerializedFields)
                    {
                        Type fieldType = field.FieldType;
                        if (fieldType.IsSubclassOf(typeof(Enum)))
                        {
                            fieldType = Enum.GetUnderlyingType(fieldType);
                        }

                        if (info.IsUnmanaged && !info.HasCustomSerializer)
                        {
                            ilGen.Emit(OpCodes.Sizeof, fieldType);
                        }
                        else if (TypeCache.TryGetTypeInfo(fieldType, out var typeInfo))
                        {
                            genericGetSize = getSizeMethod.MakeGenericMethod(fieldType);

                            ilGen.Emit(OpCodes.Ldarg_0);
                            ilGen.Emit(OpCodes.Ldflda, field);
                            ilGen.Emit(OpCodes.Call, genericGetSize);
                        }

                        ilGen.Emit(OpCodes.Add);
                    }
                }

                // ilGen.Emit(OpCodes.Ldloc_0);
                ilGen.Emit(OpCodes.Ret);
            }

            return builder;
        }
    }
}
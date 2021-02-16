/* using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace Refsa.RePacker.Builder
{
    public static class LoggerBuilder
    {
        public static Func<MethodInfo> CreateDataLogger(TypeCache.Info info)
        {
            Type[] typeParams = new Type[] { info.Type };

            string name = $"{info.Type.FullName}_Logger";

            var loggerBuilder = new DynamicMethod(
                name,
                null,
                typeParams,
                true
            );

            MethodInfo stringAdder = typeof(LoggerBuilder).GetMethod(
                nameof(LoggerBuilder.AddStrings));

            MethodInfo fieldString = typeof(LoggerBuilder).GetMethod(
                nameof(LoggerBuilder.BuildFieldString));

            var ilGen = loggerBuilder.GetILGenerator();
            {
                var lb = ilGen.DeclareLocal(typeof(string));

                // Separator
                ilGen.Emit(OpCodes.Ldstr, ", ");

                // Gather all fields info in string array
                ilGen.Emit(OpCodes.Ldc_I4, info.SerializedFields.Length);
                ilGen.Emit(OpCodes.Newarr, typeof(string));
                ilGen.Emit(OpCodes.Dup);
                for (int i = 0; i < info.SerializedFields.Length; i++)
                {
                    var field = info.SerializedFields[i];

                    // Set array element
                    ilGen.Emit(OpCodes.Ldc_I4, i);
                    ilGen.Emit(OpCodes.Ldstr, field.Name);

                    ilGen.Emit(OpCodes.Ldarg_0);
                    ilGen.Emit(OpCodes.Ldfld, field);

                    // Box our data
                    if (field.FieldType.IsValueType)
                    {
                        ilGen.Emit(OpCodes.Box, field.FieldType);
                    }
                    else
                    {
                        ilGen.Emit(OpCodes.Castclass, typeof(object));
                    }

                    // Create string of Field Name + Field Value
                    ilGen.EmitCall(OpCodes.Call, fieldString, Type.EmptyTypes);

                    ilGen.Emit(OpCodes.Stelem_Ref);

                    if (i != info.SerializedFields.Length - 1)
                        ilGen.Emit(OpCodes.Dup);
                }

                // Create string from data
                ilGen.EmitCall(OpCodes.Call, stringAdder, Type.EmptyTypes);
                ilGen.Emit(OpCodes.Stloc_0);

                // Separator
                ilGen.Emit(OpCodes.Ldstr, "");

                // Create Array to hold final string
                ilGen.Emit(OpCodes.Ldc_I4_4);
                ilGen.Emit(OpCodes.Newarr, typeof(string));
                ilGen.Emit(OpCodes.Dup);

                ilGen.Emit(OpCodes.Ldc_I4_0);
                ilGen.Emit(OpCodes.Ldstr, $"{info.Type.Name} {{ ");
                ilGen.Emit(OpCodes.Stelem_Ref);
                ilGen.Emit(OpCodes.Dup);

                ilGen.Emit(OpCodes.Ldc_I4_1);
                ilGen.Emit(OpCodes.Ldloc_0);
                ilGen.Emit(OpCodes.Stelem_Ref);
                ilGen.Emit(OpCodes.Dup);

                ilGen.Emit(OpCodes.Ldc_I4_2);
                ilGen.Emit(OpCodes.Ldstr, $" }}");
                ilGen.Emit(OpCodes.Stelem_Ref);

                ilGen.EmitCall(OpCodes.Call, stringAdder, Type.EmptyTypes);
                ilGen.Emit(OpCodes.Stloc_0);

                ilGen.EmitLog(lb);

                ilGen.Emit(OpCodes.Ret);
            }

            return () =>
            {
                return loggerBuilder;
            };
        }

        static object[] genericParams = new object[1];
        public static string BuildFieldString(string name, object data)
        {
            string text = "";

            if (data == null)
            {
                return $"{name}: Unsupported";
            }

            if (TypeCache.TryGetTypeInfo(data.GetType(), out var _))
            {
                using (var reader = new StringWriter())
                {
                    var defaultOut = Console.Out;
                    Console.SetOut(reader);

                    MethodInfo toGeneric = typeof(TypeCache).GetMethod(nameof(TypeCache.LogData)).MakeGenericMethod(data.GetType());
                    genericParams[0] = data;
                    toGeneric.Invoke(null, genericParams);

                    text = name + ": " + reader.ToString().TrimEnd();

                    Console.SetOut(defaultOut);
                }
            }
            else if (data.GetType().IsArray)
            {
                if (data.GetType().GetElementType().IsValueType)
                {
                    text += $"{name}: [ ";
                    int index = 0;
                    foreach (var element in (Array)data)
                    {
                        text += $"{element.ToString()}";
                        if (index++ != ((Array)data).Length - 1)
                        {
                            text += ", ";
                        }
                        else
                        {
                            text += " ]";
                        }
                    }
                }
            }
            else if (typeof(object).IsGenericType &&
                    typeof(object) == typeof(IList<>).MakeGenericType(typeof(object).GenericTypeArguments[0]))
            {
                text += $"{name}: [ ";
                int index = 0;
                foreach (var element in (Array)data)
                {
                    text += $"{element.ToString()}";
                    if (index++ != ((Array)data).Length - 1)
                    {
                        text += ", ";
                    }
                    else
                    {
                        text += " ]";
                    }
                }
            }
            else
            {
                text = $"{name}: {data.ToString()}";
            }

            return text;
        }

        public static string AddStrings(string sep, params string[] data)
        {
            string result = "";
            // result += s1 + s2 + s3;
            for (int i = 0; i < data.Length; i++)
            {
                result += data[i];
                if (i != data.Length - 1)
                {
                    result += sep;
                }
            }
            return result;
        }
    }
} */
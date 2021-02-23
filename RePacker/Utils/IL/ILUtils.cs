
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using RePacker.Buffers;
using RePacker.Builder;

namespace RePacker.Utils
{
    public static class ILUtils
    {
        static FieldInfo boxedBufferUnwrap = typeof(BoxedBuffer).GetField(nameof(BoxedBuffer.Buffer));

        internal static void LoadArgsPack(this ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Ldarg_0);
            if (Type.GetTypeCode(fieldInfo.FieldType) != TypeCode.Object)
            {
                ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);
            }

            if (fieldInfo.DeclaringType.IsValueType)
            {
                ilGen.Emit(OpCodes.Ldarga_S, 1);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldarg_S, 1);
            }
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);
        }

        internal static void LoadArgsUnpack(this ILGenerator ilGen, FieldInfo fieldInfo)
        {
            ilGen.Emit(OpCodes.Ldarg_0);
            if (Type.GetTypeCode(fieldInfo.FieldType) != TypeCode.Object)
            {
                ilGen.Emit(OpCodes.Ldflda, boxedBufferUnwrap);
            }

            if (fieldInfo.DeclaringType.IsValueType)
            {
                ilGen.Emit(OpCodes.Ldloca_S, 0);
            }
            else
            {
                ilGen.Emit(OpCodes.Ldloc_S, 0);
            }
            ilGen.Emit(OpCodes.Ldflda, fieldInfo);
        }

        public static void EmitLog(this ILGenerator ilGen, string msg)
        {
            ilGen.Emit(OpCodes.Call, ILoggerExt.GetLogger());
            ilGen.Emit(OpCodes.Ldstr, msg);
            ilGen.Emit(OpCodes.Call, ILoggerExt.GetLogMethod());
        }

        public static void CreateLocal(this ILGenerator ilGen, Type type, MethodInfo constructor, int loc)
        {
            ilGen.DeclareLocal(type);
            ilGen.Emit(OpCodes.Newobj, constructor);
            ilGen.Emit(OpCodes.Stloc, loc);
        }

        public static void LogList<T>(List<T> list)
        {
            RePacking.Logger.Log("Logging List");
            foreach (var item in list)
            {
                RePacking.Logger.Log($"{item}");
            }
        }

        public static MethodInfo GetLogListMethod(Type type)
        {
            return typeof(ILUtils).GetMethod(nameof(ILUtils.LogList)).MakeGenericMethod(type);
        }
    }
}
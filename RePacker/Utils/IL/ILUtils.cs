
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace RePacker.Utils
{
    public static class ILUtils
    {
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
            RePacker.Logger.Log("Logging List");
            foreach (var item in list)
            {
                RePacker.Logger.Log($"{item}");
            }
        }

        public static MethodInfo GetLogListMethod(Type type)
        {
            return typeof(ILUtils).GetMethod(nameof(ILUtils.LogList)).MakeGenericMethod(type);
        }
    }
}
using System;
using System.Reflection;

namespace RePacker.Utils
{
    public interface ILogger
    {
        bool Enabled { get; set; }

        void Log(string message);
        void Warn(string message);
        void Error(string message);
        void Exception(Exception e);
    }

    public static class ILoggerExt
    {
        public static MethodInfo GetLogger()
        {
            return typeof(RePacking).GetProperty(nameof(RePacking.Logger)).GetMethod;
        }

        public static MethodInfo GetLogMethod()
        {
            return typeof(ILogger).GetMethod(nameof(ILogger.Log));
        }
    }
}
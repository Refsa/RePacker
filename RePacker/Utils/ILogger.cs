using System;
using System.Reflection;

namespace Refsa.RePacker.Utils
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
            return typeof(RePacker).GetProperty(nameof(RePacker.Logger)).GetMethod;
        }

        public static MethodInfo GetLogMethod()
        {
            return typeof(ILogger).GetMethod(nameof(ILogger.Log));
        }
    }
}
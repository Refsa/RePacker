using System;

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
}
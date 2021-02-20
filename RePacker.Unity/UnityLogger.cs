using System;
using RePacker.Utils;

namespace RePacker.Unity
{
    public class UnityLogger : ILogger
    {
        public bool Enabled { get; set; }

        public void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public void Exception(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        public void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Warn(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }
    }
}
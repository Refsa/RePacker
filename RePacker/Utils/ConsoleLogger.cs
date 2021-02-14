using System;

namespace Refsa.RePacker.Utils
{
    public class ConsoleLogger : ILogger
    {
        public bool Enabled { get; set; }

        public void Error(string message)
        {
            if (Enabled)
            {
                Console.WriteLine("🛑 - " + message);
            }
        }

        public void Exception(Exception e)
        {
            Console.WriteLine("☣ - " + e.Message);
        }

        public void Log(string message)
        {
            if (Enabled)
            {
                Console.WriteLine(message);
            }
        }

        public void Warn(string message)
        {
            if (Enabled)
            {
                Console.WriteLine("⚠ - " + message);
            }
        }
    }
}
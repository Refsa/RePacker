using System;
using RePacker.Utils;
using Xunit.Abstractions;

namespace RePacker.Tests
{
    public class TestLogger : ILogger
    {
        public bool Enabled { get; set; }

        ITestOutputHelper logger;

        public TestLogger(ITestOutputHelper logger)
        {
            this.logger = logger;
        }

        public void Error(string message)
        {
            logger.WriteLine("Error: " + message);
        }

        public void Exception(Exception e)
        {
            logger.WriteLine("Exception: " + e.Message + "\n" + e.StackTrace);
        }

        public void Log(string message)
        {
            logger.WriteLine(message);
        }

        public void Warn(string message)
        {
            logger.WriteLine("Warn: " + message);
        }
    }
}
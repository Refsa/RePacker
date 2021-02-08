

using Refsa.RePacker.Utils;

namespace Refsa.RePacker
{
    public class RePackerSettings
    {
        ILogger log;
        public ILogger Log => log;

        bool loggingEnabled = true;
        public bool LoggingEnabled
        {
            get => loggingEnabled;
            set
            {
                log.Enabled = value;
                loggingEnabled = value;
            }
        }

        bool generateIL = true;
        public bool GenerateIL => generateIL;

        public RePackerSettings()
        {
            log = new ConsoleLogger();
            log.Enabled = loggingEnabled;
        }
    }
}
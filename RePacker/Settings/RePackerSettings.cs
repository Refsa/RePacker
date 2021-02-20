
using RePacker.Utils;

namespace RePacker
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

        public RePackerSettings(ILogger logger)
        {
            log = logger;
            log.Enabled = loggingEnabled;
        }
    }
}
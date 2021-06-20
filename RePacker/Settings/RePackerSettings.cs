
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

        Endianness endianness;
        public Endianness Endianness { get => endianness; private set => endianness = value; }

        public RePackerSettings()
        {
            log = new ConsoleLogger();
            log.Enabled = loggingEnabled;
            endianness = System.BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;
        }

        public RePackerSettings(ILogger logger)
        {
            log = logger;
            log.Enabled = loggingEnabled;
            endianness = System.BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;
        }

        public RePackerSettings(bool enableLogging)
        {
            log = new ConsoleLogger();
            log.Enabled = enableLogging;
            endianness = System.BitConverter.IsLittleEndian ? Endianness.LittleEndian : Endianness.BigEndian;
        }

        public RePackerSettings(ILogger logger = null, bool enableLogging = false, Endiannness endianness = Endianness.LittleEndian)
        {
            if (logger == null)
            {
                log = new ConsoleLogger();
                log.Enabled = enableLogging;
            }

            this.endianness = endianness;
        }
    }
}
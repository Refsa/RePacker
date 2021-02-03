

using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    public static class RePacker
    {
        static RePackerSettings settings = new RePackerSettings();
        public static RePackerSettings Settings => settings;

        public static void Init(RePackerSettings settings_)
        {
            settings = settings_;
        }

        public static void Pack<T>(BoxedBuffer buffer, ref T value)
        {
            TypeCache.Serialize<T>(buffer, ref value);
        }

        public static T Unpack<T>(BoxedBuffer buffer)
        {
            return TypeCache.Deserialize<T>(buffer);
        }

        public static void UnpackInto<T>(BoxedBuffer buffer, ref T target)
        {
            TypeCache.DeserializeInto<T>(buffer, ref target);
        }

        public static void Log<T>(ref T source)
        {
            TypeCache.LogData<T>(ref source);
        }
    }
}
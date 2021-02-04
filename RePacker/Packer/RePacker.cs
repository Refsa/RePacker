

using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;

namespace Refsa.RePacker
{
    public static class RePacker
    {
        static RePackerSettings settings = new RePackerSettings();
        public static RePackerSettings Settings => settings;
        public static ILogger Logger => settings.Log;

        public static bool IsSetup => TypeCache.IsSetup;

        static RePacker()
        {
            Init();
        }

        public static void Init()
        {
            TypeCache.Setup();
        }

        public static void Init(RePackerSettings settings_)
        {
            settings = settings_;
            TypeCache.Reload();
        }

        public static void Pack<T>(BoxedBuffer buffer, ref T value)
        {
            TypeCache.Pack<T>(buffer, ref value);
        }

        public static T Unpack<T>(BoxedBuffer buffer)
        {
            return TypeCache.Unpack<T>(buffer);
        }

        public static void UnpackInto<T>(BoxedBuffer buffer, ref T target)
        {
            TypeCache.UnpackInto<T>(buffer, ref target);
        }

        public static void Log<T>(ref T source)
        {
            TypeCache.LogData<T>(ref source);
        }
    }
}
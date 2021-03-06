using RePacker.Buffers;
using RePacker.Utils;
using RePacker.Builder;

namespace RePacker
{
    public static class RePacking
    {
#if NO_LOGGING
        static RePackerSettings settings = new RePackerSettings(false);
#else
        static RePackerSettings settings = new RePackerSettings();
#endif
        internal static RePackerSettings Settings => settings;
        internal static ILogger Logger => settings.Log;

        public static bool IsSetup => TypeCache.IsSetup;

#if !NO_BOOTSTRAP
        static RePacking()
        {
            Init();
        }
#endif

        public static void Init()
        {
            if (!IsSetup)
            {
                TypeCache.Setup();
            }
            else
            {
                TypeCache.Reload();
            }
        }

        /// <summary>
        /// Initialize RePacker with user specified settings
        /// </summary>
        /// <param name="settings_">wanted settings</param>
        public static void Init(RePackerSettings settings_)
        {
            settings = settings_;
            Init();
        }

        public static void SetSettings(RePackerSettings settings_)
        {
            settings = settings_;
        }

        /// <summary>
        /// Pack value into buffer
        /// 
        /// Any value of T needs to be supported either internally or externally with RePackerWrapper<T>
        /// </summary>
        /// <param name="buffer">target buffer</param>
        /// <param name="value">target value</param>
        /// <typeparam name="T">type of value</typeparam>
        public static void Pack<T>(Buffer buffer, ref T value)
        {
            TypeCache.Pack<T>(buffer, ref value);
        }

        public static Buffer Pack<T>(ref T value)
        {
            var buffer = new Buffer(SizeOf(ref value), true);
            Pack(buffer, ref value);
            return buffer;
        }

        /// <summary>
        /// Unpack value of type T from buffer
        /// </summary>
        /// <param name="buffer">target buffer</param>
        /// <typeparam name="T">Wanted type to extract</typeparam>
        /// <returns>The extracted value as a new instance of type T</returns>
        public static T Unpack<T>(Buffer buffer)
        {
            return TypeCache.Unpack<T>(buffer);
        }

        /// <summary>
        /// Unpacks value of type T into an out parameter
        /// </summary>
        /// <param name="buffer">target buffer</param>
        /// <param name="target">output target</param>
        /// <typeparam name="T">type of value</typeparam>
        public static void UnpackOut<T>(Buffer buffer, out T target)
        {
            target = TypeCache.Unpack<T>(buffer);
        }

        /// <summary>
        /// Unpacks value of type T into an existing instance of T
        /// 
        /// Requires the correct implementation of RePackerWrapper<T>::UnpackInto to work
        /// </summary>
        /// <param name="buffer">buffer to unpack from</param>
        /// <param name="target">target object to unpack into</param>
        /// <typeparam name="T">type of object to unpack</typeparam>
        public static void UnpackInto<T>(Buffer buffer, ref T target)
        {
            TypeCache.UnpackInto<T>(buffer, ref target);
        }

        public static int SizeOf<T>(ref T value)
        {
            return TypeCache.GetSize<T>(ref value);
        }

        internal static void AddTypePackerProvider<T>(GenericProducer producer)
        {
            TypeCache.AddTypePackerProvider(typeof(T), producer);
        }

        internal static void AttemptToCreatePacker(System.Type type)
        {
            TypeCache.AttemptToCreatePacker(type);
        }
    }
}
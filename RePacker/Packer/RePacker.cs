using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Refsa.RePacker.Builder;

namespace Refsa.RePacker
{
    public static class RePacker
    {
        static RePackerSettings settings = new RePackerSettings();
        public static RePackerSettings Settings => settings;
        public static ILogger Logger => settings.Log;

        public static bool IsSetup => TypeCache.IsSetup;

#if !NO_BOOTSTRAP
        static RePacker()
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
            TypeCache.Reload();
        }

        /// <summary>
        /// Pack value into buffer
        /// 
        /// Any value of T needs to be supported either internally or externally with RePackerWrapper<T>
        /// </summary>
        /// <param name="buffer">target buffer</param>
        /// <param name="value">target value</param>
        /// <typeparam name="T">type of value</typeparam>
        public static void Pack<T>(BoxedBuffer buffer, ref T value)
        {
            TypeCache.Pack<T>(buffer, ref value);
        }

        /// <summary>
        /// Unpack value of type T from buffer
        /// </summary>
        /// <param name="buffer">target buffer</param>
        /// <typeparam name="T">Wanted type to extract</typeparam>
        /// <returns>The extracted value as a new instance of type T</returns>
        public static T Unpack<T>(BoxedBuffer buffer)
        {
            return TypeCache.Unpack<T>(buffer);
        }

        // public static object Unpack(BoxedBuffer buffer, Type type)
        // {

        // }

        /// <summary>
        /// Unpacks value of type T into an out parameter
        /// </summary>
        /// <param name="buffer">target buffer</param>
        /// <param name="target">output target</param>
        /// <typeparam name="T">type of value</typeparam>
        public static void UnpackOut<T>(BoxedBuffer buffer, out T target)
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
        public static void UnpackInto<T>(BoxedBuffer buffer, ref T target)
        {
            TypeCache.UnpackInto<T>(buffer, ref target);
        }

        public static void AddTypePackerProvider<T>(GenericProducer producer)
        {
            TypeCache.AddTypePackerProvider(typeof(T), producer);
        }
    }
}
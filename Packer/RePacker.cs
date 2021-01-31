

using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    public static class RePacker
    {
        public static void Pack<T>(BoxedBuffer buffer, ref T value)
        {
            TypeCache.Serialize<T>(buffer, ref value);
        }

        public static T Unpack<T>(BoxedBuffer buffer)
        {
            return TypeCache.Deserialize<T>(buffer);
        }
    }
}
using Refsa.Repacker.Buffers;

namespace Refsa.Repacker.Buffers
{
    public static class StackBufferFactory
    {
        public static StackBuffer New(int size)
        {
            return new StackBuffer(1024, 0, 0);
        }
    }
}
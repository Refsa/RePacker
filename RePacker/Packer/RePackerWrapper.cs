

using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    public abstract class RePackerWrapper<T> : ITypePacker
    {
        public abstract void Pack(BoxedBuffer buffer, ref T value);
        public abstract void Unpack(BoxedBuffer buffer, ref T value);
    }
}
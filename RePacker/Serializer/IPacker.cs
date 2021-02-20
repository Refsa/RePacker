using RePacker.Buffers;
using RePacker.Builder;

namespace RePacker
{
    internal interface IPacker<T> : ITypePacker
    {
        void Pack(BoxedBuffer buffer, ref T value);
        void Unpack(BoxedBuffer buffer, out T value);
        void UnpackInto(BoxedBuffer buffer, ref T value);
    }
}
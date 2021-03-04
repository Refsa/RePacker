using RePacker.Buffers;
using RePacker.Builder;

namespace RePacker
{
    internal interface IPacker<T> : ITypePacker
    {
        void Pack(Buffer buffer, ref T value);
        void Unpack(Buffer buffer, out T value);
        void UnpackInto(Buffer buffer, ref T value);
        int SizeOf(ref T value);
    }
}
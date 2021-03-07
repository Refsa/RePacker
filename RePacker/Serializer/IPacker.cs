using RePacker.Buffers;
using RePacker.Builder;

namespace RePacker
{
    internal interface IPacker<T> : ITypePacker
    {
        void Pack(ReBuffer buffer, ref T value);
        void Unpack(ReBuffer buffer, out T value);
        void UnpackInto(ReBuffer buffer, ref T value);
        int SizeOf(ref T value);
    }
}
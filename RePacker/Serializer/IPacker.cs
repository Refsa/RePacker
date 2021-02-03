

using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    public interface IPacker
    {
        void ToBuffer(ref Buffer buffer);
        void FromBuffer(ref Buffer buffer);
    }
}
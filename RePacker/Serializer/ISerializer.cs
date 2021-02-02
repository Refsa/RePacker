

using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    public interface ISerializer
    {
        void ToBuffer(ref Buffer buffer);
        void FromBuffer(ref Buffer buffer);
    }
}
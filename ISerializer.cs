

using Refsa.Repacker.Buffers;

namespace Refsa.Repacker
{
    public interface ISerializer
    {
        void ToBuffer(ref Buffer buffer);
        void FromBuffer(ref ReadOnlyBuffer buffer);
    }
}
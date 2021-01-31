

namespace Refsa.RePacker.Buffers
{
    public class BoxedBuffer
    {
        public Buffer Buffer;

        public BoxedBuffer(int size)
        {
            Buffer = new Buffer(new byte[size], 0);
        }

        public BoxedBuffer(ref Buffer buffer)
        {
            Buffer = buffer;
        }
    }
}
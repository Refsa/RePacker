

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

        public BoxedBuffer(ref ReadOnlyBuffer buffer)
        {
            Buffer = buffer.ToRegular();
        }
    }

    public class BoxedReadBuffer
    {
        public ReadOnlyBuffer Buffer;

        public BoxedReadBuffer(int size)
        {
            Buffer = new ReadOnlyBuffer(new byte[size], 0);
        }

        public BoxedReadBuffer(ref ReadOnlyBuffer buffer)
        {
            Buffer = buffer;
        }
    }
}
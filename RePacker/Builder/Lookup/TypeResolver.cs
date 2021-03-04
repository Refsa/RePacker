using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class TypeResolver<TPacker, T> where TPacker : IPacker<T>
    {
        static TPacker defaultPacker = default(TPacker);
        public static TPacker Packer { get => defaultPacker; set => defaultPacker = value; }

        public void Pack(Buffer buffer, ref T value)
        {
            defaultPacker.Pack(buffer, ref value);
        }

        public void Unpack(Buffer buffer, out T value)
        {
            defaultPacker.Unpack(buffer, out value);
        }

        public void UnpackInto(Buffer buffer, ref T value)
        {
            defaultPacker.UnpackInto(buffer, ref value);
        }
    }
}

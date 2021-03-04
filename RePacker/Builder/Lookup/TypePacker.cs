using System.Reflection;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class TypePacker<T> : IPacker<T>
    {
        public System.Action<Buffer, T> packer;
        public System.Func<Buffer, T> unpacker;

        public TypePacker(MethodInfo packer, MethodInfo unpacker)
        {
            this.packer =
                (System.Action<Buffer, T>)packer
                    .CreateDelegate(typeof(System.Action<Buffer, T>));

            this.unpacker =
                (System.Func<Buffer, T>)unpacker
                    .CreateDelegate(typeof(System.Func<Buffer, T>));
        }

        public void Pack(Buffer buffer, ref T value)
        {
            packer.Invoke(buffer, value);
        }

        public void Unpack(Buffer buffer, out T value)
        {
            value = unpacker.Invoke(buffer);
        }

        public void UnpackInto(Buffer buffer, ref T value)
        {
            value = unpacker.Invoke(buffer);
        }
    }
}
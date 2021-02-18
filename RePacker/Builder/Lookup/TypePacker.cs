using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class TypePacker<T> : IPacker<T>
    {
        public System.Action<BoxedBuffer, T> packer;
        public System.Func<BoxedBuffer, T> unpacker;

        public TypePacker(MethodInfo packer, MethodInfo unpacker)
        {
            this.packer =
                (System.Action<BoxedBuffer, T>)packer
                    .CreateDelegate(typeof(System.Action<BoxedBuffer, T>));

            this.unpacker =
                (System.Func<BoxedBuffer, T>)unpacker
                    .CreateDelegate(typeof(System.Func<BoxedBuffer, T>));
        }

        public void Pack(BoxedBuffer buffer, ref T value)
        {
            packer.Invoke(buffer, value);
        }

        public void Unpack(BoxedBuffer buffer, out T value)
        {
            value = unpacker.Invoke(buffer);
        }

        public void UnpackInto(BoxedBuffer buffer, ref T value)
        {
            value = unpacker.Invoke(buffer);
        }
    }
}
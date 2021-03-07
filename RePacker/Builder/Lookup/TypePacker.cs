using System.Reflection;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class TypePacker<T> : IPacker<T>
    {
        System.Action<ReBuffer, T> packer;
        System.Func<ReBuffer, T> unpacker;
        System.Func<T, int> getSize;

        public TypePacker(MethodInfo packer, MethodInfo unpacker, MethodInfo getSize)
        {
            this.packer =
                (System.Action<ReBuffer, T>)packer
                    .CreateDelegate(typeof(System.Action<ReBuffer, T>));

            this.unpacker =
                (System.Func<ReBuffer, T>)unpacker
                    .CreateDelegate(typeof(System.Func<ReBuffer, T>));

            if (getSize != null)
            {
                this.getSize =
                    (System.Func<T, int>)getSize
                        .CreateDelegate(typeof(System.Func<T, int>));
            }
        }

        public void Pack(ReBuffer buffer, ref T value)
        {
            packer.Invoke(buffer, value);
        }

        public void Unpack(ReBuffer buffer, out T value)
        {
            value = unpacker.Invoke(buffer);
        }

        public void UnpackInto(ReBuffer buffer, ref T value)
        {
            value = unpacker.Invoke(buffer);
        }

        public int SizeOf(ref T value)
        {
            if (getSize != null)
            {
                return getSize.Invoke(value);
            }

            throw new System.OperationCanceledException($"No GetSize defined for TypePacker<{typeof(T)}>");
        }
    }
}
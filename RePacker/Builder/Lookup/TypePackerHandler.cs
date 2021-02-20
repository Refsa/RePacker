using System.Reflection;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class TypePackerHandler
    {
        public TypeCache.Info Info;

        ITypePacker packer;
        public ITypePacker Packer => packer;

        public TypePackerHandler(TypeCache.Info info)
        {
            this.Info = info;
        }

        public void Setup(ITypePacker packer)
        {
            this.packer = packer;
        }

        public void Setup<T>(MethodInfo packer, MethodInfo unpacker)
        {
            var ts = new TypePacker<T>(packer, unpacker);
            this.packer = ts;
        }

        public void Pack<T>(BoxedBuffer buffer, ref T target)
        {
            if (this.packer is IPacker<T> packer)
            {
                packer.Pack(buffer, ref target);
            }
        }

        public T Unpack<T>(BoxedBuffer buffer)
        {
            if (this.packer is IPacker<T> packer)
            {
                packer.Unpack(buffer, out T value);
                return value;
            }

            return default(T);
        }

        public void UnpackInto<T>(BoxedBuffer buffer, ref T target)
        {
            if (this.packer is RePackerWrapper<T> wrapper)
            {
                wrapper.UnpackInto(buffer, ref target);
            }
        }

        public IPacker<T> GetTypePacker<T>() where T : IPacker<T>
        {
            if (this.packer is IPacker<T>)
            {
                return (T)this.packer;
            }

            return default(T);
        }
    }
}
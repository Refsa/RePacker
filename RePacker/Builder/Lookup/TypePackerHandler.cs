using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker.Builder
{
    internal class TypePackerHandler
    {
        public TypeCache.Info Info;

        ITypePacker packer;

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

        public void SetLogger<T>(MethodInfo logger)
        {
            if (this.packer is TypePacker<T> serializer)
            {
                serializer.SetLogger(logger);
            }
        }

        public void RunLogger<T>(ref T target)
        {
            if (this.packer is TypePacker<T> serializer)
            {
                serializer.logger.Invoke(target);
            }
        }

        public void Pack<T>(BoxedBuffer buffer, ref T target)
        {
            if (this.packer is TypePacker<T> serializer)
            {
                serializer.packer.Invoke(buffer, target);
            }
            else if (this.packer is RePackerWrapper<T> wrapper)
            {
                wrapper.Pack(buffer, ref target);
            }
        }

        public T Unpack<T>(BoxedBuffer buffer)
        {
            if (this.packer is TypePacker<T> serializer)
            {
                return serializer.unpacker.Invoke(buffer);
            }
            else if (this.packer is RePackerWrapper<T> wrapper)
            {
                wrapper.Unpack(buffer, out T value);
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

        public T GetTypePacker<T>() where T : ITypePacker
        {
            if (this.packer is T)
            {
                return (T)this.packer;
            }

            return default(T);
        }
    }
}
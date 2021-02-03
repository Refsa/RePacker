

using System;
using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    public class TypePackerHandler
    {
        public TypeCache.Info Info;

        ITypePacker serializer;

        public TypePackerHandler(TypeCache.Info info)
        {
            this.Info = info;
        }

        public void Setup(ITypePacker serializer)
        {
            this.serializer = serializer;
        }

        public void Setup<T>(MethodInfo packer, MethodInfo unpacker, MethodInfo logger)
        {
            var ts = new TypePacker<T>(packer, unpacker);
            ts.SetLogger(logger);
            this.serializer = ts;
        }

        public void SetLogger<T>(MethodInfo logger)
        {
            if (this.serializer is TypePacker<T> serializer)
            {
                serializer.SetLogger(logger);
            }
        }

        public void RunLogger<T>(ref T target)
        {
            if (this.serializer is TypePacker<T> serializer)
            {
                serializer.logger.Invoke(target);
            }
        }

        public void Pack<T>(BoxedBuffer buffer, ref T target)
        {
            // packer.Invoke(buffer, target);
            if (this.serializer is TypePacker<T> serializer)
            {
                serializer.packer.Invoke(buffer, target);
            }
            else if (this.serializer is RePackerWrapper<T> wrapper)
            {
                wrapper.Pack(buffer, ref target);
            }
        }

        public T Unpack<T>(BoxedBuffer buffer, object target = null)
        {
            // return unpacker.Invoke(buffer);
            if (this.serializer is TypePacker<T> serializer)
            {
                return serializer.unpacker.Invoke(buffer);
            }
            else if (this.serializer is RePackerWrapper<T> wrapper)
            {
                if (target == null)
                {
                    T value = Activator.CreateInstance<T>();
                    wrapper.Unpack(buffer, ref value);
                    return value;
                }
                else
                {
                    T asT = (T)target;
                    wrapper.Unpack(buffer, ref asT);
                    return asT;
                }
            }

            return default(T);
        }
    }
}
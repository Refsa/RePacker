

using System;
using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    interface ITypeSerializer { }

    class TypeSerializer<T> : ITypeSerializer
    {
        public System.Action<BoxedBuffer, T> packer;
        public System.Func<BoxedBuffer, T> unpacker;

        public TypeSerializer(MethodInfo packer, MethodInfo unpacker)
        {
            this.packer =
                (System.Action<BoxedBuffer, T>)packer
                    .CreateDelegate(typeof(System.Action<BoxedBuffer, T>));

            this.unpacker =
                (System.Func<BoxedBuffer, T>)unpacker
                    .CreateDelegate(typeof(System.Func<BoxedBuffer, T>));
        }
    }

    public class TypePacker
    {
        public TypeCache.Info Info;

        ITypeSerializer Serializer;

        public TypePacker(TypeCache.Info info)
        {
            this.Info = info;
        }

        public void Setup<T>(MethodInfo packer, MethodInfo unpacker)
        {
            this.Serializer = new TypeSerializer<T>(packer, unpacker);
        }

        public void Pack<T>(BoxedBuffer buffer, ref T target)
        {
            // packer.Invoke(buffer, target);
            if (this.Serializer is TypeSerializer<T> serializer)
            {
                serializer.packer.Invoke(buffer, target);
            }
        }

        public T Unpack<T>(BoxedBuffer buffer)
        {
            // return unpacker.Invoke(buffer);
            if (this.Serializer is TypeSerializer<T> serializer)
            {
                return serializer.unpacker.Invoke(buffer);
            }

            return default(T);
        }
    }
}


using System;
using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    interface ITypeSerializer
    {

    }

    class TypeSerializer<T> : ITypeSerializer
    {
        public System.Action<BoxedBuffer, T> packer;
        public System.Func<BoxedBuffer, T> unpacker;

        public System.Action<T> logger;

        public TypeSerializer(MethodInfo packer, MethodInfo unpacker)
        {
            this.packer =
                (System.Action<BoxedBuffer, T>)packer
                    .CreateDelegate(typeof(System.Action<BoxedBuffer, T>));

            this.unpacker =
                (System.Func<BoxedBuffer, T>)unpacker
                    .CreateDelegate(typeof(System.Func<BoxedBuffer, T>));
        }

        public void SetLogger(MethodInfo logger)
        {
            this.logger = (System.Action<T>)logger.CreateDelegate(typeof(System.Action<T>));
        }
    }

    public class TypePacker
    {
        public TypeCache.Info Info;

        ITypeSerializer serializer;

        public TypePacker(TypeCache.Info info)
        {
            this.Info = info;
        }

        public void Setup<T>(MethodInfo packer, MethodInfo unpacker, MethodInfo logger)
        {
            var ts = new TypeSerializer<T>(packer, unpacker);
            ts.SetLogger(logger);
            this.serializer = ts;
        }

        public void SetLogger<T>(MethodInfo logger)
        {
            if (this.serializer is TypeSerializer<T> serializer)
            {
                serializer.SetLogger(logger);
            }
        }

        public void RunLogger<T>(ref T target)
        {
            if (this.serializer is TypeSerializer<T> serializer)
            {
                serializer.logger.Invoke(target);
            }
        }

        public void Pack<T>(BoxedBuffer buffer, ref T target)
        {
            // packer.Invoke(buffer, target);
            if (this.serializer is TypeSerializer<T> serializer)
            {
                serializer.packer.Invoke(buffer, target);
            }
        }

        public T Unpack<T>(BoxedBuffer buffer)
        {
            // return unpacker.Invoke(buffer);
            if (this.serializer is TypeSerializer<T> serializer)
            {
                return serializer.unpacker.Invoke(buffer);
            }

            return default(T);
        }
    }
}
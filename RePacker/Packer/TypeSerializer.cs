using System.Reflection;
using Refsa.RePacker.Buffers;

namespace Refsa.RePacker
{
    public interface ITypeSerializer
    {

    }

    public class TypeSerializer<T> : ITypeSerializer
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
}
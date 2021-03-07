using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    public abstract class RePackerWrapper<T> : IPacker<T>
    {
        public static bool IsCopyable = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Pack(ReBuffer buffer, ref T value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Unpack(ReBuffer buffer, out T value)
        {
            throw new System.NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void UnpackInto(ReBuffer buffer, ref T value)
        {
            throw new System.NotImplementedException();
        }

        public virtual int SizeOf(ref T value)
        {
            return 0;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public sealed class RePackerWrapperAttribute : System.Attribute
    {
        public System.Type WrapperFor;

        public RePackerWrapperAttribute(System.Type wrapperFor)
        {
            WrapperFor = wrapperFor;
        }
    }
}
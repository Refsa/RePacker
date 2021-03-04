using System.Runtime.CompilerServices;
using RePacker.Buffers;
using RePacker.Builder;

namespace RePacker.Builder
{
    internal class ArrayPacker<TElement> : RePackerWrapper<TElement[]>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref TElement[] value)
        {
            buffer.PackArray(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out TElement[] value)
        {
            buffer.UnpackArray<TElement>(out value);
        }
    }

    internal class ArrayUnmanagedPacker<TElement> : RePackerWrapper<TElement[]> where TElement : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref TElement[] value)
        {
            buffer.PackBlittableArray(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out TElement[] value)
        {
            buffer.UnpackUnmanagedArrayOut<TElement>(out value);
        }
    }

    internal class Array2DPacker<TElement> : RePackerWrapper<TElement[,]> where TElement : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref TElement[,] value)
        {
            buffer.PackArray2D(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out TElement[,] value)
        {
            buffer.UnpackArray2D(out value);
        }
    }

    internal class Array3DPacker<TElement> : RePackerWrapper<TElement[,,]> where TElement : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref TElement[,,] value)
        {
            buffer.PackArray3D(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out TElement[,,] value)
        {
            buffer.UnpackArray3D(out value);
        }
    }

    internal class Array4DPacker<TElement> : RePackerWrapper<TElement[,,,]> where TElement : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref TElement[,,,] value)
        {
            buffer.PackArray4D(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out TElement[,,,] value)
        {
            buffer.UnpackArray4D(out value);
        }
    }
}
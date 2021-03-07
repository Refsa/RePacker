using System.Runtime.CompilerServices;
using RePacker.Buffers;
using RePacker.Builder;

namespace RePacker.Builder
{
    internal class ArrayPacker<TElement> : RePackerWrapper<TElement[]>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref TElement[] value)
        {
            buffer.PackArray(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out TElement[] value)
        {
            buffer.UnpackArray<TElement>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref TElement[] value)
        {
            return PackerCollectionsExt.SizeOfCollection<TElement>(value.GetEnumerator());
        }
    }

    internal class ArrayUnmanagedPacker<TElement> : RePackerWrapper<TElement[]> where TElement : unmanaged
    {
        static int TElementSize = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref TElement[] value)
        {
            buffer.PackBlittableArray(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out TElement[] value)
        {
            buffer.UnpackUnmanagedArrayOut<TElement>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref TElement[] value)
        {
            if (value == null || value.Length == 0) return sizeof(ulong);
            if (TElementSize == 0) TElementSize = TypeCache.GetSize(ref value[0]);

            return sizeof(ulong) + TElementSize * value.Length;
        }
    }

    internal class Array2DPacker<TElement> : RePackerWrapper<TElement[,]> where TElement : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref TElement[,] value)
        {
            buffer.PackArray2D(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out TElement[,] value)
        {
            buffer.UnpackArray2D(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref TElement[,] value)
        {
            return PackerCollectionsExt.SizeOfCollection<TElement>(value.GetEnumerator());
        }
    }

    internal class Array3DPacker<TElement> : RePackerWrapper<TElement[,,]> where TElement : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref TElement[,,] value)
        {
            buffer.PackArray3D(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out TElement[,,] value)
        {
            buffer.UnpackArray3D(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref TElement[,,] value)
        {
            return PackerCollectionsExt.SizeOfCollection<TElement>(value.GetEnumerator());
        }
    }

    internal class Array4DPacker<TElement> : RePackerWrapper<TElement[,,,]> where TElement : unmanaged
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref TElement[,,,] value)
        {
            buffer.PackArray4D(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out TElement[,,,] value)
        {
            buffer.UnpackArray4D(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref TElement[,,,] value)
        {
            return PackerCollectionsExt.SizeOfCollection<TElement>(value.GetEnumerator());
        }
    }
}
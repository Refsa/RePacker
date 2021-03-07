using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class ValueTuplePacker<T1, T2> :
        RePackerWrapper<System.ValueTuple<T1, T2>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref System.ValueTuple<T1, T2> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out System.ValueTuple<T1, T2> value)
        {
            buffer.UnpackValueTuple<T1, T2>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref System.ValueTuple<T1, T2> value)
        {
            buffer.UnpackValueTuple<T1, T2>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2> value)
        {
            var i1 = value.Item1;
            var i2 = value.Item2;

            return RePacking.SizeOf(ref i1) + RePacking.SizeOf(ref i2);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref System.ValueTuple<T1, T2, T3> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out System.ValueTuple<T1, T2, T3> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref System.ValueTuple<T1, T2, T3> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3> value)
        {
            var i1 = value.Item1;
            var i2 = value.Item2;
            var i3 = value.Item3;

            return RePacking.SizeOf(ref i1) + RePacking.SizeOf(ref i2) + RePacking.SizeOf(ref i3);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3, T4>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref System.ValueTuple<T1, T2, T3, T4> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out System.ValueTuple<T1, T2, T3, T4> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref System.ValueTuple<T1, T2, T3, T4> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3, T4> value)
        {
            var i1 = value.Item1;
            var i2 = value.Item2;
            var i3 = value.Item3;
            var i4 = value.Item4;

            return RePacking.SizeOf(ref i1) + RePacking.SizeOf(ref i2) + RePacking.SizeOf(ref i3) + RePacking.SizeOf(ref i4);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3, T4, T5>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out System.ValueTuple<T1, T2, T3, T4, T5> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3, T4, T5> value)
        {
            var i1 = value.Item1;
            var i2 = value.Item2;
            var i3 = value.Item3;
            var i4 = value.Item4;
            var i5 = value.Item5;

            return RePacking.SizeOf(ref i1) + RePacking.SizeOf(ref i2) + RePacking.SizeOf(ref i3) +
                    RePacking.SizeOf(ref i4) + RePacking.SizeOf(ref i5);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5, T6> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out System.ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            var i1 = value.Item1;
            var i2 = value.Item2;
            var i3 = value.Item3;
            var i4 = value.Item4;
            var i5 = value.Item5;
            var i6 = value.Item6;

            return RePacking.SizeOf(ref i1) + RePacking.SizeOf(ref i2) + RePacking.SizeOf(ref i3) +
                    RePacking.SizeOf(ref i4) + RePacking.SizeOf(ref i5) + RePacking.SizeOf(ref i6);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5, T6, T7> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out System.ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            var i1 = value.Item1;
            var i2 = value.Item2;
            var i3 = value.Item3;
            var i4 = value.Item4;
            var i5 = value.Item5;
            var i6 = value.Item6;
            var i7 = value.Item7;

            return RePacking.SizeOf(ref i1) + RePacking.SizeOf(ref i2) + RePacking.SizeOf(ref i3) + 
                    RePacking.SizeOf(ref i4) + RePacking.SizeOf(ref i5) + RePacking.SizeOf(ref i6) +
                    RePacking.SizeOf(ref i7);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5, T6, T7, TRest> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
        where TRest : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(Buffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(Buffer buffer, out System.ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(Buffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            var i1 = value.Item1;
            var i2 = value.Item2;
            var i3 = value.Item3;
            var i4 = value.Item4;
            var i5 = value.Item5;
            var i6 = value.Item6;
            var i7 = value.Item7;
            var rest = value.Rest;

            return RePacking.SizeOf(ref i1) + RePacking.SizeOf(ref i2) + RePacking.SizeOf(ref i3) + 
                    RePacking.SizeOf(ref i4) + RePacking.SizeOf(ref i5) + RePacking.SizeOf(ref i6) +
                    RePacking.SizeOf(ref i7) + RePacking.SizeOf(ref rest);
        }
    }
}
using System.Runtime.CompilerServices;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class ValueTuplePacker<T1> :
        RePackerWrapper<System.ValueTuple<T1>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref System.ValueTuple<T1> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out System.ValueTuple<T1> value)
        {
            buffer.UnpackValueTuple<T1>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref System.ValueTuple<T1> value)
        {
            buffer.UnpackValueTuple<T1>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1> value)
        {
            return RePacking.SizeOf(ref value.Item1);
        }
    }

    internal class ValueTuplePacker<T1, T2> :
        RePackerWrapper<System.ValueTuple<T1, T2>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref System.ValueTuple<T1, T2> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out System.ValueTuple<T1, T2> value)
        {
            buffer.UnpackValueTuple<T1, T2>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref System.ValueTuple<T1, T2> value)
        {
            buffer.UnpackValueTuple<T1, T2>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2> value)
        {
            return RePacking.SizeOf(ref value.Item1) + RePacking.SizeOf(ref value.Item2);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out System.ValueTuple<T1, T2, T3> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3> value)
        {
            return RePacking.SizeOf(ref value.Item1) + RePacking.SizeOf(ref value.Item2) + RePacking.SizeOf(ref value.Item3);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3, T4>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3, T4> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out System.ValueTuple<T1, T2, T3, T4> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3, T4> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3, T4> value)
        {
            return RePacking.SizeOf(ref value.Item1) + RePacking.SizeOf(ref value.Item2) + RePacking.SizeOf(ref value.Item3) +
                    RePacking.SizeOf(ref value.Item4);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3, T4, T5>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out System.ValueTuple<T1, T2, T3, T4, T5> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3, T4, T5> value)
        {
            return RePacking.SizeOf(ref value.Item1) + RePacking.SizeOf(ref value.Item2) + RePacking.SizeOf(ref value.Item3) +
                    RePacking.SizeOf(ref value.Item4) + RePacking.SizeOf(ref value.Item5);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5, T6> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out System.ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            return RePacking.SizeOf(ref value.Item1) + RePacking.SizeOf(ref value.Item2) + RePacking.SizeOf(ref value.Item3) +
                    RePacking.SizeOf(ref value.Item4) + RePacking.SizeOf(ref value.Item5) + RePacking.SizeOf(ref value.Item6);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5, T6, T7> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out System.ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            return RePacking.SizeOf(ref value.Item1) + RePacking.SizeOf(ref value.Item2) + RePacking.SizeOf(ref value.Item3) +
                    RePacking.SizeOf(ref value.Item4) + RePacking.SizeOf(ref value.Item5) + RePacking.SizeOf(ref value.Item6) +
                    RePacking.SizeOf(ref value.Item7);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5, T6, T7, TRest> :
        RePackerWrapper<System.ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
        where TRest : struct
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Pack(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            buffer.PackValueTuple(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpack(ReBuffer buffer, out System.ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void UnpackInto(ReBuffer buffer, ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int SizeOf(ref System.ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            return RePacking.SizeOf(ref value.Item1) + RePacking.SizeOf(ref value.Item2) + RePacking.SizeOf(ref value.Item3) +
                    RePacking.SizeOf(ref value.Item4) + RePacking.SizeOf(ref value.Item5) + RePacking.SizeOf(ref value.Item6) +
                    RePacking.SizeOf(ref value.Item7) + RePacking.SizeOf(ref value.Rest);
        }
    }
}
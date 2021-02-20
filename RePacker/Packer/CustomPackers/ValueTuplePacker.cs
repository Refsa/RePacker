using System;
using RePacker.Buffers;

namespace RePacker.Builder
{
    internal class ValueTuplePacker<T1, T2> :
        RePackerWrapper<ValueTuple<T1, T2>>
    {
        public override void Pack(BoxedBuffer buffer, ref ValueTuple<T1, T2> value)
        {
            buffer.PackValueTuple(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out ValueTuple<T1, T2> value)
        {
            buffer.UnpackValueTuple<T1, T2>(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref ValueTuple<T1, T2> value)
        {
            buffer.UnpackValueTuple<T1, T2>(out value);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3> :
        RePackerWrapper<ValueTuple<T1, T2, T3>>
    {
        public override void Pack(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3> value)
        {
            buffer.PackValueTuple(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out ValueTuple<T1, T2, T3> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3>(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3>(out value);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4> :
        RePackerWrapper<ValueTuple<T1, T2, T3, T4>>
    {
        public override void Pack(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3, T4> value)
        {
            buffer.PackValueTuple(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out ValueTuple<T1, T2, T3, T4> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4>(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3, T4> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4>(out value);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5> :
        RePackerWrapper<ValueTuple<T1, T2, T3, T4, T5>>
    {
        public override void Pack(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3, T4, T5> value)
        {
            buffer.PackValueTuple(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out ValueTuple<T1, T2, T3, T4, T5> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5>(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3, T4, T5> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5>(out value);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5, T6> :
        RePackerWrapper<ValueTuple<T1, T2, T3, T4, T5, T6>>
    {
        public override void Pack(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            buffer.PackValueTuple(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6>(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6>(out value);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5, T6, T7> :
        RePackerWrapper<ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
    {
        public override void Pack(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            buffer.PackValueTuple(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7>(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7>(out value);
        }
    }

    internal class ValueTuplePacker<T1, T2, T3, T4, T5, T6, T7, TRest> :
        RePackerWrapper<ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
        where TRest : struct
    {
        public override void Pack(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            buffer.PackValueTuple(ref value);
        }

        public override void Unpack(BoxedBuffer buffer, out ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(out value);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            buffer.UnpackValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(out value);
        }
    }
}
using System;
using UnityEngine;
using Refsa.RePacker;
using Refsa.RePacker.Buffers;

using static Refsa.RePacker.RePacker;

namespace Refsa.RePacker.Unity
{
    [RePackerWrapper(typeof(Quaternion))]
    public class QuaternionWrapper : RePackerWrapper<Quaternion>
    {
        public override void Pack(BoxedBuffer buffer, ref Quaternion value)
        {
            buffer.Buffer.PushFloat(ref value.x);
            buffer.Buffer.PushFloat(ref value.y);
            buffer.Buffer.PushFloat(ref value.z);
            buffer.Buffer.PushFloat(ref value.w);
        }

        public override void Unpack(BoxedBuffer buffer, out Quaternion value)
        {
            buffer.Buffer.PopFloat(out value.x);
            buffer.Buffer.PopFloat(out value.y);
            buffer.Buffer.PopFloat(out value.z);
            buffer.Buffer.PopFloat(out value.w);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Quaternion value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(Vector3))]
    public class Vector3Wrapper : RePackerWrapper<Vector3>
    {
        public override void Pack(BoxedBuffer buffer, ref Vector3 value)
        {
            buffer.Buffer.PushFloat(ref value.x);
            buffer.Buffer.PushFloat(ref value.y);
            buffer.Buffer.PushFloat(ref value.z);
        }

        public override void Unpack(BoxedBuffer buffer, out Vector3 value)
        {
            buffer.Buffer.PopFloat(out value.x);
            buffer.Buffer.PopFloat(out value.y);
            buffer.Buffer.PopFloat(out value.z);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Vector3 value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(UnityEngine.Transform))]
    public class TransformWrapper : RePackerWrapper<UnityEngine.Transform>
    {
        public override void Pack(BoxedBuffer buffer, ref UnityEngine.Transform value)
        {
            var pos = value.localPosition;
            Pack<Vector3>(buffer, ref pos);
            var rot = value.localRotation;
            Pack<Quaternion>(buffer, ref rot);
            var scale = value.localScale;
            Pack<Vector3>(buffer, ref scale);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Transform value)
        {
            value.localPosition = Unpack<Vector3>(buffer);
            value.localRotation = Unpack<Quaternion>(buffer);
            value.localScale = Unpack<Vector3>(buffer);
        }
    }

}

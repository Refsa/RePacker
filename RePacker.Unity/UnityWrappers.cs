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

#region Vector
    [RePackerWrapper(typeof(Vector4))]
    public class Vector4Wrapper : RePackerWrapper<Vector4>
    {
        public override void Pack(BoxedBuffer buffer, ref Vector4 value)
        {
            buffer.Buffer.PushFloat(ref value.x);
            buffer.Buffer.PushFloat(ref value.y);
            buffer.Buffer.PushFloat(ref value.z);
            buffer.Buffer.PushFloat(ref value.w);
        }

        public override void Unpack(BoxedBuffer buffer, out Vector4 value)
        {
            buffer.Buffer.PopFloat(out value.x);
            buffer.Buffer.PopFloat(out value.y);
            buffer.Buffer.PopFloat(out value.z);
            buffer.Buffer.PopFloat(out value.w);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Vector4 value)
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

    [RePackerWrapper(typeof(Vector2))]
    public class Vector2Wrapper : RePackerWrapper<Vector2>
    {
        public override void Pack(BoxedBuffer buffer, ref Vector2 value)
        {
            buffer.Buffer.PushFloat(ref value.x);
            buffer.Buffer.PushFloat(ref value.y);
        }

        public override void Unpack(BoxedBuffer buffer, out Vector2 value)
        {
            buffer.Buffer.PopFloat(out value.x);
            buffer.Buffer.PopFloat(out value.y);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Vector2 value)
        {
            Unpack(buffer, out value);
        }
    }
#endregion

#region VectorInt
    [RePackerWrapper(typeof(Vector3Int))]
    public class Vector3IntWrapper : RePackerWrapper<Vector3Int>
    {
        public override void Pack(BoxedBuffer buffer, ref Vector3Int value)
        {
            int vx = value.x;
            int vy = value.y;
            int vz = value.z;
            buffer.Buffer.PushInt(ref vx);
            buffer.Buffer.PushInt(ref vy);
            buffer.Buffer.PushInt(ref vz);
        }

        public override void Unpack(BoxedBuffer buffer, out Vector3Int value)
        {
            buffer.Buffer.PopInt(out int vx);
            buffer.Buffer.PopInt(out int vy);
            buffer.Buffer.PopInt(out int vz);

            value = new Vector3Int(vx, vy, vz);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Vector3Int value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(Vector2Int))]
    public class Vector2IntWrapper : RePackerWrapper<Vector2Int>
    {
        public override void Pack(BoxedBuffer buffer, ref Vector2Int value)
        {
            int vx = value.x;
            int vy = value.y;
            buffer.Buffer.PushInt(ref vx);
            buffer.Buffer.PushInt(ref vy);
        }

        public override void Unpack(BoxedBuffer buffer, out Vector2Int value)
        {
            buffer.Buffer.PopInt(out int vx);
            buffer.Buffer.PopInt(out int vy);

            value = new Vector2Int(vx, vy);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Vector2Int value)
        {
            Unpack(buffer, out value);
        }
    }
#endregion

#region Color
    [RePackerWrapper(typeof(Color))]
    public class ColorWrapper : RePackerWrapper<Color>
    {
        public override void Pack(BoxedBuffer buffer, ref Color value)
        {
            buffer.Buffer.PushFloat(ref value.r);
            buffer.Buffer.PushFloat(ref value.g);
            buffer.Buffer.PushFloat(ref value.b);
            buffer.Buffer.PushFloat(ref value.a);
        }

        public override void Unpack(BoxedBuffer buffer, out Color value)
        {
            buffer.Buffer.PopFloat(out value.r);
            buffer.Buffer.PopFloat(out value.g);
            buffer.Buffer.PopFloat(out value.b);
            buffer.Buffer.PopFloat(out value.a);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Color value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(Color32))]
    public class Color32Wrapper : RePackerWrapper<Color32>
    {
        public override void Pack(BoxedBuffer buffer, ref Color32 value)
        {
            buffer.Buffer.PushByte(ref value.r);
            buffer.Buffer.PushByte(ref value.g);
            buffer.Buffer.PushByte(ref value.b);
            buffer.Buffer.PushByte(ref value.a);
        }

        public override void Unpack(BoxedBuffer buffer, out Color32 value)
        {
            value = new Color32();
            buffer.Buffer.PopByte(out value.r);
            buffer.Buffer.PopByte(out value.g);
            buffer.Buffer.PopByte(out value.b);
            buffer.Buffer.PopByte(out value.a);
        }

        public override void UnpackInto(BoxedBuffer buffer, ref Color32 value)
        {
            Unpack(buffer, out value);
        }
    }
#endregion

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

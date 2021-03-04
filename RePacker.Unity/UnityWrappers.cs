using UnityEngine;
using RePacker.Buffers;
using RePacker.Buffers.Extra;
using RePacker.Builder;
using static RePacker.RePacking;

namespace RePacker.Unity
{
    [RePackerWrapper(typeof(Quaternion))]
    public class QuaternionPacker : RePackerWrapper<Quaternion>
    {
        public override void Pack(Buffer buffer, ref Quaternion value)
        {
            buffer.PushFloat(ref value.x);
            buffer.PushFloat(ref value.y);
            buffer.PushFloat(ref value.z);
            buffer.PushFloat(ref value.w);
        }

        public override void Unpack(Buffer buffer, out Quaternion value)
        {
            buffer.PopFloat(out value.x);
            buffer.PopFloat(out value.y);
            buffer.PopFloat(out value.z);
            buffer.PopFloat(out value.w);
        }

        public override void UnpackInto(Buffer buffer, ref Quaternion value)
        {
            Unpack(buffer, out value);
        }
    }

    #region AnimationCurve
    [RePackerWrapper(typeof(Keyframe))]
    public class KeyframePacker : RePackerWrapper<Keyframe>
    {
        public override void Pack(Buffer buffer, ref Keyframe value)
        {
            var time = value.time;
            var val = value.value;
            var inTangent = value.inTangent;
            var outTangent = value.outTangent;
            var inWeight = value.inWeight;
            var outWeight = value.outWeight;
            var weightedMode = value.weightedMode;

            buffer.PushFloat(ref time);
            buffer.PushFloat(ref val);
            buffer.PushFloat(ref inTangent);
            buffer.PushFloat(ref outTangent);
            buffer.PushFloat(ref inWeight);
            buffer.PushFloat(ref outWeight);
            buffer.PackEnum(ref weightedMode);
        }

        public override void Unpack(Buffer buffer, out Keyframe value)
        {
            value = new Keyframe();

            var time = 0f;
            var val = 0f;
            var inTangent = 0f;
            var outTangent = 0f;
            var inWeight = 0f;
            var outWeight = 0f;

            buffer.PopFloat(out time);
            buffer.PopFloat(out val);
            buffer.PopFloat(out inTangent);
            buffer.PopFloat(out outTangent);
            buffer.PopFloat(out inWeight);
            buffer.PopFloat(out outWeight);
            value.weightedMode = buffer.UnpackEnum<WeightedMode>();
        }

        public override void UnpackInto(Buffer buffer, ref Keyframe value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(AnimationCurve))]
    public class AnimationCurvePacker : RePackerWrapper<AnimationCurve>
    {
        public override void Pack(Buffer buffer, ref AnimationCurve value)
        {
            var preMode = value.preWrapMode;
            var postMode = value.postWrapMode;
            var keys = value.keys;

            buffer.PackEnum(ref preMode);
            buffer.PackEnum(ref postMode);
            RePacking.Pack(buffer, ref keys);
        }

        public override void Unpack(Buffer buffer, out AnimationCurve value)
        {
            var preMode = buffer.UnpackEnum<WrapMode>();
            var postMode = buffer.UnpackEnum<WrapMode>();
            var keys = RePacking.Unpack<Keyframe[]>(buffer);

            value = new AnimationCurve(keys);
            value.preWrapMode = preMode;
            value.postWrapMode = postMode;
        }

        public override void UnpackInto(Buffer buffer, ref AnimationCurve value)
        {
            Unpack(buffer, out value);
        }
    }
    #endregion

    #region Vector
    [RePackerWrapper(typeof(Vector4))]
    public class Vector4Packer : RePackerWrapper<Vector4>
    {
        public override void Pack(Buffer buffer, ref Vector4 value)
        {
            buffer.PushFloat(ref value.x);
            buffer.PushFloat(ref value.y);
            buffer.PushFloat(ref value.z);
            buffer.PushFloat(ref value.w);
        }

        public override void Unpack(Buffer buffer, out Vector4 value)
        {
            buffer.PopFloat(out value.x);
            buffer.PopFloat(out value.y);
            buffer.PopFloat(out value.z);
            buffer.PopFloat(out value.w);
        }

        public override void UnpackInto(Buffer buffer, ref Vector4 value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(Vector3))]
    public class Vector3Packer : RePackerWrapper<Vector3>
    {
        public override void Pack(Buffer buffer, ref Vector3 value)
        {
            buffer.PushFloat(ref value.x);
            buffer.PushFloat(ref value.y);
            buffer.PushFloat(ref value.z);
        }

        public override void Unpack(Buffer buffer, out Vector3 value)
        {
            buffer.PopFloat(out value.x);
            buffer.PopFloat(out value.y);
            buffer.PopFloat(out value.z);
        }

        public override void UnpackInto(Buffer buffer, ref Vector3 value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(Vector2))]
    public class Vector2Packer : RePackerWrapper<Vector2>
    {
        public override void Pack(Buffer buffer, ref Vector2 value)
        {
            buffer.PushFloat(ref value.x);
            buffer.PushFloat(ref value.y);
        }

        public override void Unpack(Buffer buffer, out Vector2 value)
        {
            buffer.PopFloat(out value.x);
            buffer.PopFloat(out value.y);
        }

        public override void UnpackInto(Buffer buffer, ref Vector2 value)
        {
            Unpack(buffer, out value);
        }
    }
    #endregion

    #region VectorInt
    [RePackerWrapper(typeof(Vector3Int))]
    public class Vector3IntPacker : RePackerWrapper<Vector3Int>
    {
        public override void Pack(Buffer buffer, ref Vector3Int value)
        {
            int vx = value.x;
            int vy = value.y;
            int vz = value.z;
            buffer.PushInt(ref vx);
            buffer.PushInt(ref vy);
            buffer.PushInt(ref vz);
        }

        public override void Unpack(Buffer buffer, out Vector3Int value)
        {
            buffer.PopInt(out int vx);
            buffer.PopInt(out int vy);
            buffer.PopInt(out int vz);

            value = new Vector3Int(vx, vy, vz);
        }

        public override void UnpackInto(Buffer buffer, ref Vector3Int value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(Vector2Int))]
    public class Vector2IntPacker : RePackerWrapper<Vector2Int>
    {
        public override void Pack(Buffer buffer, ref Vector2Int value)
        {
            int vx = value.x;
            int vy = value.y;
            buffer.PushInt(ref vx);
            buffer.PushInt(ref vy);
        }

        public override void Unpack(Buffer buffer, out Vector2Int value)
        {
            buffer.PopInt(out int vx);
            buffer.PopInt(out int vy);

            value = new Vector2Int(vx, vy);
        }

        public override void UnpackInto(Buffer buffer, ref Vector2Int value)
        {
            Unpack(buffer, out value);
        }
    }
    #endregion

    #region Color
    [RePackerWrapper(typeof(Color))]
    public class ColorPacker : RePackerWrapper<Color>
    {
        public override void Pack(Buffer buffer, ref Color value)
        {
            buffer.PushFloat(ref value.r);
            buffer.PushFloat(ref value.g);
            buffer.PushFloat(ref value.b);
            buffer.PushFloat(ref value.a);
        }

        public override void Unpack(Buffer buffer, out Color value)
        {
            buffer.PopFloat(out value.r);
            buffer.PopFloat(out value.g);
            buffer.PopFloat(out value.b);
            buffer.PopFloat(out value.a);
        }

        public override void UnpackInto(Buffer buffer, ref Color value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(Color32))]
    public class Color32Packer : RePackerWrapper<Color32>
    {
        public override void Pack(Buffer buffer, ref Color32 value)
        {
            buffer.PushByte(ref value.r);
            buffer.PushByte(ref value.g);
            buffer.PushByte(ref value.b);
            buffer.PushByte(ref value.a);
        }

        public override void Unpack(Buffer buffer, out Color32 value)
        {
            value = new Color32();
            buffer.PopByte(out value.r);
            buffer.PopByte(out value.g);
            buffer.PopByte(out value.b);
            buffer.PopByte(out value.a);
        }

        public override void UnpackInto(Buffer buffer, ref Color32 value)
        {
            Unpack(buffer, out value);
        }
    }
    #endregion

    [RePackerWrapper(typeof(UnityEngine.Transform))]
    public class TransformPacker : RePackerWrapper<UnityEngine.Transform>
    {
        public override void Pack(Buffer buffer, ref UnityEngine.Transform value)
        {
            var pos = value.localPosition;
            Pack<Vector3>(buffer, ref pos);
            var rot = value.localRotation;
            Pack<Quaternion>(buffer, ref rot);
            var scale = value.localScale;
            Pack<Vector3>(buffer, ref scale);
        }

        public override void UnpackInto(Buffer buffer, ref Transform value)
        {
            value.localPosition = Unpack<Vector3>(buffer);
            value.localRotation = Unpack<Quaternion>(buffer);
            value.localScale = Unpack<Vector3>(buffer);
        }
    }

}

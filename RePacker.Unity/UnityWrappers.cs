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
        public override void Pack(ReBuffer buffer, ref Quaternion value)
        {
            buffer.PackFloat(ref value.x);
            buffer.PackFloat(ref value.y);
            buffer.PackFloat(ref value.z);
            buffer.PackFloat(ref value.w);
        }

        public override void Unpack(ReBuffer buffer, out Quaternion value)
        {
            buffer.UnpackFloat(out value.x);
            buffer.UnpackFloat(out value.y);
            buffer.UnpackFloat(out value.z);
            buffer.UnpackFloat(out value.w);
        }

        public override void UnpackInto(ReBuffer buffer, ref Quaternion value)
        {
            Unpack(buffer, out value);
        }
    }

    #region AnimationCurve
    [RePackerWrapper(typeof(Keyframe))]
    public class KeyframePacker : RePackerWrapper<Keyframe>
    {
        public override void Pack(ReBuffer buffer, ref Keyframe value)
        {
            var time = value.time;
            var val = value.value;
            var inTangent = value.inTangent;
            var outTangent = value.outTangent;
            var inWeight = value.inWeight;
            var outWeight = value.outWeight;
            var weightedMode = value.weightedMode;

            buffer.PackFloat(ref time);
            buffer.PackFloat(ref val);
            buffer.PackFloat(ref inTangent);
            buffer.PackFloat(ref outTangent);
            buffer.PackFloat(ref inWeight);
            buffer.PackFloat(ref outWeight);
            buffer.PackEnum(ref weightedMode);
        }

        public override void Unpack(ReBuffer buffer, out Keyframe value)
        {
            value = new Keyframe();

            var time = 0f;
            var val = 0f;
            var inTangent = 0f;
            var outTangent = 0f;
            var inWeight = 0f;
            var outWeight = 0f;

            buffer.UnpackFloat(out time);
            buffer.UnpackFloat(out val);
            buffer.UnpackFloat(out inTangent);
            buffer.UnpackFloat(out outTangent);
            buffer.UnpackFloat(out inWeight);
            buffer.UnpackFloat(out outWeight);
            value.weightedMode = buffer.UnpackEnum<WeightedMode>();
        }

        public override void UnpackInto(ReBuffer buffer, ref Keyframe value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(AnimationCurve))]
    public class AnimationCurvePacker : RePackerWrapper<AnimationCurve>
    {
        public override void Pack(ReBuffer buffer, ref AnimationCurve value)
        {
            var preMode = value.preWrapMode;
            var postMode = value.postWrapMode;
            var keys = value.keys;

            buffer.PackEnum(ref preMode);
            buffer.PackEnum(ref postMode);
            RePacking.Pack(buffer, ref keys);
        }

        public override void Unpack(ReBuffer buffer, out AnimationCurve value)
        {
            var preMode = buffer.UnpackEnum<WrapMode>();
            var postMode = buffer.UnpackEnum<WrapMode>();
            var keys = RePacking.Unpack<Keyframe[]>(buffer);

            value = new AnimationCurve(keys);
            value.preWrapMode = preMode;
            value.postWrapMode = postMode;
        }

        public override void UnpackInto(ReBuffer buffer, ref AnimationCurve value)
        {
            Unpack(buffer, out value);
        }
    }
    #endregion

    #region Vector
    [RePackerWrapper(typeof(Vector4))]
    public class Vector4Packer : RePackerWrapper<Vector4>
    {
        public override void Pack(ReBuffer buffer, ref Vector4 value)
        {
            buffer.PackFloat(ref value.x);
            buffer.PackFloat(ref value.y);
            buffer.PackFloat(ref value.z);
            buffer.PackFloat(ref value.w);
        }

        public override void Unpack(ReBuffer buffer, out Vector4 value)
        {
            buffer.UnpackFloat(out value.x);
            buffer.UnpackFloat(out value.y);
            buffer.UnpackFloat(out value.z);
            buffer.UnpackFloat(out value.w);
        }

        public override void UnpackInto(ReBuffer buffer, ref Vector4 value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(Vector3))]
    public class Vector3Packer : RePackerWrapper<Vector3>
    {
        public override void Pack(ReBuffer buffer, ref Vector3 value)
        {
            buffer.PackFloat(ref value.x);
            buffer.PackFloat(ref value.y);
            buffer.PackFloat(ref value.z);
        }

        public override void Unpack(ReBuffer buffer, out Vector3 value)
        {
            buffer.UnpackFloat(out value.x);
            buffer.UnpackFloat(out value.y);
            buffer.UnpackFloat(out value.z);
        }

        public override void UnpackInto(ReBuffer buffer, ref Vector3 value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(Vector2))]
    public class Vector2Packer : RePackerWrapper<Vector2>
    {
        public override void Pack(ReBuffer buffer, ref Vector2 value)
        {
            buffer.PackFloat(ref value.x);
            buffer.PackFloat(ref value.y);
        }

        public override void Unpack(ReBuffer buffer, out Vector2 value)
        {
            buffer.UnpackFloat(out value.x);
            buffer.UnpackFloat(out value.y);
        }

        public override void UnpackInto(ReBuffer buffer, ref Vector2 value)
        {
            Unpack(buffer, out value);
        }
    }
    #endregion

    #region VectorInt
    [RePackerWrapper(typeof(Vector3Int))]
    public class Vector3IntPacker : RePackerWrapper<Vector3Int>
    {
        public override void Pack(ReBuffer buffer, ref Vector3Int value)
        {
            int vx = value.x;
            int vy = value.y;
            int vz = value.z;
            buffer.PackInt(ref vx);
            buffer.PackInt(ref vy);
            buffer.PackInt(ref vz);
        }

        public override void Unpack(ReBuffer buffer, out Vector3Int value)
        {
            buffer.UnpackInt(out int vx);
            buffer.UnpackInt(out int vy);
            buffer.UnpackInt(out int vz);

            value = new Vector3Int(vx, vy, vz);
        }

        public override void UnpackInto(ReBuffer buffer, ref Vector3Int value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(Vector2Int))]
    public class Vector2IntPacker : RePackerWrapper<Vector2Int>
    {
        public override void Pack(ReBuffer buffer, ref Vector2Int value)
        {
            int vx = value.x;
            int vy = value.y;
            buffer.PackInt(ref vx);
            buffer.PackInt(ref vy);
        }

        public override void Unpack(ReBuffer buffer, out Vector2Int value)
        {
            buffer.UnpackInt(out int vx);
            buffer.UnpackInt(out int vy);

            value = new Vector2Int(vx, vy);
        }

        public override void UnpackInto(ReBuffer buffer, ref Vector2Int value)
        {
            Unpack(buffer, out value);
        }
    }
    #endregion

    #region Color
    [RePackerWrapper(typeof(Color))]
    public class ColorPacker : RePackerWrapper<Color>
    {
        public override void Pack(ReBuffer buffer, ref Color value)
        {
            buffer.PackFloat(ref value.r);
            buffer.PackFloat(ref value.g);
            buffer.PackFloat(ref value.b);
            buffer.PackFloat(ref value.a);
        }

        public override void Unpack(ReBuffer buffer, out Color value)
        {
            buffer.UnpackFloat(out value.r);
            buffer.UnpackFloat(out value.g);
            buffer.UnpackFloat(out value.b);
            buffer.UnpackFloat(out value.a);
        }

        public override void UnpackInto(ReBuffer buffer, ref Color value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(Color32))]
    public class Color32Packer : RePackerWrapper<Color32>
    {
        public override void Pack(ReBuffer buffer, ref Color32 value)
        {
            buffer.PackByte(ref value.r);
            buffer.PackByte(ref value.g);
            buffer.PackByte(ref value.b);
            buffer.PackByte(ref value.a);
        }

        public override void Unpack(ReBuffer buffer, out Color32 value)
        {
            value = new Color32();
            buffer.UnpackByte(out value.r);
            buffer.UnpackByte(out value.g);
            buffer.UnpackByte(out value.b);
            buffer.UnpackByte(out value.a);
        }

        public override void UnpackInto(ReBuffer buffer, ref Color32 value)
        {
            Unpack(buffer, out value);
        }
    }
    #endregion

    [RePackerWrapper(typeof(UnityEngine.Transform))]
    public class TransformPacker : RePackerWrapper<UnityEngine.Transform>
    {
        public override void Pack(ReBuffer buffer, ref UnityEngine.Transform value)
        {
            var pos = value.localPosition;
            Pack<Vector3>(buffer, ref pos);
            var rot = value.localRotation;
            Pack<Quaternion>(buffer, ref rot);
            var scale = value.localScale;
            Pack<Vector3>(buffer, ref scale);
        }

        public override void UnpackInto(ReBuffer buffer, ref Transform value)
        {
            value.localPosition = Unpack<Vector3>(buffer);
            value.localRotation = Unpack<Quaternion>(buffer);
            value.localScale = Unpack<Vector3>(buffer);
        }
    }


    [RePackerWrapper(typeof(UnityEngine.RectInt))]
    public class RectIntPacker : RePackerWrapper<UnityEngine.RectInt>
    {
        public override void Pack(ReBuffer buffer, ref RectInt value)
        {
            var pos = value.position;
            var size = value.size;

            buffer.Pack(ref pos);
            buffer.Pack(ref size);
        }

        public override void Unpack(ReBuffer buffer, out RectInt value)
        {
            buffer.Unpack<Vector2Int>(out var pos);
            buffer.Unpack<Vector2Int>(out var size);

            value = new RectInt(pos, size);
        }

        public override void UnpackInto(ReBuffer buffer, ref RectInt value)
        {
            Unpack(buffer, out value);
        }

        public override int SizeOf(ref RectInt value)
        {
            return 4 * 4;
        }
    }

    [RePackerWrapper(typeof(UnityEngine.Rect))]
    public class RectPacker : RePackerWrapper<UnityEngine.Rect>
    {
        public override void Pack(ReBuffer buffer, ref Rect value)
        {
            var pos = value.position;
            var size = value.size;

            buffer.Pack(ref pos);
            buffer.Pack(ref size);
        }

        public override void Unpack(ReBuffer buffer, out Rect value)
        {
            buffer.Unpack<Vector2Int>(out var pos);
            buffer.Unpack<Vector2Int>(out var size);

            value = new Rect(pos, size);
        }

        public override void UnpackInto(ReBuffer buffer, ref Rect value)
        {
            Unpack(buffer, out value);
        }

        public override int SizeOf(ref Rect value)
        {
            return 4 * 4;
        }
    }
}

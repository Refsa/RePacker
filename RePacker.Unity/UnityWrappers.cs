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
            buffer.Pack(ref value.x);
            buffer.Pack(ref value.y);
            buffer.Pack(ref value.z);
            buffer.Pack(ref value.w);
        }

        public override void Unpack(ReBuffer buffer, out Quaternion value)
        {
            buffer.Unpack(out value.x);
            buffer.Unpack(out value.y);
            buffer.Unpack(out value.z);
            buffer.Unpack(out value.w);
        }

        public override void UnpackInto(ReBuffer buffer, ref Quaternion value)
        {
            Unpack(buffer, out value);
        }
    }

    [RePackerWrapper(typeof(UnityEngine.Matrix4x4))]
    public class Matrix4x4Packer : RePackerWrapper<UnityEngine.Matrix4x4>
    {
        public override void Pack(ReBuffer buffer, ref Matrix4x4 value)
        {
            var row0 = value.GetRow(0);
            var row1 = value.GetRow(1);
            var row2 = value.GetRow(2);
            var row3 = value.GetRow(3);

            buffer.Pack<Vector4>(ref row0);
            buffer.Pack<Vector4>(ref row1);
            buffer.Pack<Vector4>(ref row2);
            buffer.Pack<Vector4>(ref row3);
        }

        public override void Unpack(ReBuffer buffer, out Matrix4x4 value)
        {
            value = new Matrix4x4();
            UnpackInto(buffer, ref value);
        }

        public override void UnpackInto(ReBuffer buffer, ref Matrix4x4 value)
        {
            buffer.Unpack<Vector4>(out var row0);
            buffer.Unpack<Vector4>(out var row1);
            buffer.Unpack<Vector4>(out var row2);
            buffer.Unpack<Vector4>(out var row3);

            value.SetRow(0, row0);
            value.SetRow(1, row1);
            value.SetRow(2, row2);
            value.SetRow(3, row3);
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

            buffer.Pack(ref time);
            buffer.Pack(ref val);
            buffer.Pack(ref inTangent);
            buffer.Pack(ref outTangent);
            buffer.Pack(ref inWeight);
            buffer.Pack(ref outWeight);
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

            buffer.Unpack(out time);
            buffer.Unpack(out val);
            buffer.Unpack(out inTangent);
            buffer.Unpack(out outTangent);
            buffer.Unpack(out inWeight);
            buffer.Unpack(out outWeight);
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

            buffer.Pack(ref preMode);
            buffer.Pack(ref postMode);
            RePacking.Pack(buffer, ref keys);
        }

        public override void Unpack(ReBuffer buffer, out AnimationCurve value)
        {
            buffer.Unpack<WrapMode>(out var preMode);
            buffer.Unpack<WrapMode>(out var postMode);
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
            buffer.Pack(ref value.x);
            buffer.Pack(ref value.y);
            buffer.Pack(ref value.z);
            buffer.Pack(ref value.w);
        }

        public override void Unpack(ReBuffer buffer, out Vector4 value)
        {
            buffer.Unpack(out value.x);
            buffer.Unpack(out value.y);
            buffer.Unpack(out value.z);
            buffer.Unpack(out value.w);
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
            buffer.Pack(ref value.x);
            buffer.Pack(ref value.y);
            buffer.Pack(ref value.z);
        }

        public override void Unpack(ReBuffer buffer, out Vector3 value)
        {
            buffer.Unpack(out value.x);
            buffer.Unpack(out value.y);
            buffer.Unpack(out value.z);
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
            buffer.Pack(ref value.x);
            buffer.Pack(ref value.y);
        }

        public override void Unpack(ReBuffer buffer, out Vector2 value)
        {
            buffer.Unpack(out value.x);
            buffer.Unpack(out value.y);
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
            buffer.Pack(ref vx);
            buffer.Pack(ref vy);
            buffer.Pack(ref vz);
        }

        public override void Unpack(ReBuffer buffer, out Vector3Int value)
        {
            buffer.Unpack(out int vx);
            buffer.Unpack(out int vy);
            buffer.Unpack(out int vz);

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
            buffer.Pack(ref vx);
            buffer.Pack(ref vy);
        }

        public override void Unpack(ReBuffer buffer, out Vector2Int value)
        {
            buffer.Unpack(out int vx);
            buffer.Unpack(out int vy);

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
            buffer.Pack(ref value.r);
            buffer.Pack(ref value.g);
            buffer.Pack(ref value.b);
            buffer.Pack(ref value.a);
        }

        public override void Unpack(ReBuffer buffer, out Color value)
        {
            buffer.Unpack(out value.r);
            buffer.Unpack(out value.g);
            buffer.Unpack(out value.b);
            buffer.Unpack(out value.a);
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
            buffer.Pack(ref value.r);
            buffer.Pack(ref value.g);
            buffer.Pack(ref value.b);
            buffer.Pack(ref value.a);
        }

        public override void Unpack(ReBuffer buffer, out Color32 value)
        {
            value = new Color32();
            buffer.Unpack(out value.r);
            buffer.Unpack(out value.g);
            buffer.Unpack(out value.b);
            buffer.Unpack(out value.a);
        }

        public override void UnpackInto(ReBuffer buffer, ref Color32 value)
        {
            Unpack(buffer, out value);
        }
    }
    #endregion

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

    [RePackerWrapper(typeof(UnityEngine.Bounds))]
    public class BoundsPacker : RePackerWrapper<UnityEngine.Bounds>
    {
        public override void Pack(ReBuffer buffer, ref Bounds value)
        {
            var center = value.center;
            var extents = value.extents;

            buffer.Pack<Vector3>(ref center);
            buffer.Pack<Vector3>(ref extents);
        }

        public override void Unpack(ReBuffer buffer, out Bounds value)
        {
            value = new Bounds();
            UnpackInto(buffer, ref value);
        }

        public override void UnpackInto(ReBuffer buffer, ref Bounds value)
        {
            buffer.Unpack<Vector3>(out var center);
            buffer.Unpack<Vector3>(out var extents);

            value.center = center;
            value.extents = extents;
        }
    }

    [RePackerWrapper(typeof(UnityEngine.BoundsInt))]
    public class BoundsIntPacker : RePackerWrapper<UnityEngine.BoundsInt>
    {
        public override void Pack(ReBuffer buffer, ref BoundsInt value)
        {
            var position = value.position;
            var extents = value.size;

            buffer.Pack<Vector3Int>(ref position);
            buffer.Pack<Vector3Int>(ref extents);
        }

        public override void Unpack(ReBuffer buffer, out BoundsInt value)
        {
            value = new BoundsInt();
            UnpackInto(buffer, ref value);
        }

        public override void UnpackInto(ReBuffer buffer, ref BoundsInt value)
        {
            buffer.Unpack<Vector3Int>(out var position);
            buffer.Unpack<Vector3Int>(out var size);

            value.position = position;
            value.size = size;
        }
    }

    #region Components
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

        public override void Unpack(ReBuffer buffer, out Transform value)
        {
            throw new System.NotSupportedException("UnityEngine MonoBehaviours cant be constructed, use UnpackInto");
        }

        public override void UnpackInto(ReBuffer buffer, ref Transform value)
        {
            value.localPosition = Unpack<Vector3>(buffer);
            value.localRotation = Unpack<Quaternion>(buffer);
            value.localScale = Unpack<Vector3>(buffer);
        }
    }

    [RePackerWrapper(typeof(UnityEngine.Rigidbody))]
    public class RigidbodyPacker : RePackerWrapper<UnityEngine.Rigidbody>
    {
        public override void Pack(ReBuffer buffer, ref Rigidbody value)
        {
            var position = value.position;
            var rotation = value.rotation;
            var velocity = value.velocity;
            var angularVelocity = value.angularVelocity;

            buffer.Pack<Vector3>(ref position);
            buffer.Pack<Quaternion>(ref rotation);
            buffer.Pack<Vector3>(ref velocity);
            buffer.Pack<Vector3>(ref angularVelocity);
        }

        public override void Unpack(ReBuffer buffer, out Rigidbody value)
        {
            throw new System.NotSupportedException("UnityEngine MonoBehaviours cant be constructed, use UnpackInto");
        }

        public override void UnpackInto(ReBuffer buffer, ref Rigidbody value)
        {
            buffer.Unpack<Vector3>(out var position);
            buffer.Unpack<Quaternion>(out var rotation);
            buffer.Unpack<Vector3>(out var velocity);
            buffer.Unpack<Vector3>(out var angularVelocity);

            value.position = position;
            value.rotation = rotation;
            value.velocity = velocity;
            value.angularVelocity = angularVelocity;
        }
    }

    [RePackerWrapper(typeof(UnityEngine.Rigidbody2D))]
    public class Rigidbody2DPacker : RePackerWrapper<UnityEngine.Rigidbody2D>
    {
        public override void Pack(ReBuffer buffer, ref Rigidbody2D value)
        {
            var position = value.position;
            var rotation = value.rotation;
            var velocity = value.velocity;
            var angularVelocity = value.angularVelocity;

            buffer.Pack<Vector2>(ref position);
            buffer.Pack<float>(ref rotation);
            buffer.Pack<Vector2>(ref velocity);
            buffer.Pack<float>(ref angularVelocity);
        }

        public override void Unpack(ReBuffer buffer, out Rigidbody2D value)
        {
            throw new System.NotSupportedException("UnityEngine MonoBehaviours cant be constructed, use UnpackInto");
        }

        public override void UnpackInto(ReBuffer buffer, ref Rigidbody2D value)
        {
            buffer.Unpack<Vector2>(out var position);
            buffer.Unpack<float>(out var rotation);
            buffer.Unpack<Vector2>(out var velocity);
            buffer.Unpack<float>(out var angularVelocity);

            value.position = position;
            value.rotation = rotation;
            value.velocity = velocity;
            value.angularVelocity = angularVelocity;
        }
    }
    #endregion
}

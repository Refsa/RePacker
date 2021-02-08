using Refsa.RePacker.Buffers;
using UnityEngine;

using static Refsa.RePacker.RePacker;

namespace Refsa.RePacker.Unity
{
    public static class UnityExtensions
    {
        public static void Pack(this Transform self, BoxedBuffer buffer)
        {
            RePacker.Pack(buffer, ref self);
        }

        public static void Pack(this ref Vector3 self, BoxedBuffer buffer)
        {
            RePacker.Pack(buffer, ref self);
        }

        public static void Pack(this ref Quaternion self, BoxedBuffer buffer)
        {
            RePacker.Pack(buffer, ref self);
        }

        public static void Unpack(this Transform self, BoxedBuffer buffer)
        {
            RePacker.UnpackInto(buffer, ref self);
        }

        public static void Unpack(this Vector3 self, BoxedBuffer buffer)
        {
            RePacker.UnpackInto(buffer, ref self);
        }

        public static void Unpack(this Quaternion self, BoxedBuffer buffer)
        {
            RePacker.UnpackInto(buffer, ref self);
        }
    }
}
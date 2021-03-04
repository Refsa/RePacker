using RePacker.Buffers;
using UnityEngine;
using static RePacker.RePacking;

namespace RePacker.Unity
{
    public static class UnityExtensions
    {
        public static void Pack(this Transform self, Buffer buffer)
        {
            RePacking.Pack(buffer, ref self);
        }

        public static void Pack(this ref Vector3 self, Buffer buffer)
        {
            RePacking.Pack(buffer, ref self);
        }

        public static void Pack(this ref Quaternion self, Buffer buffer)
        {
            RePacking.Pack(buffer, ref self);
        }

        public static void Unpack(this Transform self, Buffer buffer)
        {
            RePacking.UnpackInto(buffer, ref self);
        }

        public static void Unpack(this Vector3 self, Buffer buffer)
        {
            RePacking.UnpackInto(buffer, ref self);
        }

        public static void Unpack(this Quaternion self, Buffer buffer)
        {
            RePacking.UnpackInto(buffer, ref self);
        }
    }
}
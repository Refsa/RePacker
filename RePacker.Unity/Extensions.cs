using RePacker.Buffers;
using UnityEngine;
using static RePacker.RePacking;

namespace RePacker.Unity
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Packs the transform into given buffer
        /// </summary>
        /// <param name="self"></param>
        /// <param name="buffer"></param>
        public static void Pack(this Transform self, ReBuffer buffer)
        {
            RePacking.Pack(buffer, ref self);
        }

        /// <summary>
        /// Packs the Vector3 into given buffer
        /// </summary>
        /// <param name="self"></param>
        /// <param name="buffer"></param>
        public static void Pack(this ref Vector3 self, ReBuffer buffer)
        {
            RePacking.Pack(buffer, ref self);
        }

        /// <summary>
        /// Packs the quaternion into buffer
        /// </summary>
        /// <param name="self"></param>
        /// <param name="buffer"></param>
        public static void Pack(this ref Quaternion self, ReBuffer buffer)
        {
            RePacking.Pack(buffer, ref self);
        }

        /// <summary>
        /// Unpacks and sets values on Transform from given buffer
        /// </summary>
        /// <param name="self"></param>
        /// <param name="buffer"></param>
        public static void Unpack(this Transform self, ReBuffer buffer)
        {
            RePacking.UnpackInto(buffer, ref self);
        }

        /// <summary>
        /// Unpacks and sets values on Vector3 from given buffer
        /// </summary>
        /// <param name="self"></param>
        /// <param name="buffer"></param>
        public static void Unpack(this Vector3 self, ReBuffer buffer)
        {
            RePacking.UnpackInto(buffer, ref self);
        }

        /// <summary>
        /// Unpacks and sets values on Quaternion from given buffer
        /// </summary>
        /// <param name="self"></param>
        /// <param name="buffer"></param>
        public static void Unpack(this Quaternion self, ReBuffer buffer)
        {
            RePacking.UnpackInto(buffer, ref self);
        }
    }
}
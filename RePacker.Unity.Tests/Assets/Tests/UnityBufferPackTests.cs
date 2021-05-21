using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using RePacker;
using RePacker.Buffers;
using RePacker.Unity;

namespace Tests
{
    public class UnityBufferPackTests
    {
        ReBuffer buffer = new ReBuffer(1024);

        [Test]
        public void direct_quaternion_packing()
        {
            buffer.Reset();

            Quaternion quat = Quaternion.AngleAxis(45f, Vector3.right);
            buffer.Pack(ref quat);

            buffer.Unpack(out Quaternion from);
            Assert.AreEqual(quat, from);
            Assert.AreEqual(quat.eulerAngles, from.eulerAngles);
        }

        [Test]
        public void direct_vector2_packing()
        {
            buffer.Reset();

            Vector2 vec = new Vector2(1.2f, 2.323423f);
            buffer.Pack(ref vec);

            buffer.Unpack(out Vector2 from);
            Assert.AreEqual(vec, from);
        }

        [Test]
        public void direct_vector3_packing()
        {
            buffer.Reset();

            Vector3 vec = new Vector3(1.2f, 2.3f, 4.345235235f);
            buffer.Pack(ref vec);

            buffer.Unpack(out Vector3 from);
            Assert.AreEqual(vec, from);
        }

        [Test]
        public void direct_vector4_packing()
        {
            buffer.Reset();

            Vector4 vec = new Vector4(1.2f, 2.3f, 4.345235235f, 342.1234754f);
            buffer.Pack(ref vec);

            buffer.Unpack(out Vector4 from);
            Assert.AreEqual(vec, from);
        }

        [Test]
        public void direct_vector2int_packing()
        {
            buffer.Reset();

            Vector2Int vec = new Vector2Int(123, 34262345);
            buffer.Pack(ref vec);

            buffer.Unpack(out Vector2Int from);
            Assert.AreEqual(vec, from);
        }

        [Test]
        public void direct_vector3int_packing()
        {
            buffer.Reset();

            Vector3Int vec = new Vector3Int(123, 34262345, 934562345);
            buffer.Pack(ref vec);

            buffer.Unpack(out Vector3Int from);
            Assert.AreEqual(vec, from);
        }

        [Test]
        public void direct_matrix4x4_packing()
        {
            buffer.Reset();

            Matrix4x4 mat = Matrix4x4.TRS(Vector3.one * 12345, Quaternion.AngleAxis(45f, Vector3.right), Vector3.one * 23);
            buffer.Pack(ref mat);

            buffer.Unpack(out Matrix4x4 from);

            Assert.AreEqual(mat, from);
        }

        [Test]
        public void direct_color_packing()
        {
            buffer.Reset();

            Color color = new Color(0.2f, 0.3f, 0.4f, 0.5f);
            buffer.Pack(ref color);

            buffer.Unpack(out Color from);

            Assert.AreEqual(color, from);
        }

        [Test]
        public void direct_color32_packing()
        {
            buffer.Reset();

            Color32 color = new Color32(10, 50, 122, 240);
            buffer.Pack(ref color);

            buffer.Unpack(out Color32 from);
            Assert.AreEqual(color, from);
        }

        [Test]
        public void direct_rectInt_packing()
        {
            buffer.Reset();

            RectInt rect = new RectInt(10, 20, 255, 562436);
            buffer.Pack(ref rect);

            buffer.Unpack(out RectInt from);
            Assert.AreEqual(rect, from);
        }

        [Test]
        public void direct_rect_packing()
        {
            buffer.Reset();

            Rect rect = new Rect(10.23f, 20.3453f, 255.345346f, 562436.54345f);
            buffer.Pack(ref rect);

            buffer.Unpack(out Rect from);
            Assert.AreEqual(rect, from);
        }

        [Test]
        public void direct_bounds_packing()
        {
            buffer.Reset();

            Bounds bounds = new Bounds(Vector3.one * 325.234f, Vector3.one * 5634.345f);
            buffer.Pack(ref bounds);

            buffer.Unpack(out Bounds from);
            Assert.AreEqual(bounds, from);
        }

        [Test]
        public void direct_boundsInt_packing()
        {
            buffer.Reset();

            BoundsInt bounds = new BoundsInt(Vector3Int.one * 325, Vector3Int.one * 5634);
            buffer.Pack(ref bounds);

            buffer.Unpack(out BoundsInt from);
            Assert.AreEqual(bounds, from);
        }
    }
}

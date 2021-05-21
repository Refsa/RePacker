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
    public class UnityWrapperTests
    {
        ReBuffer buffer = new ReBuffer(1024);

        [Test]
        public void wrapper_quaternion_packing()
        {
            buffer.Reset();

            Quaternion quat = Quaternion.AngleAxis(45f, Vector3.right);
            RePacking.Pack(buffer, ref quat);

            Quaternion from = RePacking.Unpack<Quaternion>(buffer);
            Assert.AreEqual(quat, from);
            Assert.AreEqual(quat.eulerAngles, from.eulerAngles);
        }

        [Test]
        public void wrapper_vector2_packing()
        {
            buffer.Reset();

            Vector2 vec = new Vector2(1.2f, 2.323423f);
            RePacking.Pack(buffer, ref vec);

            Vector2 from = RePacking.Unpack<Vector2>(buffer);
            Assert.AreEqual(vec, from);
        }

        [Test]
        public void wrapper_vector3_packing()
        {
            buffer.Reset();

            Vector3 vec = new Vector3(1.2f, 2.3f, 4.345235235f);
            RePacking.Pack(buffer, ref vec);

            Vector3 from = RePacking.Unpack<Vector3>(buffer);
            Assert.AreEqual(vec, from);
        }

        [Test]
        public void wrapper_vector4_packing()
        {
            buffer.Reset();

            Vector4 vec = new Vector4(1.2f, 2.3f, 4.345235235f, 342.1234754f);
            RePacking.Pack(buffer, ref vec);

            Vector4 from = RePacking.Unpack<Vector4>(buffer);
            Assert.AreEqual(vec, from);
        }

        [Test]
        public void wrapper_vector2int_packing()
        {
            buffer.Reset();

            Vector2Int vec = new Vector2Int(123, 34262345);
            RePacking.Pack(buffer, ref vec);

            Vector2Int from = RePacking.Unpack<Vector2Int>(buffer);
            Assert.AreEqual(vec, from);
        }

        [Test]
        public void wrapper_vector3int_packing()
        {
            buffer.Reset();

            Vector3Int vec = new Vector3Int(123, 34262345, 934562345);
            RePacking.Pack(buffer, ref vec);

            Vector3Int from = RePacking.Unpack<Vector3Int>(buffer);
            Assert.AreEqual(vec, from);
        }

        [Test]
        public void wrapper_matrix4x4_packing()
        {
            buffer.Reset();

            Matrix4x4 mat = Matrix4x4.TRS(Vector3.one * 12345, Quaternion.AngleAxis(45f, Vector3.right), Vector3.one * 23);
            RePacking.Pack(buffer, ref mat);

            Matrix4x4 from = RePacking.Unpack<Matrix4x4>(buffer);

            Assert.AreEqual(mat, from);
        }

        [Test]
        public void wrapper_color_packing()
        {
            buffer.Reset();

            Color color = new Color(0.2f, 0.3f, 0.4f, 0.5f);
            RePacking.Pack(buffer, ref color);

            Color from = RePacking.Unpack<Color>(buffer);

            Assert.AreEqual(color, from);
        }

        [Test]
        public void wrapper_color32_packing()
        {
            buffer.Reset();

            Color32 color = new Color32(10, 50, 122, 240);
            RePacking.Pack(buffer, ref color);

            Color32 from = RePacking.Unpack<Color32>(buffer);
            Assert.AreEqual(color, from);
        }

        [Test]
        public void wrapper_rectInt_packing()
        {
            buffer.Reset();

            RectInt rect = new RectInt(10, 20, 255, 562436);
            RePacking.Pack(buffer, ref rect);

            RectInt from = RePacking.Unpack<RectInt>(buffer);
            Assert.AreEqual(rect, from);
        }

        [Test]
        public void wrapper_rect_packing()
        {
            buffer.Reset();

            Rect rect = new Rect(10.23f, 20.3453f, 255.345346f, 562436.54345f);
            RePacking.Pack(buffer, ref rect);

            Rect from = RePacking.Unpack<Rect>(buffer);
            Assert.AreEqual(rect, from);
        }

        [Test]
        public void wrapper_bounds_packing()
        {
            buffer.Reset();

            Bounds bounds = new Bounds(Vector3.one * 325.234f, Vector3.one * 5634.345f);
            RePacking.Pack(buffer, ref bounds);

            Bounds from = RePacking.Unpack<Bounds>(buffer);
            Assert.AreEqual(bounds, from);
        }

        [Test]
        public void wrapper_boundsInt_packing()
        {
            buffer.Reset();

            BoundsInt bounds = new BoundsInt(Vector3Int.one * 325, Vector3Int.one * 5634);
            RePacking.Pack(buffer, ref bounds);

            BoundsInt from = RePacking.Unpack<BoundsInt>(buffer);
            Assert.AreEqual(bounds, from);
        }
    }
}

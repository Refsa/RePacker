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
    }
}

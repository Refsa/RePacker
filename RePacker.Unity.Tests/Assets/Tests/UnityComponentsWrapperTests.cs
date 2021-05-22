using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using RePacker;
using RePacker.Buffers;

namespace Tests
{
    public class UnityComponentsWrapperTests
    {
        ReBuffer buffer = new ReBuffer(1024);

        GameObject MakeGameObject(string name, params System.Type[] components)
        {
            var go = new GameObject(name);

            foreach (var component in components)
            {
                go.AddComponent(component);
            }

            return go;
        }

        [UnityTest]
        public IEnumerator transform_wrapper()
        {
            buffer.Reset();

            var go = new GameObject("Pack Transform");
            go.transform.position = Vector3.one * 1234.44f;
            go.transform.rotation = Quaternion.Euler(45f, 45f, 45f);
            yield return null;

            var transform = go.transform;
            RePacking.Pack(buffer, ref transform);

            go.transform.position += Vector3.one * 1234f;
            go.transform.rotation *= Quaternion.Euler(45f, 45f, 45f);
            yield return null;

            RePacking.UnpackInto(buffer, ref transform);
            yield return null;

            Assert.AreEqual(Vector3.one * 1234.44f, transform.position);
            Assert.AreEqual(Quaternion.Euler(45f, 45f, 45f), transform.rotation);
        }

        [UnityTest]
        public IEnumerator transform_wrapper_throws()
        {
            buffer.Reset();

            var go = new GameObject("Pack Transform");
            go.transform.position = Vector3.one * 1234.1235f;
            go.transform.rotation = Quaternion.Euler(45.456f, 45.456f, 45.456f);
            yield return null;

            var transform = go.transform;
            RePacking.Pack(buffer, ref transform);

            Assert.Throws<System.NotSupportedException>(() =>
            {
                Transform from = RePacking.Unpack<Transform>(buffer);
            });
        }

        [UnityTest]
        public IEnumerator rigidbody_wrapper()
        {
            buffer.Reset();

            var go = MakeGameObject("rigidbody", typeof(Rigidbody));
            var rb = go.GetComponent<Rigidbody>();

            rb.position = Vector3.one * 234.234f;
            rb.rotation = Quaternion.Euler(45f, 45f, 45f);
            rb.velocity = Vector3.one * 2998.346f;
            rb.angularVelocity = Vector3.one * 90890.34f;
            yield return null;

            RePacking.Pack(buffer, ref rb);

            rb.position += Vector3.one * 234.234f;
            rb.rotation *= Quaternion.Euler(45f, 45f, 45f);
            rb.velocity += Vector3.one * 2998.346f;
            rb.angularVelocity += Vector3.one * 90890.34f;
            yield return null;

            RePacking.UnpackInto(buffer, ref rb);

            Assert.AreEqual(Vector3.one * 234.234f, rb.position);
            Assert.AreEqual(Quaternion.Euler(45f, 45f, 45f), rb.rotation);
            Assert.AreEqual(Vector3.one * 2998.346f, rb.velocity);
            Assert.AreEqual(Vector3.one * 90890.34f, rb.angularVelocity);

            yield return null;
        }

        [UnityTest]
        public IEnumerator rigidbody_wrapper_throws()
        {
            buffer.Reset();

            var go = MakeGameObject("rigidbody", typeof(Rigidbody));
            var rb = go.GetComponent<Rigidbody>();

            rb.position = Vector3.one * 234.234f;
            rb.rotation = Quaternion.Euler(45f, 45f, 45f);
            rb.velocity = Vector3.one * 2998.346f;
            rb.angularVelocity = Vector3.one * 90890.34f;
            yield return null;

            RePacking.Pack(buffer, ref rb);
            yield return null;

            Assert.Throws<System.NotSupportedException>(() =>
            {
                rb = RePacking.Unpack<Rigidbody>(buffer);
            });

            yield return null;
        }

        [UnityTest]
        public IEnumerator rigidbody2d_wrapper()
        {
            buffer.Reset();

            var go = MakeGameObject("rigibody2d", typeof(Rigidbody2D));
            var rb2d = go.GetComponent<Rigidbody2D>();

            rb2d.position = Vector2.one * 234.456f;
            rb2d.rotation = 34.346f;
            rb2d.velocity = Vector2.one * 908.345f;
            rb2d.angularVelocity = 86.5434f;
            yield return null;

            RePacking.Pack(buffer, ref rb2d);
            yield return null;

            rb2d.position += Vector2.one * 234.456f;
            rb2d.rotation += 34.346f;
            rb2d.velocity += Vector2.one * 908.345f;
            rb2d.angularVelocity += 86.5434f;
            yield return null;

            RePacking.UnpackInto(buffer, ref rb2d);

            Assert.AreEqual(Vector2.one * 234.456f, rb2d.position);
            Assert.AreEqual(34.346f, rb2d.rotation);
            Assert.AreEqual(Vector2.one * 908.345f, rb2d.velocity);
            Assert.AreEqual(86.5434f, rb2d.angularVelocity);
        }

        [UnityTest]
        public IEnumerator rigidbody2d_wrapper_throws()
        {
            buffer.Reset();

            var go = MakeGameObject("rigibody2d", typeof(Rigidbody2D));
            var rb2d = go.GetComponent<Rigidbody2D>();

            rb2d.position = Vector2.one * 234.456f;
            rb2d.rotation = 34.346f;
            rb2d.velocity = Vector2.one * 908.345f;
            rb2d.angularVelocity = 86.5434f;
            yield return null;

            RePacking.Pack(buffer, ref rb2d);
            yield return null;

            Assert.Throws<System.NotSupportedException>(() =>
            {
                rb2d = RePacking.Unpack<Rigidbody2D>(buffer);
            });
        }
    }
}
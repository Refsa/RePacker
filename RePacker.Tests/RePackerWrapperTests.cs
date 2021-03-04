using Xunit;
using RePacker.Buffers;
using RePacker.Builder;
using Xunit.Abstractions;

namespace RePacker.Tests
{
    public class RePackerWrapperTests
    {
        public RePackerWrapperTests(ITestOutputHelper output)
        {
            TestBootstrap.Setup(output);
        }

        public struct Vector3
        {
            public float X;
            public float Y;
            public float Z;
        }

        public class Transform
        {
            Vector3 position;
            Vector3 rotation;
            Vector3 scale;

            public Vector3 Position { get => position; set => position = value; }
            public Vector3 Rotation { get => rotation; set => rotation = value; }
            public Vector3 Scale { get => scale; set => scale = value; }
        }

        [RePackerWrapper(typeof(Vector3))]
        public class Vector3Packer : RePackerWrapper<Vector3>
        {
            public override void Pack(Buffer buffer, ref Vector3 value)
            {
                buffer.Pack(ref value.X);
                buffer.Pack(ref value.Y);
                buffer.Pack(ref value.Z);
            }

            public override void Unpack(Buffer buffer, out Vector3 value)
            {
                buffer.Unpack(out value.X);
                buffer.Unpack(out value.Y);
                buffer.Unpack(out value.Z);
            }
        }

        [RePackerWrapper(typeof(Transform))]
        public class TransformPacker : RePackerWrapper<Transform>
        {
            public override void Pack(Buffer buffer, ref Transform value)
            {
                Vector3 pos = value.Position;
                RePacking.Pack<Vector3>(buffer, ref pos);

                Vector3 rot = value.Rotation;
                RePacking.Pack<Vector3>(buffer, ref rot);

                Vector3 scale = value.Scale;
                RePacking.Pack<Vector3>(buffer, ref scale);
            }

            public override void UnpackInto(Buffer buffer, ref Transform value)
            {
                value.Position = RePacking.Unpack<Vector3>(buffer);
                value.Rotation = RePacking.Unpack<Vector3>(buffer);
                value.Scale = RePacking.Unpack<Vector3>(buffer);
            }
        }

        [Fact]
        public void simple_custom_wrapped_type()
        {
            Vector3 testVec3 = new Vector3 { X = 1.234f, Y = 4532.24f, Z = 943.342f };
            var buffer = new Buffer(1024);
            RePacking.Pack<Vector3>(buffer, ref testVec3);
            var fromBuf = RePacking.Unpack<Vector3>(buffer);

            Assert.Equal(testVec3.X, fromBuf.X);
            Assert.Equal(testVec3.Y, fromBuf.Y);
            Assert.Equal(testVec3.Z, fromBuf.Z);
        }

        [Fact]
        public void nested_custom_wrapped_type()
        {
            var testTransform = new Transform
            {
                Position = new Vector3 { X = 1.234f, Y = 4532.24f, Z = 943.342f },
                Rotation = new Vector3 { X = 1.234f, Y = 4532.24f, Z = 943.342f },
                Scale = new Vector3 { X = 1.234f, Y = 4532.24f, Z = 943.342f },
            };

            var buffer = new Buffer(1024);
            RePacking.Pack<Transform>(buffer, ref testTransform);

            var fromBuf = new Transform();
            RePacking.UnpackInto<Transform>(buffer, ref fromBuf);

            Assert.Equal(testTransform.Position.X, fromBuf.Position.X);
            Assert.Equal(testTransform.Position.Y, fromBuf.Position.Y);
            Assert.Equal(testTransform.Position.Z, fromBuf.Position.Z);

            Assert.Equal(testTransform.Rotation.X, fromBuf.Rotation.X);
            Assert.Equal(testTransform.Rotation.Y, fromBuf.Rotation.Y);
            Assert.Equal(testTransform.Rotation.Z, fromBuf.Rotation.Z);

            Assert.Equal(testTransform.Scale.X, fromBuf.Scale.X);
            Assert.Equal(testTransform.Scale.Y, fromBuf.Scale.Y);
            Assert.Equal(testTransform.Scale.Z, fromBuf.Scale.Z);
        }

        [Fact]
        public void unpack_into_existing_object()
        {
            var testTransform = new Transform
            {
                Position = new Vector3 { X = 1.234f, Y = 4532.24f, Z = 943.342f },
                Rotation = new Vector3 { X = 1.234f, Y = 4532.24f, Z = 943.342f },
                Scale = new Vector3 { X = 1.234f, Y = 4532.24f, Z = 943.342f },
            };

            var buffer = new Buffer(1024);
            RePacking.Pack<Transform>(buffer, ref testTransform);

            var targetTransform = new Transform();
            RePacking.UnpackInto<Transform>(buffer, ref targetTransform);

            Assert.Equal(testTransform.Position.X, targetTransform.Position.X);
            Assert.Equal(testTransform.Position.Y, targetTransform.Position.Y);
            Assert.Equal(testTransform.Position.Z, targetTransform.Position.Z);

            Assert.Equal(testTransform.Rotation.X, targetTransform.Rotation.X);
            Assert.Equal(testTransform.Rotation.Y, targetTransform.Rotation.Y);
            Assert.Equal(testTransform.Rotation.Z, targetTransform.Rotation.Z);

            Assert.Equal(testTransform.Scale.X, targetTransform.Scale.X);
            Assert.Equal(testTransform.Scale.Y, targetTransform.Scale.Y);
            Assert.Equal(testTransform.Scale.Z, targetTransform.Scale.Z);
        }
    }
}
using System.Linq;
using RePacker.Buffers;
using Xunit;

namespace RePacker.Tests
{
    public class StructPackerBenchmarkTests
    {
        static LargeValueMsg _largeValueMsg;

        public StructPackerBenchmarkTests()
        {
            GenerateLargeValueMessage();
        }

        [Fact]
        public void can_pack_large_value_msg()
        {
            var buffer = new ReBuffer(1 << 24);

            RePacking.Pack(buffer, ref _largeValueMsg);
            var fromBuf = RePacking.Unpack<LargeValueMsg>(buffer);

            for (int i = 0; i < _largeValueMsg.Prop1.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop1[i], fromBuf.Prop1[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop2.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop2[i], fromBuf.Prop2[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop3.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop3[i], fromBuf.Prop3[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop4.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop4[i], fromBuf.Prop4[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop5.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop5[i], fromBuf.Prop5[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop6.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop6[i], fromBuf.Prop6[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop7.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop7[i], fromBuf.Prop7[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop8.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop8[i], fromBuf.Prop8[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop9.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop9[i], fromBuf.Prop9[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop10.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop10[i], fromBuf.Prop10[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop11.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop11[i], fromBuf.Prop11[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop12.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop12[i], fromBuf.Prop12[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop13.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop13[i], fromBuf.Prop13[i]);
            }
            for (int i = 0; i < _largeValueMsg.Prop14.Length; i++)
            {
                Assert.Equal(_largeValueMsg.Prop14[i], fromBuf.Prop14[i]);
            }
        }

        static void GenerateLargeValueMessage()
        {
            var rnd = new System.Random(123456);

            var bytes = new byte[512 * 1024];

            rnd.NextBytes(bytes);

            var strings = new string[1024];

            for (int i = 0; i < strings.Length; i++)
                strings[i] = new string('s', i);

            var decimals = new decimal[1024];

            for (int i = 0; i < decimals.Length; i++)
                decimals[i] = (decimal)rnd.NextDouble();

            T[] GenerateSequence<T>() => Enumerable.Range(0, 1500).Select(v => (T)((System.IConvertible)(v % 128)).ToType(typeof(T), null)).ToArray();

            _largeValueMsg = new LargeValueMsg
            {
                Prop1 = GenerateSequence<short>(),
                Prop2 = GenerateSequence<int>(),
                Prop3 = GenerateSequence<long>(),
                Prop4 = GenerateSequence<ushort>(),
                Prop5 = GenerateSequence<uint>(),
                Prop6 = GenerateSequence<ulong>(),
                Prop8 = GenerateSequence<sbyte>(),
                Prop10 = Enumerable.Repeat(new System.DateTime(2020, 3, 20, 10, 20, 30, System.DateTimeKind.Utc), 250).ToArray(),
                Prop11 = Enumerable.Repeat(new System.TimeSpan(20, 55, 12), 250).ToArray(),
                Prop12 = Enumerable.Range(0, 512).Select(e => e % 2 == 0 ? false : true).ToArray(),
                Prop13 = strings.SelectMany(s => s).ToArray(),
                Prop7 = bytes,
                Prop9 = strings,
                Prop14 = decimals
            };
        }
    }

    [RePacker]
    public struct LargeValueMsg
    {
        [RePack]
        public short[] Prop1 { get; set; }

        [RePack]
        public int[] Prop2 { get; set; }

        [RePack]
        public long[] Prop3 { get; set; }

        [RePack]
        public ushort[] Prop4 { get; set; }

        [RePack]
        public uint[] Prop5 { get; set; }

        [RePack]
        public ulong[] Prop6 { get; set; }

        [RePack]
        public byte[] Prop7 { get; set; }

        [RePack]
        public sbyte[] Prop8 { get; set; }

        [RePack]
        public string[] Prop9 { get; set; }

        [RePack]
        public System.DateTime[] Prop10 { get; set; }

        [RePack]
        public System.TimeSpan[] Prop11 { get; set; }

        [RePack]
        public bool[] Prop12 { get; set; }

        [RePack]
        public char[] Prop13 { get; set; }

        [RePack]
        public decimal[] Prop14 { get; set; }
    }
}
namespace Refsa.RePacker.Benchmarks
{

    public static class RandomStuff
    {
        public static void Run()
        {
            /* var buffer = new BoxedBuffer(1024);
            ArrayWrapper awrapper = new ArrayWrapper();
            Array testArray = Enumerable.Range(0, 10).Select(e => (float)e).ToArray();
            awrapper.Pack(buffer, ref testArray);

            Array targetArray = new float[10];
            awrapper.Unpack(buffer, ref targetArray);

            Console.WriteLine(targetArray.GetValue(5)); */

            /* Vector3[] testArray = Enumerable.Range(0, 10).Select(e => new Vector3 { X = e, Y = e * 2, Z = e * 4 }).ToArray();
            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<Vector3[]>(buffer, ref testArray);
            var fromBuf = RePacker.Unpack<Vector3[]>(buffer);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(fromBuf[i].X + ", " + fromBuf[i].Y + ", " + fromBuf[i].Z);
            } */


            /* List<Vector3> testList = Enumerable.Range(0, 10).Select(e => new Vector3 { X = e, Y = e * 2, Z = e * 4 }).ToList();
            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref testList);
            var fromBuf = RePacker.Unpack<List<Vector3>>(buffer);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(fromBuf[i].X + ", " + fromBuf[i].Y + ", " + fromBuf[i].Z);
            }  */

            /* Queue<Vector3> testList = new Queue<Vector3>(Enumerable.Range(0, 10).Select(e => new Vector3 { X = e, Y = e * 2, Z = e * 4 }));
            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref testList);
            var fromBuf = RePacker.Unpack<Queue<Vector3>>(buffer);

            for (int i = 0; i < 10; i++)
            {
                var popped = fromBuf.Dequeue();
                Console.WriteLine(popped.X + ", " + popped.Y + ", " + popped.Z);
            } */

            /* Dictionary<int, int> testDict = new Dictionary<int, int>();
            for (int i = 0; i < 10; i++)
            {
                testDict.Add(i, i * i);
            }

            var buffer = new BoxedBuffer(1024); */

            /* RePacker.Pack(buffer, ref testDict);
            var fromBuf = RePacker.Unpack<Dictionary<int, int>>(buffer);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(i + ": " + fromBuf[i]);
            } */

            /* buffer.Reset();
            var keyValuePairs = testDict.ToList();
            foreach (var kvp in keyValuePairs)
            {
                var kvp_ = kvp;
                RePacker.Pack<KeyValuePair<int, int>>(buffer, ref kvp_);
            }

            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                var kvp = RePacker.Unpack<KeyValuePair<int, int>>(buffer);
                Console.WriteLine(kvp.Key + " - " + kvp.Value);
            } */

            /* var vt2 = new ValueTuple<int, int, int, int, int>(10, 100, 1000, 10000, 100000);
            RePacker.Pack(buffer, ref vt2);
            var vt2FromBuf = RePacker.Unpack<ValueTuple<int, int, int, int, int>>(buffer);
            Console.WriteLine(vt2FromBuf.Item1 + " - " + vt2FromBuf.Item2 + " - " + vt2FromBuf.Item3 + " - " + vt2FromBuf.Item4 + " - " + vt2FromBuf.Item5); */

            /* var personArray = Enumerable.Range(0, 1000).Select(e => new ZeroFormatterBench.Person { Age = e, FirstName = "Windows", LastName = "Server", Sex = ZeroFormatterBench.Sex.Female }).ToArray();
            var container = new ZeroFormatterBench.PersonArrayContainer { Persons = personArray };

            var buffer = new BoxedBuffer(1 << 16);

            RePacker.Pack(buffer, ref container);
            var fromBuf = RePacker.Unpack<ZeroFormatterBench.PersonArrayContainer>(buffer);

            for (int i = 0; i < 1000; i++)
            {
                bool equal =
                    (ZeroFormatterBench.Sex.Female == fromBuf.Persons[i].Sex) &&
                    (i == fromBuf.Persons[i].Age) &&
                    ("Windows" == fromBuf.Persons[i].FirstName) &&
                    ("Server" == fromBuf.Persons[i].LastName);

                if (!equal)
                {
                    Console.WriteLine("Somethings fucked yo");
                    RePacker.Log<ZeroFormatterBench.Person>(ref fromBuf.Persons[i]);
                    break;
                }
            } */


            // object[] param = new object[] { 1.234f, 15, (byte)127 };
            // Program.TestStruct test = (Program.TestStruct)TypeCache.CreateInstance(typeof(Program.TestStruct), param);
            // Console.WriteLine($"{test.Value1} - {test.Value2} - {test.Value3}");


            /* TestClass2 ts2 = new TestClass2
            {
                Bool = true,
                // Char = 'X',
                Sbyte = 10,
                Byte = 20,
                Short = 30,
                Ushort = 40,
                Int = 50,
                Uint = 60,
                Long = 70,
                Ulong = 80,
                Float = 90,
                Double = 100,
                Decimal = 1000,
            };
            Buffer buffer = new Buffer(new byte[1024], 0);
            BoxedBuffer boxedBuffer = new BoxedBuffer(ref buffer);

            // Serialize
            TypeCache.Serialize<TestClass2>(boxedBuffer, ref ts2);
            Console.WriteLine($"Buffer Size: {boxedBuffer.Buffer.Length()}");

            // Deserialize
            TestClass2 des = TypeCache.Deserialize<TestClass2>(boxedBuffer);
            Console.WriteLine($"{des}");
            Console.WriteLine($"Buffer Size: {boxedBuffer.Buffer.Length()}"); */

            /* StructWithString sws = new StructWithString
            {
                Float = 1.337f,
                String1 = "Hello",
                String2 = "World",
                Int = 1337,
            };

            BoxedBuffer buffer = new BoxedBuffer(1024);

            RePacker.Pack<StructWithString>(buffer, ref sws);
            Console.WriteLine($"Buffer Size: {buffer.Buffer.Length()}");

            var fromBuf = RePacker.Unpack<StructWithString>(buffer);
            Console.WriteLine($"Buffer Size: {buffer.Buffer.Length()}"); */

            /* StructWithEnum sws = new StructWithEnum
            {
                Float = 1.337f,
                ByteEnum = ByteEnum.Ten,
                LongEnum = LongEnum.High,
                Int = 1337,
            };

            BoxedBuffer buffer = new BoxedBuffer(1024);

            RePacker.Pack<StructWithEnum>(buffer, ref sws);
            Console.WriteLine($"Buffer Size: {buffer.Buffer.Length()}");

            var fromBuf = RePacker.Unpack<StructWithEnum>(buffer);
            Console.WriteLine($"Buffer Size: {buffer.Buffer.Length()}");

            RePacker.Log<StructWithEnum>(ref fromBuf); */

            /* var p = new Parent
            {
                Float = 1.337f,
                ULong = 987654321,
                Child = new Child
                {
                    Float = 10f,
                    Byte = 120,
                },
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<Parent>(buffer, ref p);

            var fromBuf = RePacker.Unpack<Parent>(buffer);
            RePacker.Log<Parent>(ref fromBuf); */

            /* var rt = new RootType
            {
                Float = 13.37f,
                Double = 9876.54321,
                UnmanagedStruct = new UnmanagedStruct
                {
                    Int = 1337,
                    ULong = 9876543210,
                },
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<RootType>(buffer, ref rt);
            var fromBuf = RePacker.Unpack<RootType>(buffer);

            RePacker.Log<RootType>(ref fromBuf); */

            /* var swa = new StructWithArray
            {
                Int = 1337,
                Long = -123456789,
                Floats = new float[] {
                    0f, 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f
                }
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack<StructWithArray>(buffer, ref swa);
            
            var fromBuf = RePacker.Unpack<StructWithArray>(buffer);
            RePacker.Log<StructWithArray>(ref fromBuf); */

            /* var hml = new HasUnmanagedIList
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new List<int>{1,2,3,4,5,6,7,8,9,0},
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);
            var fromBuf = RePacker.Unpack<HasUnmanagedIList>(buffer);

            RePacker.Log<HasUnmanagedIList>(ref fromBuf); */

            /* var hml = new HasUnmanagedList
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 },
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hml);
            var fromBuf = RePacker.Unpack<HasUnmanagedList>(buffer);

            RePacker.Log<HasUnmanagedList>(ref fromBuf); */

            /* var hml = new HasUnmanagedQueue
            {
                Float = 141243f,
                Double = 2345613491441234,
                Ints = new Queue<int>(System.Linq.Enumerable.Range(1, 10)),
            };
            var buffer = new BoxedBuffer(1024);
            RePacker.Pack(buffer, ref hml);
            var fromBuf = RePacker.Unpack<HasUnmanagedQueue>(buffer);

            RePacker.Log(ref fromBuf); */

            // var generator = GeneratorLookup.Get(GeneratorType.Struct, null);
            // Console.WriteLine(generator);

            /* var um = new UnmanagedStruct
            {
                Int = 1337,
                ULong = 9876543210,
            };

            var buffer = new BoxedBuffer(1024);
            RePacker.Pack(buffer, ref um);
            var fromBuf = RePacker.Unpack<UnmanagedStruct>(buffer);

            RePacker.Log<UnmanagedStruct>(ref fromBuf); */

            /* var dictCont = new HasUnmanagedDictionary
            {
                Float = 1234.4562345f,
                Long = 23568913457,
                Dict = new Dictionary<int, float> {
                    {123, 12.5f},
                    {4567456, 125.2345f},
                    {23454, 3456.235f},
                    {345643, 3456234.1341f},
                    {89678, 1234.3523f},
                    {452, .654567f},
                }
            };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref dictCont);

            Console.WriteLine("Buf Size" + buffer.Buffer.Length());

            var fromBuf = RePacker.Unpack<HasUnmanagedDictionary>(buffer);

            RePacker.Log<HasUnmanagedDictionary>(ref fromBuf); */

            /* Vector3 testVec3 = new Vector3 { X = 1.234f, Y = 4532.24f, Z = 943.342f };
            var buffer = new BoxedBuffer(1024);
            RePacker.Pack<Vector3>(buffer, ref testVec3);
            var fromBuf = RePacker.Unpack<Vector3>(buffer);
            Console.WriteLine(testVec3.X == fromBuf.X && testVec3.Y == fromBuf.Y && testVec3.Z == fromBuf.Z); */

            /* var hdt = new HasDateTime { Float = 1.2344534f, DateTime = DateTime.Now };

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref hdt);
            var fromBuf = RePacker.Unpack<HasDateTime>(buffer); */

            /* string value = "abrakadabra this is a magic trick";

            var buffer = new BoxedBuffer(1024);

            RePacker.Pack(buffer, ref value);
            string fromBuf = RePacker.Unpack<string>(buffer); */

            /* var wanted = new StructWithKeyValuePair
            {
                Float = 1.234f,
                KeyValuePair = new KeyValuePair<int, int>(10, 100)
            };

            var buffer = new BoxedBuffer(1024);
            RePacker.Pack(buffer, ref wanted);

            var valuetuples = new StructWithValueTuple
            {
                VT2 = (10, 100),
                VT3 = (10, 100, 1000),
                VT4 = (10, 100, 1000, 10000),
                VT5 = (10, 100, 1000, 10000, 100000),
                VT6 = (10, 100, 1000, 10000, 100000, 1000000),
            };

            RePacker.Pack(buffer, ref valuetuples); */

            // var vtrest = ValueTuple.Create(1,1,1,1,1,1,1, ValueTuple.Create(1,1,1,1,1,1,1));
            // RePacker.Pack(buffer, ref vtrest);
        }
    }
}
using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using Refsa.RePacker;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Unsafe;
using Buffer = Refsa.RePacker.Buffers.Buffer;
using System.Linq;
using System.Collections.Generic;
using Refsa.RePacker.Builder;
using System.Runtime.InteropServices;

namespace Refsa.RePacker.Benchmarks
{
    public class Program
    {
        [RePacker]
        public struct Ints
        {
            public int Int1;
            public int Int2;
            public int Int3;
            public int Int4;
        }

        public static void Main(string[] args)
        {
            RePacker.Init();
            Console.WriteLine("Benchmark");

            // var bufferBench = BenchmarkRunner.Run<BufferBench>();
            var zeroFormatterBench = BenchmarkRunner.Run<ZeroFormatterBench>();
            // var ilGeneratedBench = BenchmarkRunner.Run<ILGenerated>();
            // var generalBenches = BenchmarkRunner.Run<GeneralBenches>();
            // var setupBench = BenchmarkRunner.Run<SetupBench>();

            // new TestClass();
            // RandomStuff.Run();

            // int[] intArray = Enumerable.Range(0, 1000).ToArray();
            // var intArrayBuffer = new BoxedBuffer(sizeof(int) * 1002);
            // intArrayBuffer.Pack(ref intArray);
        }
    }
}

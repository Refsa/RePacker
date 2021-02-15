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

namespace Refsa.RePacker.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RePacker.Init();
            Console.WriteLine("Benchmark");

            // var bufferBench = BenchmarkRunner.Run<BufferBench>();
            // var zeroFormatterBench = BenchmarkRunner.Run<ZeroFormatterBench>();
            // var ilGeneratedBench = BenchmarkRunner.Run<ILGenerated>();
            // var generalBenches = BenchmarkRunner.Run<GeneralBenches>();
            var setupBench = BenchmarkRunner.Run<SetupBench>();

            // new TestClass();
            // RandomStuff.Run();
        }
    }
}

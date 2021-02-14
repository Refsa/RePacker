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

            // var summary1 = BenchmarkRunner.Run<BufferBench>();
            // var summary2 = BenchmarkRunner.Run<ZeroFormatterBench>();
            // var summary2 = BenchmarkRunner.Run<ILGenerated>();
            // var summary2 = BenchmarkRunner.Run<GeneralBenches>();

            new TestClass();

            // RandomStuff.Run();
        }
    }
}

using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using RePacker;
using RePacker.Buffers;
using RePacker.Unsafe;
using Buffer = RePacker.Buffers.ReBuffer;
using System.Linq;
using System.Collections.Generic;
using RePacker.Builder;
using System.Runtime.InteropServices;

namespace RePacker.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RePacking.Init();
            Console.WriteLine("Benchmark Start");

            var zeroFormatterBench = BenchmarkRunner.Run<ZeroFormatterBench>();
            // var bufferBench = BenchmarkRunner.Run<BufferBench>();
            // var ilGeneratedBench = BenchmarkRunner.Run<ILGenerated>();
            // var generalBenches = BenchmarkRunner.Run<GeneralBenches>();
            // var setupBench = BenchmarkRunner.Run<SetupBench>();

            // new TestClass();
            // RandomStuff.Run();

            Console.WriteLine("Benchmark End");
        }
    }
}

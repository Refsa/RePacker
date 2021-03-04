using System;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using RePacker;
using RePacker.Buffers;
using RePacker.Unsafe;
using Buffer = RePacker.Buffers.Buffer;
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

            // var bufferBench = BenchmarkRunner.Run<BufferBench>();
            // var zeroFormatterBench = BenchmarkRunner.Run<ZeroFormatterBench>();
            // var ilGeneratedBench = BenchmarkRunner.Run<ILGenerated>();
            // var generalBenches = BenchmarkRunner.Run<GeneralBenches>();
            // var setupBench = BenchmarkRunner.Run<SetupBench>();

            // new TestClass();
            // RandomStuff.Run();

            var buffer = new Buffer(1024);
            var v = new ZeroFormatterBench.Person();
            RePacking.Pack(buffer, ref v);
            // var fromBuf = RePacking.Unpack<ZeroFormatterBench.Person>(buffer);

            // int val = 10;
            // RePacking.Pack(buffer, ref val);

            Console.WriteLine("Benchmark End");
        }
    }
}

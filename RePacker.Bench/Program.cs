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
            RePacker.Init();
            Console.WriteLine("Benchmark Start");

            // var bufferBench = BenchmarkRunner.Run<BufferBench>();
            // var zeroFormatterBench = BenchmarkRunner.Run<ZeroFormatterBench>();
            // var ilGeneratedBench = BenchmarkRunner.Run<ILGenerated>();
            // var generalBenches = BenchmarkRunner.Run<GeneralBenches>();
            // var setupBench = BenchmarkRunner.Run<SetupBench>();

            // new TestClass();
            // RandomStuff.Run();

            char[] strings = Enumerable.Range(0, 1000).Select(e => 'c').ToArray();

            var buffer = new BoxedBuffer(1 << 16);
            buffer.Pack(ref strings);
            var fromBuf = buffer.Unpack<char[]>();

            for (int i = 0; i < strings.Length; i++)
            {
                if (strings[i] != fromBuf[i])
                {
                    // Console.WriteLine("turn");
                }
            }

            Console.WriteLine("Benchmark End");
        }
    }
}

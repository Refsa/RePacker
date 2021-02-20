using System;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace RePacker.Tests
{

    public class BootstrapClass : IDisposable
    {
        public TestLogger Logger;

        public BootstrapClass() { }

        public void Setup(ITestOutputHelper output)
        {
            Logger = new TestLogger(output);
            var settings = new RePackerSettings(Logger);
            if (!RePacker.IsSetup)
            {
                RePacker.Init(settings);
            }
            else
            {
                RePacker.SetSettings(settings);
            }
        }

        public void Dispose()
        {

        }
    }

    public static class TestBootstrap
    {
        public static BootstrapClass Bootstrap;

        public static void Setup(ITestOutputHelper output)
        {
            Bootstrap = new BootstrapClass();
            Bootstrap.Setup(output);
        }
    }
}
using System;
using Xunit;
using Xunit.Abstractions;

// [assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace RePacker.Tests
{

    public class GlobalData : IDisposable
    {
        public TestLogger Logger;

        public GlobalData() { }

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
        public static GlobalData GlobalData;

        public static void Setup(ITestOutputHelper output)
        {
            GlobalData = new GlobalData();
            GlobalData.Setup(output);
        }
    }
}
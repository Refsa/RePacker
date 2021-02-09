using System;
using Refsa.RePacker.Utils;

namespace Refsa.RePacker.Unity
{
    public static class UnityBootstrap
    {
        static UnityBootstrap()
        {
            var unitySettings = new RePackerSettings(new UnityLogger());
            RePacker.Init(unitySettings);
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod]
        static void SetupRePacker() { }
    }
}
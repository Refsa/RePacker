using System;
using Refsa.RePacker.Utils;

namespace Refsa.RePacker.Unity
{
#if !NO_BOOTSTRAP
    public static class UnityBootstrap
    {
        static UnityBootstrap()
        {
            var unitySettings = new RePackerSettings(new UnityLogger());
#if DISABLE_LOGGING
                unitySettings.LoggingEnabled = false;
#endif

            RePacker.Init(unitySettings);
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod]
        static void SetupRePacker() { }
    }
#endif
}
using System;
using RePacker.Utils;

namespace RePacker.Unity
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

            RePacking.Init(unitySettings);
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void SetupRePacker() { }
    }
#endif
}
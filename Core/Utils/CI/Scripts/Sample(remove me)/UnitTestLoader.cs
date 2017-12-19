
using UnityEngine;
using RuntimeUnitTestToolkit;

namespace SampleUnitTest
{
    public static class UnitTestLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Register()
        {
            // setup created test class to RegisterAllMethods<T>
            UnitTest.RegisterAllMethods<SampleGroup>();
        }
    }
}
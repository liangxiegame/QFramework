#if !NOT_UNITY3D

using ModestTree;

namespace Zenject
{
    public class SceneKernel : MonoKernel
    {
        // Only needed to set "script execution order" in unity project settings

#if ZEN_INTERNAL_PROFILING
        public override void Start()
        {
            base.Start();
            Log.Info("SceneContext.Awake detailed profiling: {0}", ProfileTimers.FormatResults());
        }
#endif
    }
}

#endif

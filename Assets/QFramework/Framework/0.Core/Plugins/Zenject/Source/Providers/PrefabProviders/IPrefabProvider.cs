#if !NOT_UNITY3D

namespace Zenject
{
    public interface IPrefabProvider
    {
        UnityEngine.Object GetPrefab();
    }
}

#endif


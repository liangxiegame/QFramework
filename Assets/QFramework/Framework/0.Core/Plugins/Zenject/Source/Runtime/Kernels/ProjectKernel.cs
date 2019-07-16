#if !NOT_UNITY3D

using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine.SceneManagement;

namespace Zenject
{
    public class ProjectKernel : MonoKernel
    {
        [Inject]
        ZenjectSettings _settings = null;

        [Inject]
        SceneContextRegistry _contextRegistry = null;

        // One issue with relying on MonoKernel.OnDestroy to call IDisposable.Dispose
        // is that the order that OnDestroy is called in is difficult to predict
        // One good thing is that it does follow the heirarchy order (so root game objects
        // will have thier OnDestroy called before child objects)
        // However, the order that OnDestroy is called for the root game objects themselves
        // is largely random
        // Within an individual scene, this can be helped somewhat by placing all game objects
        // underneath the SceneContext and then also checking the 'ParentNewObjectsUnderRoot'
        // property to ensure any new game objects will also be parented underneath SceneContext
        // By doing this, we can be guaranteed to have any bound IDisposable's have their
        // Dispose called before any game object is destroyed in the scene
        // However, when using multiple scenes (each with their own SceneContext) the order
        // that these SceneContext game objects are destroyed is random
        // So to address that, we explicitly call GameObject.DestroyImmediate for all
        // SceneContext's in the reverse order that the scenes were loaded in below
        // (this works because OnApplicationQuit is always called before OnDestroy)
        // Note that this only works when stopping the app and not when changing scenes
        // When changing scenes, if you have multiple scenes loaded at once, you will have to
        // manually unload the scenes in the reverse order they were loaded before going to
        // the new scene, if you require a predictable destruction order.  Or you can always use
        // ZenjectSceneLoader which will do this for you
        public void OnApplicationQuit()
        {
            if (_settings.EnsureDeterministicDestructionOrderOnApplicationQuit)
            {
                DestroyEverythingInOrder();
            }
        }

        public void DestroyEverythingInOrder()
        {
            ForceUnloadAllScenes(true);

            // Destroy project context after all scenes
            Assert.That(!IsDestroyed);
            DestroyImmediate(gameObject);
            Assert.That(IsDestroyed);
        }

        public void ForceUnloadAllScenes(bool immediate = false)
        {
            // OnApplicationQuit should always be called before OnDestroy
            // (Unless it is destroyed manually)
            Assert.That(!IsDestroyed);

            var sceneOrder = new List<Scene>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                sceneOrder.Add(SceneManager.GetSceneAt(i));
            }

            // Destroy the scene contexts from bottom to top
            // Since this is the reverse order that they were loaded in
            foreach (var sceneContext in _contextRegistry.SceneContexts.OrderByDescending(x => sceneOrder.IndexOf(x.gameObject.scene)).ToList())
            {
                if (immediate)
                {
                    DestroyImmediate(sceneContext.gameObject);
                }
                else
                {
                    Destroy(sceneContext.gameObject);
                }
            }
        }
    }
}

#endif

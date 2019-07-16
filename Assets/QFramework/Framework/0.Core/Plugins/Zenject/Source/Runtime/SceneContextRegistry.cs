using System.Collections.Generic;
using ModestTree;
using UnityEngine.SceneManagement;

namespace Zenject
{
    public class SceneContextRegistry
    {
        readonly Dictionary<Scene, SceneContext> _map = new Dictionary<Scene, SceneContext>();

        public IEnumerable<SceneContext> SceneContexts
        {
            get { return _map.Values; }
        }

        public void Add(SceneContext context)
        {
            Assert.That(!_map.ContainsKey(context.gameObject.scene));
            _map.Add(context.gameObject.scene, context);
        }

        public SceneContext GetSceneContextForScene(string name)
        {
            var scene = SceneManager.GetSceneByName(name);
            Assert.That(scene.IsValid(), "Could not find scene with name '{0}'", name);
            return GetSceneContextForScene(scene);
        }

        public SceneContext GetSceneContextForScene(Scene scene)
        {
            return _map[scene];
        }

        public SceneContext TryGetSceneContextForScene(string name)
        {
            var scene = SceneManager.GetSceneByName(name);
            Assert.That(scene.IsValid(), "Could not find scene with name '{0}'", name);
            return TryGetSceneContextForScene(scene);
        }

        public SceneContext TryGetSceneContextForScene(Scene scene)
        {
            SceneContext context;

            if (_map.TryGetValue(scene, out context))
            {
                return context;
            }

            return null;
        }

        public DiContainer GetContainerForScene(Scene scene)
        {
            var container = TryGetContainerForScene(scene);

            if (container != null)
            {
                return container;
            }

            throw Assert.CreateException(
                "Unable to find DiContainer for scene '{0}'", scene.name);
        }

        public DiContainer TryGetContainerForScene(Scene scene)
        {
            if (scene == ProjectContext.Instance.gameObject.scene)
            {
                return ProjectContext.Instance.Container;
            }

            var sceneContext = TryGetSceneContextForScene(scene);

            if (sceneContext != null)
            {
                return sceneContext.Container;
            }

            return null;
        }

        public void Remove(SceneContext context)
        {
            bool removed = _map.Remove(context.gameObject.scene);

            if (!removed)
            {
                Log.Warn("Failed to remove SceneContext from SceneContextRegistry");
            }
        }
    }

}

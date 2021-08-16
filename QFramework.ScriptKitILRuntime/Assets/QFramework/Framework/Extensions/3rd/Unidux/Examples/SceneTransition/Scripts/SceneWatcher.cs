using Unidux.SceneTransition;
using UniRx;
using UnityEngine;

namespace Unidux.Example.SceneTransition
{
    public class SceneWatcher : MonoBehaviour
    {
        void Start()
        {
            Unidux.Subject
                .Where(state => state.Scene.IsStateChanged)
                .StartWith(Unidux.State)
                .Subscribe(state => ChangeScenes(state.Scene))
                .AddTo(this);
        }

        void ChangeScenes(SceneState<Scene> state)
        {
            foreach (var scene in state.Removals(SceneUtil.GetActiveScenes<Scene>()))
            {
                StartCoroutine(SceneUtil.Remove(scene.ToString()));
            }
            
            foreach (var scene in state.Additionals(SceneUtil.GetActiveScenes<Scene>()))
            {
                StartCoroutine(SceneUtil.Add(scene.ToString()));
            }
        }
    }
}
using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Unidux.SceneTransition
{
    public static class SceneUtil
    {
        public static System.Collections.Generic.IEnumerable<TScene> GetActiveScenes<TScene>()
            where TScene : struct
        {
            var sceneCount = SceneManager.sceneCount;

            for (var i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var enumScene = (TScene)Enum.Parse(typeof(TScene), scene.name);
                yield return enumScene;
            }
        }

        public static IEnumerator Add(string name)
        {
            if (!SceneManager.GetSceneByName(name).isLoaded)
            {
                yield return SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            }
        }

        public static IEnumerator Remove(string name)
        {
            if (SceneManager.GetSceneByName(name).isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(name);
            }
        }
    }
}
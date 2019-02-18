using System;
using System.Collections;
using UnityEngine;

namespace uFrame.Kernel
{
    public abstract class SceneLoader<T> : uFrameComponent, ISceneLoader where T : IScene
    {

        public virtual Type SceneType
        {
            get { return typeof (T); }
        }

        /// <summary>
        /// The type of scene that this loader is for.  This is used by the kernel to link link it to the
        /// scene type when the "SceneAwakeEvent" is invoked.
        /// </summary>
        /// <param name="scene">The scene component that is at the root of the scene.</param>
        /// <param name="progressDelegate">The progress delegate for providing user feedback on long running actions.</param>
        /// <returns></returns>
        protected abstract IEnumerator LoadScene(T scene, Action<float, string> progressDelegate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene">The scene component that is at the root of the scene.</param>
        /// <param name="progressDelegate">The progress delegate for providing user feedback on long running actions.</param>
        /// <returns></returns>
        protected abstract IEnumerator UnloadScene(T scene, Action<float, string> progressDelegate);

        public IEnumerator Load(object sceneObject, Action<float, string> progressDelegate)
        {
            return LoadScene((T) sceneObject, progressDelegate);
        }

        public IEnumerator Unload(object sceneObject, Action<float, string> progressDelegate)
        {
            return UnloadScene((T) sceneObject, progressDelegate);
        }

    }



    public class DefaultSceneLoader : SceneLoader<IScene>
    {

        public override Type SceneType
        {
            get { return typeof (IScene); }
        }

        protected override IEnumerator LoadScene(IScene scene, Action<float, string> progressDelegate)
        {
            yield break;
        }

        protected override IEnumerator UnloadScene(IScene scene, Action<float, string> progressDelegate)
        {
            yield break;
        }

        //public IEnumerator Load(object sceneObject, Action<float, string> progressDelegate)
        //{
        //    return LoadScene((IScene)sceneObject, progressDelegate);
        //}

        //public IEnumerator Unload(object sceneObject, object settings, Action<float, string> progressDelegate)
        //{
        //    return UnloadScene((IScene)sceneObject, progressDelegate);
        //}
    }

}
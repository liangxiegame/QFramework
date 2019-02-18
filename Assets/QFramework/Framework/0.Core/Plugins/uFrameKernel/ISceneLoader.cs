using System;
using System.Collections;
/// <summary>
/// The Scene Loader is used to manage loading and unloading a scene, it is part of the kernel and is always available,
/// this makes sure that whenever you want to load a scene, you have complete control over it very easily.
/// </summary>
public interface ISceneLoader
{
    /// <summary>
    /// The type of scene that this loader is for.  This is used by the kernel to link link it to the
    /// scene type when the "SceneAwakeEvent" is invoked.
    /// </summary>
    Type SceneType { get; }

    /// <summary>
    /// Use this method to load the scene, this is useful to provide any additional setup that the scene may need programatically.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="progressDelegate"></param>
    /// <returns></returns>
    IEnumerator Load(object scene, Action<float, string> progressDelegate);

    /// <summary>
    /// Whenever the sene is destroyed, Unload gives you programmatic control over how a scene is unloaded.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="progressDelegate"></param>
    /// <returns></returns>
    IEnumerator Unload(object scene, Action<float, string> progressDelegate);
}
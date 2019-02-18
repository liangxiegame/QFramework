using System.Collections;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;

namespace uFrame.Kernel
{
    /// <summary>
    /// The scene class is used to define a scene as a class, 
    /// this MonoBehaviour should live on a gameobject that is at the root level of the scene it is defining.
    /// When this type is loaded by unity, it will publish the SceneAwakeEvent.  The SceneManagementService (part of the kernel) will
    /// then find the scene loader associated with this scene and invoke its Load Co-Routine method.
    /// </summary>
    public class Scene : uFrameComponent, IScene
    {
        [SerializeField] private string _KernelScene;

        /// <summary>
        /// The kernel scene property is so that this scene can load the correct kernel if it hasn't been loaded yet.
        /// </summary>
        protected string KernelScene
        {
            get
            {
                if (string.IsNullOrEmpty(_KernelScene))
                {
                    return DefaultKernelScene;
                }
                return _KernelScene;
            }
        }

        /// <summary>
        /// The default kernel scene is what is used if the "KernelScene" property is not set.  This is really used by
        /// the uFrame designer to remove the extra step of specifying the kernel scene each time a scene is created.
        /// </summary>
        public virtual string DefaultKernelScene { get; set; }

        /// <summary>
        /// The Name of this scene, this is set by the kernel so that it can reference back to it and destroy it when the
        /// Unload Scene Command is fired.
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// If this scene was loaded via a 
        /// </summary>
        public ISceneSettings _SettingsObject { get; set; }

        /// <summary>
        /// In this class we override the start method so that we can trigger the kernel to load if its not already.
        /// </summary>
        /// <returns></returns>
        protected override void Start()
        {
            if (!uFrameKernel.IsKernelLoaded)
            {
                Name = SceneManager.GetActiveScene().name;
                StartCoroutine(uFrameKernel.InstantiateSceneAsyncAdditively(KernelScene));
            }

            base.Start();
        }

        public override void KernelLoaded()
        {
            base.KernelLoaded();
            this.Publish(new SceneAwakeEvent() { Scene = this });
        }
    }

    /// <summary>
    /// This class is used internally by the Scene class and the kernel to trigger scene loaders load method.
    /// </summary>
    public class SceneAwakeEvent
    {
        public IScene Scene { get; set; }
    }
}
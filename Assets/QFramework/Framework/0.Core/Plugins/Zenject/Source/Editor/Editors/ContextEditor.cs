#if !ODIN_INSPECTOR

namespace Zenject
{
    [NoReflectionBaking]
    public class ContextEditor : UnityInspectorListEditor
    {
        protected override string[] PropertyNames
        {
            get
            {
                return new string[]
                {
                    "_scriptableObjectInstallers",
                    "_monoInstallers",
                    "_installerPrefabs",
                };
            }
        }

        protected override string[] PropertyDisplayNames
        {
            get
            {
                return new string[]
                {
                    "Scriptable Object Installers",
                    "Mono Installers",
                    "Prefab Installers",
                };
            }
        }

        protected override string[] PropertyDescriptions
        {
            get
            {
                return new string[]
                {
                    "Drag any assets in your Project that implement ScriptableObjectInstaller here",
                    "Drag any MonoInstallers that you have added to your Scene Hierarchy here.",
                    "Drag any prefabs that contain a MonoInstaller on them here",
                };
            }
        }
    }
}

#endif

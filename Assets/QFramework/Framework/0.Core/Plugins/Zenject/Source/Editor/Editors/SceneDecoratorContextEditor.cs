#if !ODIN_INSPECTOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using ModestTree;

namespace Zenject
{
    [CustomEditor(typeof(SceneDecoratorContext))]
    [NoReflectionBaking]
    public class SceneDecoratorContextEditor : ContextEditor
    {
        SerializedProperty _decoratedContractNameProperty;

        protected override string[] PropertyNames
        {
            get
            {
                return base.PropertyNames.Concat(new string[]
                    {
                        "_lateInstallers",
                        "_lateInstallerPrefabs",
                        "_lateScriptableObjectInstallers"
                    })
                    .ToArray();
            }
        }

        protected override string[] PropertyDisplayNames
        {
            get
            {
                return base.PropertyDisplayNames.Concat(new string[]
                    {
                        "Late Installers",
                        "Late Prefab Installers",
                        "Late Scriptable Object Installers"
                    })
                    .ToArray();
            }
        }

        protected override string[] PropertyDescriptions
        {
            get
            {
                return base.PropertyDescriptions.Concat(new string[]
                    {
                        "Drag any MonoInstallers that you have added to your Scene Hierarchy here. They'll be installed after the target installs its bindings",
                        "Drag any prefabs that contain a MonoInstaller on them here. They'll be installed after the target installs its bindings",
                        "Drag any assets in your Project that implement ScriptableObjectInstaller here. They'll be installed after the target installs its bindings"
                    })
                    .ToArray();
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            _decoratedContractNameProperty = serializedObject.FindProperty("_decoratedContractName");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(_decoratedContractNameProperty);
        }
    }
}

#endif

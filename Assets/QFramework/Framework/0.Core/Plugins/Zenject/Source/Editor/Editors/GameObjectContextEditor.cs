#if !ODIN_INSPECTOR

using UnityEditor;

namespace Zenject
{
    [CustomEditor(typeof(GameObjectContext))]
    [NoReflectionBaking]
    public class GameObjectContextEditor : RunnableContextEditor
    {
        SerializedProperty _kernel;

        public override void OnEnable()
        {
            base.OnEnable();

            _kernel = serializedObject.FindProperty("_kernel");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(_kernel);
        }
    }
}

#endif

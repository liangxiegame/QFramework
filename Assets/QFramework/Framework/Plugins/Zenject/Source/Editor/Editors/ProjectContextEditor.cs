#if !ODIN_INSPECTOR

using UnityEditor;

namespace Zenject
{
    [CustomEditor(typeof(ProjectContext))]
    [NoReflectionBaking]
    public class ProjectContextEditor : ContextEditor
    {
        SerializedProperty _settingsProperty;
        SerializedProperty _editorReflectionBakingCoverageModeProperty;
        SerializedProperty _buildsReflectionBakingCoverageModeProperty;

        public override void OnEnable()
        {
            base.OnEnable();

            _settingsProperty = serializedObject.FindProperty("_settings");
            _editorReflectionBakingCoverageModeProperty = serializedObject.FindProperty("_editorReflectionBakingCoverageMode");
            _buildsReflectionBakingCoverageModeProperty = serializedObject.FindProperty("_buildsReflectionBakingCoverageMode");
        }

        protected override void OnGui()
        {
            base.OnGui();

            EditorGUILayout.PropertyField(_settingsProperty, true);
            EditorGUILayout.PropertyField(_editorReflectionBakingCoverageModeProperty, true);
            EditorGUILayout.PropertyField(_buildsReflectionBakingCoverageModeProperty, true);
        }
    }
}

#endif

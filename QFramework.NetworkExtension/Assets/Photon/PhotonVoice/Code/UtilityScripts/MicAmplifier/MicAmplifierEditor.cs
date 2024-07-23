#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Photon.Voice.Unity.UtilityScripts
{
    [CustomEditor(typeof(MicAmplifier))]
    public class MicAmplifierEditor : Editor
    {
        private MicAmplifier simpleAmplifier;

        private void OnEnable()
        {
            this.simpleAmplifier = this.target as MicAmplifier;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            this.simpleAmplifier.AmplificationFactor = EditorGUILayout.FloatField(
                new GUIContent("Amplification Factor", "Amplification Factor (Multiplication)"),
                this.simpleAmplifier.AmplificationFactor);
            this.simpleAmplifier.BoostValue = EditorGUILayout.FloatField(
                new GUIContent("Boost Value", "Boost Value (Addition)"),
                this.simpleAmplifier.BoostValue);
            if (EditorGUI.EndChangeCheck())
            {
                this.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
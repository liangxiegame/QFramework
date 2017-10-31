using System.Linq;
using QFramework.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace QFramework.VisualDebugging.Unity.Editor {

    [CustomEditor(typeof(ContextObserverBehaviour))]
    public class ContextObserverInspector : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var contextObserver = ((ContextObserverBehaviour)target).ContextObserver;

            EntitasEditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.LabelField(contextObserver.Context.ContextInfo.Name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Entities", contextObserver.Context.Count.ToString());
                EditorGUILayout.LabelField("Reusable entities", contextObserver.Context.ReusableEntitiesCount.ToString());

                var retainedEntitiesCount = contextObserver.Context.RetainedEntitiesCount;
                if (retainedEntitiesCount != 0) {
                    var c = GUI.contentColor;
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Retained entities", retainedEntitiesCount.ToString());
                    GUI.color = c;
                    EditorGUILayout.HelpBox("WARNING: There are retained entities.\nDid you call entity.Retain(owner) and forgot to call entity.Release(owner)?", MessageType.Warning);
                }

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Create Entity")) {
                        var entity = contextObserver.Context.CreateEntity();
                        var entityBehaviour = Object.FindObjectsOfType<EntityBehaviour>()
                                                    .Single(eb => eb.Entity == entity);

                        Selection.activeGameObject = entityBehaviour.gameObject;
                    }

                    var bgColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Destroy All Entities")) {
                        contextObserver.Context.DestroyAllEntities();
                    }
                    GUI.backgroundColor = bgColor;
                }
                EditorGUILayout.EndHorizontal();
            }
            EntitasEditorLayout.EndVerticalBox();

            var groups = contextObserver.Groups;
            if (groups.Length != 0) {
                EntitasEditorLayout.BeginVerticalBox();
                {
                    EditorGUILayout.LabelField("Groups (" + groups.Length + ")", EditorStyles.boldLabel);
                    foreach (var group in groups.OrderByDescending(g => g.count)) {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField(group.ToString());
                            EditorGUILayout.LabelField(group.count.ToString(), GUILayout.Width(48));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EntitasEditorLayout.EndVerticalBox();
            }

            EditorUtility.SetDirty(target);
        }
    }
}

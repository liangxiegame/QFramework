using System.Linq;
using QFramework.Unity;
using UnityEditor;
using UnityEngine;

namespace QFramework.VisualDebugging.Unity.Editor {

    [CustomEditor(typeof(EntityLink))]
    public class EntityLinkInspector : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var link = (EntityLink)target;

            if (link.Entity != null) {
                if (GUILayout.Button("Unlink")) {
                    link.Unlink();
                }
            }

            if (link.Entity != null) {
                EditorGUILayout.Space();

                EditorGUILayout.LabelField(link.Entity.ToString());

                if (GUILayout.Button("Show entity")) {
                    Selection.activeGameObject = FindObjectsOfType<EntityBehaviour>()
                        .Single(e => e.Entity == link.Entity).gameObject;
                }

                EditorGUILayout.Space();

                EntityDrawer.DrawEntity(link.Context, link.Entity);
            } else {
                EditorGUILayout.LabelField("Not linked to an entity");
            }
        }
    }
}

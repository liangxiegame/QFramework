using System.Linq;
using UnityEditor;

namespace Entitas.VisualDebugging.Unity.Editor {

    [CustomEditor(typeof(EntityBehaviour)), CanEditMultipleObjects]
    public class EntityInspector : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            if (targets.Length == 1) {
                var entityBehaviour = (EntityBehaviour)target;
                EntityDrawer.DrawEntity(entityBehaviour.Context, entityBehaviour.Entity);
            } else {
                var entityBehaviour = (EntityBehaviour)target;
                var entities = targets
                        .Select(t => ((EntityBehaviour)t).Entity)
                        .ToArray();

                EntityDrawer.DrawMultipleEntities(entityBehaviour.Context, entities);
            }

            if (target != null) {
                EditorUtility.SetDirty(target);
            }
        }
    }
}

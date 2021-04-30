using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFramework
{
    [CustomEditor(typeof(ActionKitVisualEvent), true)]
    public class ActionKitVisualEventEditor : EasyInspectorEditor
    {
        ActionKitVisualEvent mScript
        {
            get
            {
                var script = target as ActionKitVisualEvent;

                if (!script) return script;

                if (script.Acitons == null)
                {
                    script.Acitons = new List<ActionKitVisualAction>();
                }

                return script;
            }
        }


        private List<ActionKitVisualActionEditor> mActionEditors = new List<ActionKitVisualActionEditor>();

        private void OnEnable()
        {
            mActionEditors.Clear();
            if (!mScript) return;
            foreach (var actionKitVisualAction in mScript.Acitons)
            {
                AddEditor(actionKitVisualAction);
            }

            this.AddChild(
                EasyIMGUI.Horizontal()
                    .AddChild(EasyIMGUI.Label().Text("Actions").FontBold().FontSize(12))
                    .AddChild(EasyIMGUI.FlexibleSpace())
                    .AddChild(EasyIMGUI.Button().Text("+").FontBold().FontSize(12).OnClick(() =>
                    {
                        ActionBrowser.Open((t) =>
                        {
                            var visualAction = mScript.gameObject.AddComponent(t) as ActionKitVisualAction;
                            visualAction.hideFlags = HideFlags.HideInInspector;
                            mScript.Acitons.Add(visualAction);
                            AddEditor(visualAction);

                           Save();
                        }); 
                    }))
            );
        }



        private void OnDisable()
        {
            mActionEditors.Clear();
            this.GetLayout().Clear();
        }


        void AddEditor(ActionKitVisualAction action)
        {
            var editor = CreateEditor(action, typeof(ActionKitVisualActionEditor)) as ActionKitVisualActionEditor;

            editor.OnDeleteAction = () =>
            {
                this.PushRenderEndCommand(() =>
                {
                    mActionEditors.Remove(editor);
                    mScript.Acitons.Remove(editor.target as ActionKitVisualAction);
                    DestroyImmediate(editor.target);
                    Save();
                });
            };
            mActionEditors.Add(editor);
        }
        
        
        
        private bool mFoldOut = false;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal("Box");
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();



            EditorGUILayout.BeginHorizontal();
            mFoldOut = EditorGUILayout.Foldout(mFoldOut, target.GetType().Name);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-"))
            {
                OnEventDelete.InvokeGracefully();
            }
            EditorGUILayout.EndHorizontal();

            if (mFoldOut)
            {
                base.OnInspectorGUI();
                
                this.GetLayout().DrawGUI();

                foreach (var actionKitVisualActionEditor in mActionEditors)
                {
                    actionKitVisualActionEditor.OnInspectorGUI();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            this.ExecuteRenderEndCommand();
        }

        public Action OnEventDelete;

        public void DeleteAllActions()
        {
            foreach (var editor in mActionEditors)
            {
                mActionEditors.Remove(editor);
                mScript.Acitons.Remove(editor.target as ActionKitVisualAction);
                DestroyImmediate(editor.target);
            }
        }
    }
}
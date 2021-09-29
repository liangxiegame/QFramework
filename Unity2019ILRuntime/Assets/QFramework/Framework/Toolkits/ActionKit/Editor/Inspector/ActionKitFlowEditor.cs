/****************************************************************************
 * Copyright (c) 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QFramework
{
    [CustomEditor(typeof(ActionKitFlow))]
    public class ActionKitFlowEditor : EasyInspectorEditor
    {
        private List<ActionKitVisualEventEditor> Editors = new List<ActionKitVisualEventEditor>();
        

        private bool mInited = false;
        private void OnEnable()
        {

            if (mInited) return;
            if (!mScript) return;

            mInited = true;
            Editors.Clear();
            foreach (var actionKitVisualEvent in mScript.Events)
            {
                AddEvent(actionKitVisualEvent);
            }
            
            this.AddChild(
                EasyIMGUI.Horizontal()
                    .AddChild(EasyIMGUI.Label().Text("Events").FontBold().FontSize(12))
                    .AddChild(EasyIMGUI.FlexibleSpace())
                    .AddChild(EasyIMGUI.Button().Text("+").OnClick(() =>
                    {
                        EventBrowser.Open((t) =>
                        {
                            var e = mScript.gameObject.AddComponent(t) as ActionKitVisualEvent;
                            e.hideFlags = HideFlags.HideInInspector;
                    
                            mScript.Events.Add(e);
                            AddEvent(e);
                    
                            EditorUtility.SetDirty(target);
                            UnityEditor.SceneManagement.EditorSceneManager
                                .MarkSceneDirty(SceneManager.GetActiveScene());
                        });
                    }))
                );
        }
        
        private void OnDisable()
        {
            this.GetLayout().Clear();
            
            Editors.Clear();
        }

        void AddEvent(ActionKitVisualEvent e)
        {
            var editor = CreateEditor(e,typeof(ActionKitVisualEventEditor)) as ActionKitVisualEventEditor;
            editor.OnEventDelete = () =>
            {
                this.PushRenderEndCommand(() =>
                {
                    editor.DeleteAllActions();
                    
                    Editors.Remove(editor);
                    mScript.Events.Remove(editor.target as ActionKitVisualEvent);
                    DestroyImmediate(editor.target);

                    Save();
                });
            };
            Editors.Add(editor);
        }

        ActionKitFlow mScript
        {
            get
            {
                var script = target as ActionKitFlow;
                
                if (!script) return script;

                if (script.Events == null)
                {
                    script.Events = new List<ActionKitVisualEvent>();
                }
                
                return script;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.GetLayout().DrawGUI();

            foreach (var actionKitVisualEventEditor in Editors)
            {
                actionKitVisualEventEditor.OnInspectorGUI();
            }

            this.ExecuteRenderEndCommand();
        }
    }
}
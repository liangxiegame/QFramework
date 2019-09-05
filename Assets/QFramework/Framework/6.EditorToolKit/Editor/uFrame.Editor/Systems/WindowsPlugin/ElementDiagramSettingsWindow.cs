using System;
using System.Linq;
using Invert.Common;
using QF.GraphDesigner;
using Invert.uFrame.Editor;
using UnityEditor;
using UnityEngine;

public class ElementDiagramSettingsWindow : EditorWindow
{
    private TextAsset _ChangeLog;

    private Vector2 _ChangeLogScrollPosition;

    private Rect _MainAreaRect = new Rect(4, 48, 512, 345);

    public DiagramViewModel DesignerData { get; set; }

    private bool _ViewingReadme;
    //private ElementsDesigner _designerWindow;

    //public ElementsDesigner DesignerWindow
    //{
    //    get { return _designerWindow ?? (_designerWindow =   EditorWindow.GetWindow<ElementsDesigner>()); }
    //    set { _designerWindow = value; }
    //}

    internal static void ShowWindow(DiagramViewModel diagram)
    {
        var window = GetWindow<ElementDiagramSettingsWindow>();
        window.title = "Settings";
        //window.minSize = window.maxSize = new Vector2(400, 400);
        window.DesignerData = diagram;
        window.Show();
    }

    private void OnEnable()
    {
        //minSize = new Vector2(520, 400);
       // maxSize = new Vector2(520, 400);
       
        position = new Rect(position.x, position.y, 520, 400);
    }

    public static void DrawTitleBar(string subTitle)
    {
        //GUI.Label();
        ElementDesignerStyles.DoTilebar(subTitle);
    }

    public void OnGUI()
    {
        try
        {
            if (DesignerData == null)
            {

                if (InvertGraphEditor.DesignerWindow.DiagramViewModel != null)
                {
                    DesignerData = InvertGraphEditor.DesignerWindow.DiagramViewModel;
                }
                else
                {
                    EditorGUILayout.HelpBox("Reopen this window.", MessageType.Info);
                    return;
                }
            }

            DrawTitleBar(DesignerData.Title);
            GUILayout.BeginArea(new Rect(5, 50, position.width - 10, this.position.height - 55), GUI.skin.box);


            _ChangeLogScrollPosition = GUILayout.BeginScrollView(_ChangeLogScrollPosition, false, false,
                GUILayout.Width(position.width - 15), GUILayout.Height(this.position.height - 15));

            //GUILayout.Label(_ChangeLog.text, EditorStyles.wordWrappedLabel);
            var s = DesignerData.Settings;
            
            
          
      
            //s.AssociationLinkColor = EditorGUILayout.ColorField("Association Link Color", s.AssociationLinkColor);
            //s.GridLinesColor = EditorGUILayout.ColorField("Grid Lines Color", s.GridLinesColor);
            //s.GridLinesColorSecondary = EditorGUILayout.ColorField("Grid Lines Secondary Color", s.GridLinesColorSecondary);
            //s.DefinitionLinkColor = EditorGUILayout.ColorField("Definition Link Color", s.DefinitionLinkColor);
            //s.InheritanceLinkColor = EditorGUILayout.ColorField("Inheritance Link Color", s.InheritanceLinkColor);
            //s.SceneManagerLinkColor = EditorGUILayout.ColorField("SceneManager Link Color", s.SceneManagerLinkColor);
            //s.SubSystemLinkColor = EditorGUILayout.ColorField("SubSystem Link Color", s.SubSystemLinkColor);
            //s.TransitionLinkColor = EditorGUILayout.ColorField("Transition Link Color", s.TransitionLinkColor);
            //s.ViewLinkColor = EditorGUILayout.ColorField("View Link Color", s.ViewLinkColor);
            s.CodeGenDisabled = EditorGUILayout.Toggle("Disable Code Generation", s.CodeGenDisabled);
            s.SnapSize = Math.Max(1, EditorGUILayout.IntField("Snap Size", s.SnapSize));
            s.Snap = EditorGUILayout.Toggle("Snap",s.Snap);
         
            //var pathStrategies = 
            //    InvertApplication.Container.Mappings.Where(p => p.Key.Item1 == typeof (ICodePathStrategy)).ToArray();
            //var names = pathStrategies.Select(p => p.Name).ToArray();
            //var selected = Array.IndexOf(names, s.CodePathStrategyName);
            //var types = pathStrategies.Select(p => p.to);
            //EditorGUI.BeginChangeCheck();
            //var newIndex = EditorGUILayout.Popup("Generator Path Strategy", Math.Max(0, selected), names);
            //if (EditorGUI.EndChangeCheck())
            //{
            //    if (newIndex >= 0 && DesignerData.RefactorCount < 1)
            //    {
            //        if (names[newIndex] != s.CodePathStrategyName)
            //        {
            //            DesignerData.Settings.CodePathStrategyName = names[newIndex];
            //            EditorApplication.SaveAssets();
                        
            //            //var newStrategy = uFrameEditor.Container.Resolve<ICodePathStrategy>(names[newIndex]);
            //            //DesignerData.DiagramData.CodePathStrategy.MoveTo(DesignerData.CurrentRepository.GeneratorSettings, newStrategy, names[newIndex], this.DesignerWindow);
            //        }

            //    }
            //    else
            //    {
            //        EditorUtility.DisplayDialog("Save First",
            //            "Save your diagram first before changing the path strategy.", "OK");
            //    }
            //}
            
            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }
        catch(Exception ex)
        {
            //UnityEngine.Debug.LogException(ex);
        }
    }
}
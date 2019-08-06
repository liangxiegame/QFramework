using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Invert.Common.UI;
using QF.GraphDesigner;
using Invert.Data;
using UnityEditor;
using UnityEngine;


public class SelectedCodePreview : EditorWindow
{
    private List<IDrawer> _generatorDrawers;
    private CodeFileGenerator[] fileGenerators;
    private Vector2 _scrollPosition;

    [MenuItem("Window/uFrame/Code Preview Window #&p")]
    internal static void ShowWindow()
    {
        var window = GetWindow<SelectedCodePreview>();
        window.title = "Code Preview";
        // window.minSize = new Vector2(400, 500);

        window.Show();
    }

    public void OnGUI()
    {
        if (Issues)
        {
            EditorGUILayout.HelpBox("Fix Errors First", MessageType.Info);
        }
        if (GeneratorDrawers != null)
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            var rect = new Vector2(0f, 28f);
            foreach (var fileGenerator in GeneratorDrawers)
            {
                if (GUIHelpers.DoToolbarEx(fileGenerator.ViewModelObject.Name))
                {
                    var lastRect = new Rect(0f, 0f, Screen.width, Screen.height);

                    fileGenerator.Refresh(InvertGraphEditor.PlatformDrawer, rect);
                    rect.y += fileGenerator.Bounds.height;
                    GUILayoutUtility.GetRect(Screen.width, fileGenerator.Bounds.height);
                    fileGenerator.Draw(InvertGraphEditor.PlatformDrawer, 1f);
                    //EditorGUILayout.TextArea(fileGenerator.ToString());
                }
                rect.y += 28f;
            }
            GUILayout.EndScrollView();
        }

    }

    public void Update()
    {
        var vm = InvertGraphEditor.CurrentDiagramViewModel;
        if (vm == null) return;

        if (SelectedNode != InvertGraphEditor.CurrentDiagramViewModel.SelectedNode || SelectedNode == null)
        {

            SelectedItemChanged();
            Repaint();
        }

    }
    public List<IDrawer> GeneratorDrawers
    {
        get { return _generatorDrawers ?? (_generatorDrawers = new List<IDrawer>()); }
        set { _generatorDrawers = value; }
    }

    private void SelectedItemChanged()
    {

        GeneratorDrawers.Clear();
        fileGenerators = null;

        SelectedNode = InvertGraphEditor.CurrentDiagramViewModel.SelectedGraphItem;

        if (SelectedNode == null)
        {
            return;
        }
        //Issues = SelectedNode.Issues.Any(p => p.Siverity == ValidatorType.Error);
        //if (Issues) return;
        var item = SelectedNode == null ? null : SelectedNode.DataObject;
        
        fileGenerators = InvertGraphEditor.GetAllFileGenerators(InvertApplication.Container.Resolve<IGraphConfiguration>(), new [] {item as IDataRecord}, true).ToArray();

        foreach (var fileGenerator in fileGenerators)
        {
            var list = fileGenerator.Generators.ToList();
            if (item != null)
                list.RemoveAll(p => p.ObjectData != item);
            fileGenerator.Generators = list.ToArray();
            if (fileGenerator.Generators.Length < 1) continue;

            var syntaxViewModel = new SyntaxViewModel(fileGenerator.ToString(), fileGenerator.Generators[0].Filename, 0);
            var syntaxDrawer = new SyntaxDrawer(syntaxViewModel);

            GeneratorDrawers.Add(syntaxDrawer);
        }

    }

    public bool Issues { get; set; }

    public GraphItemViewModel SelectedNode { get; set; }
}
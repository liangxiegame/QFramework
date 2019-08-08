using System.Collections.Generic;
using System.Linq;
using Invert.Common;
using Invert.Common.UI;
using QF.GraphDesigner;
using QF.GraphDesigner.Unity;
using Invert.Data;
using QF;
using UnityEditor;
using UnityEngine;

public class InspectorPlugin : DiagramPlugin
    , IDrawInspector
    , IDrawExplorer
    , IDataRecordPropertyChanged
    , IDataRecordInserted
    , IDataRecordRemoved
    , IGraphSelectionEvents
    , INothingSelectedEvent
    , IWorkspaceChanged
    , IToolbarQuery
{
    private bool _graphsOpen;
    private static GUIStyle _item5;
    private static GUIStyle _item4;

    public override void Initialize(QFrameworkContainer container)
    {
        base.Initialize(container);

    }

    public override void Loaded(QFrameworkContainer container)
    {
        base.Loaded(container);
        Repository = container.Resolve<IRepository>();
        WorkspaceService = container.Resolve<WorkspaceService>();
    }

    public WorkspaceService WorkspaceService { get; set; }

    public IRepository Repository { get; set; }

    public void DrawInspector(Rect rect)
    {
        var d = InvertGraphEditor.PlatformDrawer as UnityDrawer;
      //  d.DrawStretchBox(rect, CachedStyles.WizardListItemBoxStyle, 10);     
        if (Groups == null || !Groups.Any())
        {
                var textRect = rect;
                var cacheColor = GUI.color;
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.4f);
                d.DrawLabel(textRect, "No Items Selected", CachedStyles.WizardSubBoxTitleStyle, DrawingAlignment.MiddleCenter);
                GUI.color = cacheColor;
                return;
        }


        var groupRect = rect.WithHeight(30);

        foreach (var group in Groups)
        {

            if (GUIHelpers.DoToolbarEx(group.Key, groupRect))
            {
                var itemRect = rect.WithHeight(17).Below(groupRect);

                foreach (var item in group)
                {

                    Rect inspBounds;
                    if (item.InspectorType == InspectorType.GraphItems)
                    {
                        itemRect = itemRect.WithHeight(30);
                    }
                    else if (item.InspectorType == InspectorType.TextArea)
                    {
                        itemRect = itemRect.WithHeight(60);
                    }
                    else
                    {
                        itemRect = itemRect.WithHeight(17);
                    }
                    d.DrawInspector(itemRect, item, EditorStyles.label);
                    itemRect = itemRect.Below(itemRect).Translate(0,3).WithHeight(17);
                }

                groupRect = groupRect.Below(itemRect.Above(itemRect));

            }
            else
            {
                groupRect = groupRect.Below(groupRect);
            }

        }

       
     
    }

    public void UpdateItems()
    {
        if (WorkspaceService == null) return;
        if (WorkspaceService.CurrentWorkspace == null) return;

//        Items =
//            WorkspaceService.CurrentWorkspace.Graphs.SelectMany(p => p.NodeItems.OrderBy(x=>x.Name))
//                .OfType<GenericNode>()
//                .GroupBy(p => p.Graph.Name)
//                .OrderBy(p => p.Key).ToArray();

    }

    public IGrouping<string, GenericNode>[] Items { get; set; }

    public void PropertyChanged(IDataRecord record, string name, object previousValue, object nextValue)
    {
     
        UpdateItems();
        if (uFrameInspectorWindow.Instance != null)
            uFrameInspectorWindow.Instance.Repaint();
    }
    public virtual List<PropertyFieldViewModel> GetInspectorOptions(object obj)
    {
        List<PropertyFieldViewModel> list = new List<PropertyFieldViewModel>();
        if (obj == null) return list;
        var items = obj.GetType().GetPropertiesWithAttributeByType<InspectorProperty>().ToArray();
        for (int index = 0; index < items.Length; index++)
        {
            var item = items[index];

            var property = item.Key;
            var attribute = item.Value;
            var item1 = item;
            var fieldViewModel = new PropertyFieldViewModel()
            {
                Name = property.Name,
            };

            fieldViewModel.Getter = () => property.GetValue(obj, null);

            fieldViewModel.Setter = delegate(object d, object v)
            {
                fieldViewModel.PropertyInfo.SetValue(d, v, null);
            };

            fieldViewModel.PropertyInfo = item.Key;
            fieldViewModel.InspectorType = attribute.InspectorType;
            fieldViewModel.InspectorTip = attribute.InspectorTip;
            fieldViewModel.Type = property.PropertyType;
            fieldViewModel.DataObject = obj;
            fieldViewModel.CustomDrawerType = attribute.CustomDrawerType;
            fieldViewModel.CachedValue = fieldViewModel.Getter();
            list.Add(fieldViewModel);
        }
        return list;
    }
    private void UpdateSelection()
    {
        Groups = Selected.SelectMany(x => GetInspectorOptions(x)).GroupBy(p=>p.DataObject.GetType().Name).ToArray();
        

        if (uFrameInspectorWindow.Instance != null)
            uFrameInspectorWindow.Instance.Repaint();
    }

    public IGrouping<string, PropertyFieldViewModel>[] Groups { get; set; }


    public IDataRecord[] Selected { get; set; }

    public void RecordInserted(IDataRecord record)
    {
        UpdateItems(); 
        if (uFrameInspectorWindow.Instance != null)
            uFrameInspectorWindow.Instance.Repaint();
    }

    public void RecordRemoved(IDataRecord record)
    {
        UpdateItems(); if (uFrameInspectorWindow.Instance != null)
            uFrameInspectorWindow.Instance.Repaint();
    }

    public void SelectionChanged(GraphItemViewModel selected)
    {
        SelectItem(selected.DataObject as IDataRecord);
    }

    public void SelectItem(IDataRecord item)
    {
        var list = new List<IDataRecord>();

        if (WorkspaceService != null)
        {
            if (WorkspaceService.CurrentWorkspace != null)
            {
                list.Add(WorkspaceService.CurrentWorkspace);
                if (WorkspaceService.CurrentWorkspace.CurrentGraph != null)
                {
                    list.Add(WorkspaceService.CurrentWorkspace.CurrentGraph);
                }
            }
        }
        if (item != null)
        {
            list.Add(item);
        }
        Selected = list.ToArray();
        UpdateSelection();
    }
    public void DrawExplorer()
    {
        if (Repository == null) return;
        if (WorkspaceService == null) return;
        if (WorkspaceService.CurrentWorkspace == null) return;
       // if (Items == null) UpdateItems();
        
//        if (GUIHelpers.DoToolbarEx("Explorer"))
//        {
//            
//            EditorGUI.indentLevel++;
//            foreach (var group in Items)
//            {
//                //EditorPrefs.SetBool(group.Key, EditorGUILayout.Foldout(EditorPrefs.GetBool(group.Key), group.Key));
//                if (GUIHelpers.DoToolbarEx(group.Key) ) //EditorPrefs.GetBool(group.Key))
//                {
//                    EditorGUI.indentLevel++;
//                    foreach (var node in group)
//                    {
//                        EditorGUILayout.BeginHorizontal();
//                        GUILayout.Space(EditorGUI.indentLevel * 15f);
//                        var selected = Selected != null && Selected.Identifier == node.Identifier;
//                        if (GUILayout.Button(node.Name, selected ? Item5 : Item4))
//                        {
//                            Selected = node;
//                            UpdateSelection(null);
//                        } EditorGUILayout.EndHorizontal();
//                    }
//                    EditorGUI.indentLevel--;
//                }
//
//            } EditorGUI.indentLevel--;
//        }
    }
    public static GUIStyle Item4
    {
        get
        {
            if (_item4
                == null)
                _item4 = new GUIStyle
                {
                    normal = { background = ElementDesignerStyles.GetSkinTexture("Item4"), textColor = Color.white },
                    active = { background = ElementDesignerStyles.GetSkinTexture("EventButton"), textColor = Color.white },
                    stretchHeight = true,
                    stretchWidth = true,
                    fontSize = 12,
                    fixedHeight = 20f,
                    padding = new RectOffset(5, 0, 0, 0),
                    alignment = TextAnchor.MiddleLeft
                }.WithFont("Verdana", 12);

            return _item4;
        }
    }
    public static GUIStyle Item5
    {
        get
        {
            if (_item5
                == null)
                _item5= new GUIStyle
                {
                    normal = { background = ElementDesignerStyles.GetSkinTexture("Item1"), textColor = Color.white },
                    active = { background = ElementDesignerStyles.GetSkinTexture("EventButton"), textColor = Color.white },
                    stretchHeight = true,
                    stretchWidth = true,
                    padding = new RectOffset(5,0,0,0),
                    fontSize = 12,
                    fixedHeight = 20f,
                    alignment = TextAnchor.MiddleLeft
                }.WithFont("Verdana", 12);

            return _item5;
        }
    }
    public void WorkspaceChanged(Workspace workspace)
    {
        SelectItem(null);
    }

    public void NothingSelected()
    {
        SelectItem(null);
    }
    static bool IsWindowOpen<WindowType>() where WindowType : EditorWindow
    {
        WindowType[] windows = Resources.FindObjectsOfTypeAll<WindowType>();
        return windows != null && windows.Length > 0;

    }
    public void QueryToolbarCommands(ToolbarUI ui)
    {
        var isInspectorWindowOpened = IsWindowOpen<uFrameInspectorWindow>();
        var isIssuesWindowOpened = IsWindowOpen<uFrameIssuesWindow>();
        ui.AddCommand(new ToolbarItem()
        {
            Title = "Inspector",
            Checked = isInspectorWindowOpened,
            Position = ToolbarPosition.BottomRight,
            Command = new LambdaCommand("Show", () =>
            {
                var window = EditorWindow.GetWindow<uFrameInspectorWindow>();
                window.title = "Inspector";
                if (isInspectorWindowOpened)
                {
                    window.Close();
                }
            })
        }); 
        
        ui.AddCommand(new ToolbarItem()
        {
            Title = "Issues",
            Checked = isIssuesWindowOpened,
            Position = ToolbarPosition.BottomRight,
            Command = new LambdaCommand("Show", () =>
            {
                var window = EditorWindow.GetWindow<uFrameIssuesWindow>();
                window.title = "Issues";
                if (isIssuesWindowOpened)
                {
                    window.Close();
                }
            })
        }); 
    }
}
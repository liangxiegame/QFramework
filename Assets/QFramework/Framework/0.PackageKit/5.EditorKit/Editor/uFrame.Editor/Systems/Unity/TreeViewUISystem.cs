using System;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using QF.GraphDesigner.Systems.GraphUI;
using UnityEditor;
using UnityEngine;

/*
 * ACHTUNG: This is an ugly piece of code, but it does everything.
 */

public class TreeViewUISystem : DiagramPlugin, IDrawTreeView, IQueryDesignerWindowModalContent
{
    private IPlatformDrawer _platformDrawer;
    private Vector2 _scrollPos;
    private string _searchCriteria;
    private IItem[] _treeData;
    private TreeViewModel _treeViewModel;
    private WorkspaceService _workspaceService;


    public WorkspaceService WorkspaceService
    {
        get
        {
            return _workspaceService ?? (_workspaceService = InvertApplication.Container.Resolve<WorkspaceService>());
        }
        set { _workspaceService = value; }
    }

    public IPlatformDrawer PlatformDrawer
    {
        get { return _platformDrawer ?? (_platformDrawer = InvertApplication.Container.Resolve<IPlatformDrawer>()); }
        set { _platformDrawer = value; }
    }

    public IItem[] TreeData
    {
        get { return _treeData ?? (_treeData = WorkspaceService.CurrentWorkspace.Graphs.OfType<IItem>().ToArray()); }
        set { _treeData = value; }
    }

//    private void DrawTestTree(Rect obj)
//    {
//
//        PlatformDrawer.DrawTextbox("asdfsadf",new Rect().WithSize(80,30).Align(obj).Above(obj),_searchCriteria,ElementDesignerStyles.ItemTextEditingStyle,
//            (a, b) =>
//            {
//                _searchCriteria = a;
//                TreeViewModel.Refresh();
//            });
//        PlatformDrawer.DrawStretchBox(obj, CachedStyles.WizardSubBoxStyle, 13);
////
////        if (string.IsNullOrEmpty(_searchCriteria))
////        {
////            DrawTreeView(obj.PadSides(15), TreeData,ref _scrollPos, (m, i) => { });
////        }
////        else
////        {
////            DrawTreeView(obj.PadSides(15), TreeData, ref _scrollPos, (m, i) => { }, null, i => i.Title.Contains(_searchCriteria));
////        }
//
//        if (!string.IsNullOrEmpty(_searchCriteria))
//        {
//            TreeViewModel.Predicate = i => i.Title.Contains(_searchCriteria);
//        }
//        else
//        {
//            TreeViewModel.Predicate = null;
//        }
//
//      
//
//        DrawTreeView(obj.PadSides(15),TreeViewModel,(m,i)=>{});
//
//
//    }



    public int ItemHeight
    {
        get { return 17; }
    }

    public void DrawTreeView(Rect bounds, TreeViewModel viewModel, Action<Vector2, IItem> itemClicked,
        Action<Vector2, IItem> itemRightClicked = null)
    {
        var boundY = bounds.height;
        if (Event.current != null && Event.current.isKey && Event.current.rawType == EventType.KeyUp)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.UpArrow:
                    viewModel.MoveUp();
                    break;
                case KeyCode.DownArrow:
                    viewModel.MoveDown();
                    break;
                case KeyCode.RightArrow:
                {
                    var selectedContainer = viewModel.SelectedData as ITreeItem;
                    if (selectedContainer != null)
                    {
                        selectedContainer.Expanded = true;
                        viewModel.IsDirty = true;
                    }
                }
                    break;
                case KeyCode.LeftArrow:
                {
                    var selectedContainer = viewModel.SelectedData as ITreeItem;
                    if (selectedContainer != null)
                    {
                        selectedContainer.Expanded = false;
                        viewModel.IsDirty = true;
                    }
                }
                    break;
                case KeyCode.Return:
                    if(viewModel!= null)
                    viewModel.InvokeSubmit();
                    break;
                default:
                    break;
            }
        }
        //   PlatformDrawer.DrawLabel(new Rect().WithSize(100,100).InnerAlignWithBottomRight(bounds),"Total height: {0}, Total Items: {1}");
        var dirty = false;
        var position = bounds;
        var usedRect = position.WithWidth(Math.Max(bounds.width, PlatformDrawer.CalculateTextSize(viewModel.LargestString,CachedStyles.ListItemTitleStyle).x+5*viewModel.MaxIdentLevel)).Pad(0, 0, 15, 0).WithHeight(ItemHeight * viewModel.TreeData.Count(s => s.Visible));

        PlatformDrawer.DrawStretchBox(position.PadSides(-1),CachedStyles.WizardListItemBoxStyle,10);

        

        viewModel.Scroll = GUI.BeginScrollView(position, viewModel.Scroll, usedRect);

        var itemTemplateRect = bounds.WithHeight(ItemHeight);
        bool hasItems = false;

        foreach (var treeViewItem in viewModel.TreeData)
        {
            if (!treeViewItem.Visible) continue;
            hasItems = true;
            var data = treeViewItem.Data;

            var treeData = data as ITreeItem;

            var itemRect = itemTemplateRect.Pad(5*treeViewItem.Indent, 0, 5*treeViewItem.Indent, 0);
            
            var localItemY = itemRect.Translate(0, -position.yMin).y;
            if (localItemY > (viewModel.Scroll.y + position.height)) break;
            var imageRect = new Rect().WithSize(12, 12)
                .Align(itemRect)
                .AlignHorisonallyByCenter(itemRect)
                .Translate(5, 0);

            var labelRect =
                itemRect.WithWidth(
                    PlatformDrawer.CalculateTextSize(treeViewItem.Data.Title, CachedStyles.BreadcrumbTitleStyle).x)
                    .Translate(25, 0);

            if (treeViewItem == viewModel.ScrollTarget)
            {
                viewModel.Scroll = new Vector2(0, localItemY - ItemHeight * 5);
                viewModel.ScrollToItem(null);
            }

            if (treeViewItem.Selected)
            {
                PlatformDrawer.DrawStretchBox(itemRect, CachedStyles.WizardSubBoxStyle, 14);
            }

            PlatformDrawer.DrawLabel(labelRect, treeViewItem.Data.Title, CachedStyles.ListItemTitleStyle);
            PlatformDrawer.DrawImage(imageRect, treeViewItem.Icon, true);
            if (treeViewItem.ColorMark.HasValue)
            {
                var colorMarkRect = new Rect().WithSize(8, 8).InnerAlignWithCenterRight(itemRect).Translate(-24,0);
                PlatformDrawer.DrawRect(colorMarkRect,treeViewItem.ColorMark.Value);
            }

            if (viewModel.ShowToggle)
            {
                EditorGUI.BeginChangeCheck();
                var toggleRect = new Rect().WithSize(16, 16).InnerAlignWithUpperRight(itemRect).Translate(-50, 0);
                GUI.enabled = viewModel.AllowManualToggle;
                var toggle = GUI.Toggle(toggleRect, treeViewItem.IsChecked, "");
                GUI.enabled = true;

                if (treeViewItem.IsChecked != toggle)
                {
                    viewModel.ToggleItem(treeViewItem, toggle);
                    return;
                }

            }

            var item1 = treeViewItem;
            PlatformDrawer.DoButton(itemRect.Translate(25, 0), "", CachedStyles.ClearItemStyle,
                m =>
                {
                    viewModel.SelectedIndex = item1.Index;
                    //TODO PUBLISH EVENT
                    if (itemClicked != null) itemClicked(m, item1.Data);
                }, m => { if (itemRightClicked != null) itemRightClicked(m, item1.Data); });

      

            if (treeData != null)
                PlatformDrawer.DoButton(imageRect, "", CachedStyles.ClearItemStyle,
                    () =>
                    {
                        treeData.Expanded = !treeData.Expanded;
                        dirty = true;
                    });

            if (treeViewItem.Highlighted)
            {
                PlatformDrawer.DrawLine(new[]
                {
                    new Vector3(labelRect.x, itemRect.yMax - 1, 0),
                    new Vector3(labelRect.x + 75, itemRect.yMax - 1, 0)
                }, Color.cyan);
            }

            itemTemplateRect = itemTemplateRect.Below(itemTemplateRect);
        }

  


        GUI.EndScrollView();

        if (!hasItems)
        {
            var textRect = bounds;
            var cacheColor = GUI.color;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.4f);
            PlatformDrawer.DrawLabel(textRect, "No Items Found", CachedStyles.WizardSubBoxTitleStyle, DrawingAlignment.MiddleCenter);
            GUI.color = cacheColor;
            return;
        }

        if (dirty) viewModel.IsDirty = true;
    }

    // Make cool icons

    public void QueryDesignerWindowModalContent(List<DesignerWindowModalContent> content)
    {
//        content.Add(new DesignerWindowModalContent()
//        {
//            Drawer = DrawTestTree,
//            ZIndex = 1
//        });
    }
}
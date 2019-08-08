using System;
using System.Collections.Generic;
using System.Linq;
using Invert.Common;
using Invert.Common.UI;
using QF.GraphDesigner;
using Invert.uFrame.Editor;
using UnityEngine;

public class ItemSelectionWindow : SearchableScrollWindow
{
    public static void Init(string title, IEnumerable<IItem> items, Action<IItem> selected, bool allowNone = false)
    {
        // Get existing open window or if none, make a new one:
        var window = (ItemSelectionWindow)GetWindow(typeof(ItemSelectionWindow));
        window.title = title;
        window.Items = items;
        window.SelectedAction = selected;
        window.ApplySearch();
        window.minSize = new Vector2(200, 200);
        window.AllowNode = allowNone;
        window.Show();
    }

    public bool AllowNode { get; set; }

    public IEnumerable<IItem> Items { get; set; }
    public IItem[] ItemsArray { get; set; }
    public Action<IItem> SelectedAction { get; set; }
    public bool IsClosing { get; set; }
    public int HighlightedIndex { get; set; }

    public void MoveUp()
    {
        HighlightedIndex++;
    }

    public void MoveDown()
    {
        if (HighlightedIndex <= 1)
        {
            HighlightedIndex = 0;
        }
        else
        {
            HighlightedIndex--;
        }
    }
    protected override void ApplySearch()
    {
        if (Items == null) return;
        if (!string.IsNullOrEmpty(_SearchText))
        {
            var text = _SearchText.ToLower();
            //ItemsArray = Items.Where(p => p.SearchTag != null && p.SearchTag.Contains(_SearchText)).ToArray();
            ItemGroups = Items.Where(
                delegate(IItem p)
                {

                    var st = p.SearchTag;
                    if (st == null) return false;
                    st = st.ToLower();
                    return (st.Contains(text) || st == text);
                }).OrderBy(p => p.Title).GroupBy(p => p.Group).ToArray();
            HighlightedIndex = 0;
        }
        else
        {
            //ItemsArray = Items.ToArray();
            ItemGroups = Items.OrderBy(p=> p.Title).GroupBy(p => p.Group).ToArray();
        }
    }

    public IGrouping<string, IItem>[] ItemGroups { get; set; }

    public override void OnGUIScrollView()
    {
        if (Event.current != null && Event.current.isKey && Event.current.keyCode == KeyCode.UpArrow && Event.current.type == EventType.KeyUp)
        {
            MoveDown();
        }
        if (Event.current != null && Event.current.isKey && Event.current.keyCode == KeyCode.DownArrow && Event.current.type == EventType.KeyUp)
        {
            MoveUp();
        }
        if (AllowNode)
        {
            if (
                           GUIHelpers.DoTriggerButton(new UFStyle()
                           {
                               Label = "[NONE]",
                               IsWindow = true,
                               FullWidth = true,
                               BackgroundStyle = ElementDesignerStyles.EventButtonStyleSmall
                           }))
            {
                SelectedAction(null);
                IsClosing = true;
            }
        }
        if (ItemGroups == null)
        {
            return;
        }
        var index = 0;
        var isFirst = true;
        foreach (var group in ItemGroups)
        {
            if (group.Any())
            {
                if (string.IsNullOrEmpty(_SearchText))
                {
                    if (GUIHelpers.DoToolbarEx(group.Key))
                    {   
                        foreach (var item in group)
                        {
                            var item1 = item;
                            if (item == null) continue;
                            if (
                                GUIHelpers.DoTriggerButton(new UFStyle()
                                {
                                    Label = item.Title,
                                    IsWindow = true,
                                    FullWidth = true,

                                    BackgroundStyle = index == HighlightedIndex ? ElementDesignerStyles.Item1 : ElementDesignerStyles.EventButtonStyleSmall
                                }))
                            {
                                SelectedAction(item1);
                                IsClosing = true;
                            }
                            if (index == HighlightedIndex && Event.current != null && Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                            {
                                SelectedAction(item1);
                            }
                            isFirst = false;
                            index++;
                        }
                    }
                }
                else
                {
                
                        foreach (var item in group)
                        {
                            if (item == null) continue;
                            var item1 = item;
                            if (GUIHelpers.DoTriggerButton(new UFStyle()
                            {
                                Label = item.Group + " : " + item.Title, IsWindow = true, FullWidth = true,
                                BackgroundStyle = index == HighlightedIndex ? ElementDesignerStyles.Item1 : ElementDesignerStyles.EventButtonStyleSmall
                            }))
                            {
                                SelectedAction(item1);
                                IsClosing = true;
                            }
                            if (index == HighlightedIndex && Event.current != null && Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                            {
                                SelectedAction(item1);
                                IsClosing = true;
                            }
                            isFirst = false;
                            index++;
                        }
                    
                }
            

            }

        }
    }

    public override void OnGUI()
    {
        base.OnGUI();
        if (IsClosing == true)
        {
            IsClosing = false;
            Close();
        }
    }
}
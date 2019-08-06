using System;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using UnityEngine;

public class TreeViewModel
{
    private List<IItem> _data;
    private Func<IItem, bool> _predicate;
    private int _selectedIndex = -1;
    private List<TreeViewItem> _treeData;
    private string _singleItemIcon;

    public TreeViewModel()
    {
        IsDirty = true;
    }

    public Vector2 Scroll { get; set; }

    public List<IItem> Data
    {
        get { return _data; }
        set
        {
            _data = value;
            ConstructData();
            Refresh();
        }
    }

    public IEnumerable<IItem> CheckedData
    {
        get { return TreeData.Where(i => i.IsChecked).Select(i => i.Data); }
    }

    public Func<IItem, bool> InitialToggleSelector { get; set; }

    public Action<IItem, bool> OnItemToggleChanged { get; set; }

    public bool ShowToggle { get; set; }

    public bool AllowManualToggle { get; set; }

    public string SingleItemIcon
    {
        get { return _singleItemIcon ?? (_singleItemIcon = "DotIcon"); }
        set { _singleItemIcon = value; }
    }

    public Func<IItem, string> SingleIconSelector { get; set; }

    public IItem SelectedData
    {
        get { return SelectedItem != null ? SelectedItem.Data : null; }
    }

    public TreeViewItem SelectedItem
    {
        get
        {
            return SelectedIndex > -1 && SelectedIndex < TreeData.Count
                ? TreeData[SelectedIndex]
                : TreeData.FirstOrDefault(_ => !(_.Data is ITreeItem));
        }
    }

    public Func<IItem, Color?> ColorMarkSelector { get; set; } 

    public int SelectedIndex
    {
        get
        {
            if (_selectedIndex < 0 && TreeData.Count > 0)
            {
                if(Predicate != null)
                foreach (var treeViewItem in TreeData)
                {
                    if (treeViewItem.Visible && !(treeViewItem.Data is ITreeItem)) return treeViewItem.Index;
                }
                return 0;
            }
            return _selectedIndex;
        }
        set
        {
            if (_selectedIndex == value) return;
            _selectedIndex = value;  
            ScrollToItem(SelectedItem);
            IsDirty = true;
        }
    }

    public Action<IItem> Submit { get; set; }

    public List<TreeViewItem> TreeData
    {
        get { return _treeData ?? (_treeData = new List<TreeViewItem>()); }
        set { _treeData = value; }
    }

    public bool IsDirty { get; set; }

    public Func<IItem, bool> Predicate
    {
        get { return _predicate; }
        set
        {
            if (_predicate == value) return;
            _predicate = value;
            IsDirty = true;
        }
    }

    public void MoveUp()
    {
        var ndx = SelectedIndex - 1;
        while (ndx >= 0)
        {
            if (TreeData[ndx].Visible)
            {
                SelectedIndex = ndx;
                return;
            }
            ndx--;
        }

        ndx = TreeData.Count - 1;
        while (ndx >= SelectedIndex)
        {
            if (TreeData[ndx].Visible)
            {
                SelectedIndex = ndx;

                return;
            }
            ndx--;
        }
    }

    public void MoveDown()
    {
        var ndx = SelectedIndex + 1;
        while (ndx < TreeData.Count)
        {
            if (TreeData[ndx].Visible)
            {
                SelectedIndex = ndx;
                return;
            }
            ndx++;
        }

        ndx = 0;
        while (ndx <= SelectedIndex)
        {
            if (TreeData[ndx].Visible)
            {
                SelectedIndex = ndx;
                return;
            }
            ndx++;
        }
    }

    public void ConstructData()
    {
        if (Data == null) return;
        TreeData.Clear();
        foreach (var items in Data)
        {
         
            ExtractItems(TreeData, items, null, 0);
        }
    }

    public void Refresh()
    {
        LargestString = "";
        MaxIdentLevel = 0;
        if (Predicate == null)
        {
            foreach (var item in TreeData)
            {
                var data = item.Data;
                var treeData = data as ITreeItem;
                item.Visible = item.Parent == null || item.Parent.Visible && item.ParentData.Expanded;
                item.Icon = treeData == null || !treeData.Children.Any() ? (SingleIconSelector == null ? SingleItemIcon : SingleIconSelector(data)) : treeData.Expanded ? "MinusIcon_Micro" : "PlusIcon_Micro";
                item.ColorMark = ColorMarkSelector != null ? ColorMarkSelector(data) : null;
                item.Highlighted = false;
                item.Selected = SelectedIndex == item.Index;
                if (data.Title == null) continue;
                if (data.Title.Length > LargestString.Length) LargestString = data.Title;
                if (item.Visible && item.Indent > MaxIdentLevel) MaxIdentLevel = item.Indent;
            }
        }
        else
        {
            foreach (var item in TreeData)
            {
                var data = item.Data;
                var treeData = data as ITreeItem;
                var match = Predicate(data);
                item.Highlighted = match;
                item.Selected = SelectedIndex == item.Index;

                if (item.Parent != null && Predicate(item.Parent.Data) && item.ParentData.Expanded)
                {
                    //Show as a content of the parent
                    item.Visible = true;
                }
                else if (match)
                {
                    item.Visible = true;
                    //Force show parent
                    var parent = item.Parent;
                    if (parent != null)
                        while (parent != null)
                        {
                            parent.Visible = true;
                            (parent.Data as ITreeItem).Expanded = true;
                            parent = parent.Parent;
                        }
                }
                else
                {
                    item.Visible = false;
                }

                item.ColorMark = ColorMarkSelector != null ? ColorMarkSelector(data) : null;
                item.Icon = treeData == null || !treeData.Children.Any() ? (SingleIconSelector == null ? SingleItemIcon : SingleIconSelector(data)) : treeData.Expanded ? "MinusIcon_Micro" : "PlusIcon_Micro";
                
                if (data.Title == null) continue;
                if (data.Title.Length > LargestString.Length) LargestString = data.Title;
                if (item.Visible && item.Indent > MaxIdentLevel) MaxIdentLevel = item.Indent;

            }
        }
        IsDirty = false;
    }

    public void ExtractItems(List<TreeViewItem> items, IItem data, TreeViewItem parent, int ident)
    {
        var treeViewItem = new TreeViewItem
        {
            Data = data,
            Index = items.Count,
            Parent = parent,
            Indent = ident,
            IsChecked = InitialToggleSelector != null ? InitialToggleSelector(data) : false
        };
 
        items.Add(treeViewItem);

        var treeItem = data as ITreeItem;

        if (treeItem != null)
        {
            foreach (var child in treeItem.Children.OrderBy(c =>
            {
                var t = c as ITreeItem;
                return !(t != null && t.Children.Any());
            }).ThenBy(c=>c.Title))
            {
                ExtractItems(items, child, treeViewItem, ident + 1);
            }
        }
    }

    public int MaxIdentLevel { get; set; }
    public string LargestString { get; set; }

    public void InvokeSubmit()
    {
        if (Submit != null) Submit(SelectedData);
    }

    public void ExpandPathTo(TreeViewItem item)
    {
        var parent = item.ParentData;
        if (parent != null)
        {
            parent.Expanded = true;
            ExpandPathTo(item.Parent);
        }

    }


    public TreeViewItem ScrollTarget { get; set; }

    public void ScrollToItem(TreeViewItem item)
    {
        ScrollTarget = item;
    }

    public void ToggleItem(TreeViewItem treeViewItem,bool value)
    {
        if(treeViewItem.IsChecked == value) return;

        treeViewItem.IsChecked = value;
        if (OnItemToggleChanged != null) OnItemToggleChanged(treeViewItem.Data, value);

        if (treeViewItem.Data is ITreeItem)
        {
            foreach (var item in TreeData.ToArray())
            {
                if (item.Parent == treeViewItem)
                {
                    ToggleItem(item,value);
                }
            }
        }




    }
}
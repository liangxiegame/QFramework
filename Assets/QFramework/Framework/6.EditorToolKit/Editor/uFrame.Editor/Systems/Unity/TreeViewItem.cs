using QF.GraphDesigner;
using UnityEngine;

public class TreeViewItem
{
    public TreeViewItem Parent { get; set; }

    public ITreeItem ParentData
    {
        get { return Parent == null ? null : Parent.Data as ITreeItem; }
    }
    public bool IsChecked { get; set; }
    public IItem Data { get; set; }
    public int Index { get; set; }
    public bool Visible { get; set; }
    public int Indent { get; set; }
    public string Icon { get; set; }
    public bool Highlighted { get; set; }
    public bool Selected { get; set; }
    public Color? ColorMark { get; set; }
}
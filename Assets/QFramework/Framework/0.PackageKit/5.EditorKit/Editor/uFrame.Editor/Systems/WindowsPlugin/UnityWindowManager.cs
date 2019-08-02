using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QF.GraphDesigner;
using Invert.uFrame.Editor;
using UnityEditor;

public class UnityWindowManager : IWindowManager
{
    public void InitTypeListWindow(GraphTypeInfo[] typesInfoList, Action<GraphTypeInfo> action)
    {
        ElementItemTypesWindow.InitTypeListWindow("Choose Type", typesInfoList, (selected) =>
        {
            EditorWindow.GetWindow<ElementItemTypesWindow>().Close();
            InvertApplication.Execute(() =>
            {
                action(selected);
            });
        });
    }
    public void InitItemWindow<TItem>(IEnumerable<TItem> items, Action<TItem> action, bool allowNone = false)
        where TItem : IItem
    {
        ItemSelectionWindow.Init("Select Item",items.Cast<IItem>(), (item) =>
        {
            InvertApplication.Execute(() =>
            {
                action((TItem)item);
            });
        }, allowNone);
    }

    public void ShowHelpWindow(string helpProviderName, Type graphItemType)
    {
        UFrameHelp.ShowWindow(graphItemType);
    }
}
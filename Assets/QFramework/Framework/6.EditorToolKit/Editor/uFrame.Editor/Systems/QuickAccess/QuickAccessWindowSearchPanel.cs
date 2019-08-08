using System.Linq;
using Invert.Common;
using Invert.Common.UI;
using QF.GraphDesigner;
using Invert.Windows;
using UnityEditor;
using UnityEngine;

public class QuickAccessWindowSearchPanel : Area<QuickAccessWindowViewModel>
{

    private static GUIStyle _textFieldStyle;
    private Vector2 scrollPosition;
    private static GUIStyle _item5;
    private static GUIStyle _item1;
    private static GUIStyle _toolbar;

    public static GUIStyle TextFieldStyle
    {
        get
        {
            if (_textFieldStyle == null)
                _textFieldStyle = new GUIStyle(EditorStyles.textField)
                {
                    fontSize = 20,
                };
            return _textFieldStyle;
        }
    }
    public static GUIStyle SelectedStyle
    {
        get
        {
            if (_item1 == null)
                _item1 = new GUIStyle
                {
                    normal = { background = ElementDesignerStyles.GetSkinTexture("Toolbar"), textColor = new Color(0.85f,0.85f,0.85f) },
                    active = { background = ElementDesignerStyles.CommandBarClosedStyle.normal.background },
                    stretchHeight = true,
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 10,
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 25,
                    border = new RectOffset(3, 3, 3, 3),

              
                };

            return _item1;
        }
    }
    public static GUIStyle ItemStyle
    {
        get
        {
            if (_item5 == null)
                _item5 = new GUIStyle
                {
                    normal = { background = ElementDesignerStyles.GetSkinTexture("EventButton"), textColor = new Color(0.9f, 0.9f, 0.9f) },
                    active = { background = ElementDesignerStyles.CommandBarClosedStyle.normal.background },
                    stretchHeight = true,
                    fontSize = 10,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = 25,
                    border = new RectOffset(3, 3, 3, 3),

                };

            return _item5;
        }
    }
    public static GUIStyle GroupStyle
    {
        get
        {
            if (_toolbar == null)
                _toolbar = new GUIStyle
                {
                    normal = { background = ElementDesignerStyles.GetSkinTexture("Toolbar"), textColor = Color.white },
                    active = { background = ElementDesignerStyles.CommandBarClosedStyle.normal.background },
                    stretchHeight = true,
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = 35,
                    border = new RectOffset(3, 3, 3, 3),

                    padding = new RectOffset(25, 0, 5, 5)
                };

            return _toolbar;
        }
    }
    private void HandleInput(QuickAccessWindowViewModel data)
    {
        var evt = Event.current;
        if (evt.isKey && evt.type == EventType.KeyUp)
        {
            if (evt.keyCode == KeyCode.Return)
            {
                data.Execute();
                InvertApplication.SignalEvent<IWindowsEvents>(_ => _.WindowRequestCloseWithArea(this));
            }
            if (evt.keyCode == KeyCode.UpArrow)
            {
                data.MoveUp();
            }
            if (evt.keyCode == KeyCode.DownArrow)
            {
                data.MoveDown();
            }
            if (evt.keyCode == KeyCode.Escape)
                InvertApplication.SignalEvent<IWindowsEvents>(_=>_.WindowRequestCloseWithArea(this));

            InvertApplication.SignalEvent<IWindowsEvents>(_ => _.WindowRefresh(this));
        }
    }

    public override void Draw(QuickAccessWindowViewModel data)
    {
        HandleInput(data);
        EditorGUI.DrawRect(new Rect(0f,0f,Screen.width,Screen.height),new Color(0.3f,0.3f,0.5f) );
        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        GUI.SetNextControlName("SearchField");
        data.SearchText = GUILayout.TextField(data.SearchText, TextFieldStyle);
        if (EditorGUI.EndChangeCheck())
        {
            data.UpdateSearch();
        }
        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        //var y = GUILayoutUtility.GetLastRect().y;
        var itemHeight = 20;

        if (string.IsNullOrEmpty(data.SearchText))
        {
            foreach (var group in data.GroupedLaunchItems)
            {
                if (GUIHelpers.DoToolbarEx(group.Key,null,null,null,null,true,new Color(0.8f,0.8f,0.8f)))
                {
                    var i = 0;
                    foreach (var item in group)
                    {
                        //var isSelected = i == data.SelectedIndex;

                        if (GUILayout.Button(item.Title, ItemStyle))
                        {
                            data.SelectedIndex = i;
                            data.Execute(item);
                        }
                        i++;
                    }
                    
                }
            }
        }
        else
        {
            for (var i = 0; i < data.QuickLaunchItems.Count; i++)
            {
                var item = data.QuickLaunchItems[i];
                var isSelected = i == data.SelectedIndex;

                if (GUILayout.Button(item.Title, isSelected ? SelectedStyle : ItemStyle))
                {
                    data.SelectedIndex = i;
                    data.Execute();
                }

            }
        }

        
        GUILayout.EndScrollView();
        GUI.FocusControl("SearchField");
    }
}
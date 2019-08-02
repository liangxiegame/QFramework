using System.Collections.Generic;
using System.Linq;
using Invert.Common;
using Invert.Common.UI;
using QF.GraphDesigner;
using QF.GraphDesigner.Unity;
using Invert.Data;

using UnityEditor;
using UnityEngine;

public class ErrorsPlugin : DiagramPlugin
    , IDrawErrorsList
    , INodeValidated
    , IDataRecordRemoved
{
    private List<ErrorInfo> _errorInfo = new List<ErrorInfo>();
    private IPlatformDrawer _platformDrawer;
    private TreeViewModel _errorInfoList;
    private static GUIStyle _eventButtonStyleSmall;

    public List<ErrorInfo> ErrorInfo
    {
        get { return _errorInfo; }
        set { _errorInfo = value; }
    }

    public TreeViewModel ErrorInfoList
    {
        get { return _errorInfoList; }
        set { _errorInfoList = value; }
    }

    public IPlatformDrawer PlatformDrawer
    {
        get { return _platformDrawer ?? (_platformDrawer = Container.Resolve<IPlatformDrawer>()); }
        set { _platformDrawer = value; }
    }

    public static GUIStyle EventButtonStyleSmall
    {
        get
        {
            var textColor = Color.white;
            if (_eventButtonStyleSmall == null)
                _eventButtonStyleSmall = new GUIStyle
                {
                    normal = { background = ElementDesignerStyles.GetSkinTexture("EventButton"), textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black },
                    active = { background = ElementDesignerStyles.CommandBarClosedStyle.normal.background },
                    stretchHeight = true,

                    fixedHeight = 25,
                    border = new RectOffset(3, 3, 3, 3),

                    padding = new RectOffset(25, 0, 5, 5)
                };

            return _eventButtonStyleSmall;
        }
    }

    public void DrawErrors(Rect rect)
    {
        GUIHelpers.IsInsepctor = false;
        if (InvertGraphEditor.PlatformDrawer == null) return;

        var d = InvertGraphEditor.PlatformDrawer as UnityDrawer;
        d.DrawStretchBox(rect,CachedStyles.WizardListItemBoxStyle,10);        


        if (!ErrorInfo.Any())
        {
            var textRect = rect;
            var cacheColor = GUI.color;
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.4f);
            d.DrawLabel(textRect, "No Issues Found", CachedStyles.WizardSubBoxTitleStyle, DrawingAlignment.MiddleCenter);
            GUI.color = cacheColor;
            return;
        }

        if (ErrorInfoList == null) return;
        if (ErrorInfoList.IsDirty) ErrorInfoList.Refresh();

        Signal<IDrawTreeView>(_=>_.DrawTreeView(rect,ErrorInfoList, (m,i) =>
        {
            
            var bp = i as ErrorInfo;
            if (bp != null && bp.SourceNode != null)
            {
                Execute(new NavigateToNodeCommand()
                {
                    Node = bp.SourceNode,
                    Select = true
                });
            }
        }));


//        // var itemRect = new Rect(0f, 0f, rect.width, 25);
//        if (GUIHelpers.DoToolbarEx("Issues"))
//        {
//
//            foreach (var item in ErrorInfo)
//            {
//                var item1 = item;
//                var name = string.Empty;
//                var node = item.Record as GraphNode;
//                if (node != null)
//                {
//                    var filter = node.Filter;
//                    if (filter != null)
//                        name = filter.Name + ": ";
//                }
//                if (GUILayout.Button(name + item1.Message,EventButtonStyleSmall))
//                {
//                   
//                    if (node != null)
//                        Execute(new NavigateToNodeCommand()
//                        {
//                            Node = node
//                        });
//                }
//
//                var lastRect = GUILayoutUtility.GetLastRect();
//                PlatformDrawer.DrawImage(lastRect.WithWidth(lastRect.height).PadSides(3),"WarningIcon",true);
//                //InvertGraphEditor.PlatformDrawer.DoButton(itemRect.Pad(25f,0f,0f,0f),item.Message,CachedStyles.DefaultLabel, () =>
//                //{
//
//                //});
//                //itemRect.y += 26;
//                //var lineRect = itemRect;
//                //lineRect.height -= 24;
//                //InvertGraphEditor.PlatformDrawer.DrawRect(lineRect,new Color(0f,0f,0f,0.3f));
//            }
//        }
//        
    
    }

    public void NodeValidated(IDiagramNode node)
    {
        ErrorInfo.Clear();
        Signal<IQueryErrors>(_=>_.QueryErrors(ErrorInfo));
        UpdateList();
    }

    private void UpdateList()
    {
        if (ErrorInfoList == null) ErrorInfoList = new TreeViewModel();
        ErrorInfoList.SingleIconSelector = i =>
        {
            var item = i as ErrorInfo;
            if (item == null) return "DotIcon";
            if (item.Siverity == ValidatorType.Error) return "ErrorIcon_Micro";
            if (item.Siverity == ValidatorType.Warning) return "WarningIcon_Micro";
            return "DotIcon";
        };
        ErrorInfoList.Data = ErrorInfo.OfType<IItem>().ToList();
        ErrorInfoList.Submit = i =>
        {
            
            var bp = i as ErrorInfo;
            if (bp != null && bp.SourceNode != null)
            {
                Execute(new NavigateToNodeCommand()
                {
                    Node = bp.SourceNode,
                    Select = true
                });
            }
        };
    }

    public void RecordRemoved(IDataRecord record)
    {
        ErrorInfo.RemoveAll(p => p.Record.Identifier == record.Identifier);
    }

}
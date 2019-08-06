using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using Invert.Common;
using QF.GraphDesigner.Systems.GraphUI;
using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public class QuickAccessUISystem : DiagramPlugin,
        IQueryDesignerWindowOverlayContent, 
        IOverlayDrawer, 
        IShowSelectionMenu, 
        IHideSelectionMenu,
         IGraphSelectionEvents
    {

        public const int QuickAccessWidth = 300;
        public const int QuickAccessHeigth = 500;

        private IPlatformDrawer _platrformDrawer;
        private bool _focusNeeded;

        public Vector2? RequestPosition {get; set; }
        public bool EnableContent { get; set; }
        public TreeViewModel TreeModel { get; set; }
        public string SearchCriteria { get; set; }
        
        public IPlatformDrawer PlatformDrawer
        {
            get { return _platrformDrawer ?? (_platrformDrawer = InvertApplication.Container.Resolve<IPlatformDrawer>()); }
            set { _platrformDrawer = value; }
        }
       

        public void Draw(Rect bouds)
        {

            if (TreeModel == null) return;

            HandleInput(bouds);

            if (!EnableContent ) return;

            var searcbarRect = bouds.WithHeight(30).PadSides(5);

            var leftHeight = bouds.height - searcbarRect.height;

            var listRect = bouds.Below(searcbarRect).WithHeight(leftHeight * 0.6f).PadSides(5);

            var searchIconRect = new Rect().WithSize(30, 30).InnerAlignWithBottomRight(searcbarRect).AlignHorisonallyByCenter(searcbarRect).PadSides(10);

            var descriptionRect = bouds.Below(listRect).Clip(bouds).PadSides(5);

            GUI.SetNextControlName("SelectionMenu_Search");
            EditorGUI.BeginChangeCheck();
            var newSearchCrit = GUI.TextField(searcbarRect, SearchCriteria ?? "",ElementDesignerStyles.SearchBarTextStyle);
            PlatformDrawer.DrawImage(searchIconRect, "SearchIcon", true);
            if (EditorGUI.EndChangeCheck())
            {
                if (newSearchCrit != SearchCriteria)
                {
                    SearchCriteria = newSearchCrit;
                    TreeModel.SelectedIndex = -1;
                    if (string.IsNullOrEmpty(SearchCriteria))
                    {
                        TreeModel.Predicate = null;
                    }
                    else
                    {
                        var sc = SearchCriteria.ToLower();
                        TreeModel.Predicate = i =>
                        {
                            if (string.IsNullOrEmpty(i.Title)) return false;

                            if (
                                CultureInfo.CurrentCulture.CompareInfo.IndexOf(i.Title, SearchCriteria,
                                    CompareOptions.IgnoreCase) != -1) return true;

                            if (!string.IsNullOrEmpty(i.SearchTag) &&
                                CultureInfo.CurrentCulture.CompareInfo.IndexOf(i.SearchTag, SearchCriteria,
                                    CompareOptions.IgnoreCase) != -1) return true;

                            return false;
                        };
                    }
                    TreeModel.IsDirty = true;
                }
            }

            if (TreeModel.IsDirty) TreeModel.Refresh();

            InvertApplication.SignalEvent<IDrawTreeView>(_ =>
            {
                _.DrawTreeView(listRect, TreeModel, (m, i) => { SelectItem(i); });
            });

            if (TreeModel == null) return;

            var selectedItem = TreeModel.SelectedData as IItem;
            if (selectedItem != null)
            {
                PlatformDrawer.DrawStretchBox(descriptionRect, CachedStyles.WizardSubBoxStyle, 10);

                if (string.IsNullOrEmpty(selectedItem.Description))
                {
                    var textRect = descriptionRect;
                    var cacheColor = GUI.color;
                    GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.4f);
                    PlatformDrawer.DrawLabel(textRect, string.Format("{0}\nNo Description", selectedItem.Title), CachedStyles.WizardSubBoxTitleStyle, DrawingAlignment.MiddleCenter);
                    GUI.color = cacheColor;
                }
                else
                {
                    var titleRect = descriptionRect.WithHeight(40).PadSides(10);
                    var desctTextRect = descriptionRect.Below(titleRect).Clip(descriptionRect).PadSides(10);
                    PlatformDrawer.DrawLabel(titleRect, selectedItem.Title, CachedStyles.WizardActionTitleStyle, DrawingAlignment.TopLeft);
                    PlatformDrawer.DrawLabel(desctTextRect, selectedItem.Description ?? "Please add description to the item", CachedStyles.WizardActionTitleStyle, DrawingAlignment.TopLeft);
                }


            }

            if (_focusNeeded)
            {
                GUI.FocusControl("SelectionMenu_Search");
                _focusNeeded = false;
            }
        }

        private void HandleInput(Rect rect)
        {
            var evt = Event.current;
            if (evt == null) return;

            if (evt.isMouse && !rect.Contains(evt.mousePosition))
            {
                HideSelection();
                return;
            }

            if (evt.isKey && evt.rawType == EventType.KeyUp)
            {
                switch (evt.keyCode)
                {
                    case KeyCode.Escape:
                        HideSelection();
                        break;
                }
            }
            
        }

        public Rect CalculateBounds(Rect diagramRect)
        {
            if (RequestPosition.HasValue)
            {

                if (TreeModel == null) return new Rect(0,0,0,0);
                var selectedItem = TreeModel.SelectedData as IItem;

                var rect = new Rect().WithSize(QuickAccessWidth, selectedItem == null ? QuickAccessHeigth - 200 : QuickAccessHeigth).WithOrigin(RequestPosition.Value.x, RequestPosition.Value.y);
                if (rect.height > diagramRect.height) rect = rect.WithHeight(Mathf.Max(diagramRect.height, 230));

                if (rect.yMax > diagramRect.yMax) rect = rect.WithOrigin(rect.x, diagramRect.yMax - rect.height - 15 );
                if (rect.xMax > diagramRect.xMax) rect = rect.WithOrigin(diagramRect.xMax - rect.width - 15, rect.y);

                return rect;


            }
            var rectNorm =  new Rect().WithSize(QuickAccessWidth, QuickAccessHeigth).CenterInsideOf(diagramRect);
            if (rectNorm.height > diagramRect.height) rectNorm = rectNorm.WithHeight(Mathf.Max(diagramRect.height, 230));
            return rectNorm;

        }

        public void ShowSelectionMenu(SelectionMenu menu, Vector2? position = null, bool useWindow = false)
        {
            HideSelection();

            TreeModel = ConstructViewModel(menu);
            SearchCriteria = null;
            EnableContent = true;
            RequestPosition = position;
            _focusNeeded = true;
        }

        public void HideSelection()
        {
            RequestPosition = null;
            TreeModel = null;
            SearchCriteria = null;
            EnableContent = false;
        }

        public void SelectItem(IItem i)
        {
            var item = i as SelectionMenuItem;
            if (item == null || item.Action == null) return;
            Execute(new LambdaCommand("",item.Action));
            HideSelection();
       }

        protected TreeViewModel ConstructViewModel(SelectionMenu items)
        {
            var result = new TreeViewModel
            {
                Data = items.Items,
                Submit = SelectItem
            };
            return result;
        }

        public void QueryDesignerWindowOverlayContent(List<DesignerWindowOverlayContent> content)
        {
            if (EnableContent)
                content.Add(new DesignerWindowOverlayContent()
                {
                    Drawer = this,
                    DisableTransparency = true
                });
        }

        public void SelectionChanged(GraphItemViewModel selected)
        {
            HideSelection();
        }
    }
}

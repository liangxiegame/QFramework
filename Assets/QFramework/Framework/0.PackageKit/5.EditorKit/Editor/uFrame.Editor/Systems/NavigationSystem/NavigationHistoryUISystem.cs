using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Invert.Common;
using Invert.Common.UI;
using QF.GraphDesigner;
using QF.GraphDesigner.Unity;
using Invert.Data;
using QF;
using UnityEngine;

namespace Assets.Plugins.Editor.uFrame.Editor.Systems.NavigationSystem
{
    public class NavigationHistoryUISystem : DiagramPlugin, IDrawNavigationHistory, ICommandExecuted
    {
        private IPlatformDrawer _drawer;
        private TreeViewModel _navHistoryTree;
        private List<NavHistoryItem> _navHistoryItems;
        private IRepository _repository;
        private bool _updateRequired = true;

        public TreeViewModel NavHistoryTree
        {
            get { return _navHistoryTree; }
            set { _navHistoryTree = value; }
        }

        public List<NavHistoryItem> NavHistoryItems
        {
            get { return _navHistoryItems ?? (_navHistoryItems = new List<NavHistoryItem>()); }
            set { _navHistoryItems = value; }
        }

        public IPlatformDrawer Drawer
        {
            get { return _drawer ?? (_drawer = InvertApplication.Container.Resolve<IPlatformDrawer>()); }
            set { _drawer = value; }
        }


        public void DrawNavigationHistory(Rect rect)
        {
            GUIHelpers.IsInsepctor = false;
            if (Drawer == null) return;
            if (_updateRequired)
            {
                UpdateItems();
                _updateRequired = false;
            }

            Drawer.DrawStretchBox(rect, CachedStyles.WizardListItemBoxStyle, 10);


            if (!NavHistoryItems.Any())
            {
                var textRect = rect;
                var cacheColor = GUI.color;
                GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.4f);
                Drawer.DrawLabel(textRect, "No History", CachedStyles.WizardSubBoxTitleStyle, DrawingAlignment.MiddleCenter);
                GUI.color = cacheColor;
                return;
            }


            var clearButton = new Rect().WithSize(80, 33).InnerAlignWithBottomRight(rect).PadSides(5);
            Drawer.DoButton(clearButton, "Clear", ElementDesignerStyles.ButtonStyle,
                m =>
                {
                    Execute(new LambdaCommand("Clear Navigation History", () =>
                    {
                        Repository.RemoveAll<NavHistoryItem>();
                    }));
                });

            if (NavHistoryTree == null) return;
            if (NavHistoryTree.IsDirty) NavHistoryTree.Refresh();

            Signal<IDrawTreeView>(_ => _.DrawTreeView(rect.AddHeight(-28).PadSides(5), NavHistoryTree, (m, i) =>
            {

                               var bp = i as NavHistoryItem;
                if (bp != null)
                {
                    Execute(new NavigateByHistoryItemCommand()
                    {
                        Item = bp,
                    });
                }
            }));

        }

        private void UpdateList()
        {
            if (NavHistoryTree == null) NavHistoryTree = new TreeViewModel();
            NavHistoryTree.SingleIconSelector = i =>
            {
                var nhi = i as NavHistoryItem;
                if (nhi != null && nhi.IsActive) return "ForwardIcon";
                return "DotIcon";
            };
            NavHistoryTree.Data = NavHistoryItems.OrderBy(i=>i.Time).OfType<IItem>().ToList();
            NavHistoryTree.Submit = i =>
            {

                var bp = i as NavHistoryItem;
                if (bp != null)
                {
                    Execute(new NavigateByHistoryItemCommand()
                    {
                        Item = bp,
                    });
                }
            };
        }

        public void CommandExecuted(ICommand command)
        {
            _updateRequired = true;
        }

        private void UpdateItems()
        {
            NavHistoryItems.Clear();
            NavHistoryItems.AddRange(Repository.All<NavHistoryItem>());
            UpdateList();
        }

     
        public IRepository Repository
        {
            get { return _repository ?? (InvertApplication.Container.Resolve<IRepository>()); }
            set { _repository = value; }
        }
    }
}

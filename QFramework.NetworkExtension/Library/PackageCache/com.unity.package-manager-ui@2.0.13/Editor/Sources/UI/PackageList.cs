using UnityEngine.Experimental.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.PackageManager.UI
{
#if !UNITY_2018_3_OR_NEWER
    internal class PackageListFactory : UxmlFactory<PackageList>
    {
        protected override PackageList DoCreate(IUxmlAttributes bag, CreationContext cc)
        {
            return new PackageList();
        }
    }
#endif

    internal class PackageList : VisualElement
    {
#if UNITY_2018_3_OR_NEWER
        internal new class UxmlFactory : UxmlFactory<PackageList> {}
#endif

        public event Action<Package> OnSelected = delegate {};
        public event Action OnLoaded = delegate {};
        public event Action OnFocusChange = delegate {};

        private readonly VisualElement root;
        internal PackageItem selectedItem;
        private List<PackageGroup> Groups;

        public PackageList()
        {
            Groups = new List<PackageGroup>();

            root = Resources.GetTemplate("PackageList.uxml");
            Add(root);
            root.StretchToParentSize();

            UIUtils.SetElementDisplay(Empty, false);
            UIUtils.SetElementDisplay(NoResult, false);

            PackageCollection.Instance.OnPackagesChanged += SetPackages;

            RegisterCallback<AttachToPanelEvent>(OnEnterPanel);
            RegisterCallback<DetachFromPanelEvent>(OnLeavePanel);

            Reload();
        }

        public void GrabFocus()
        {
            if (selectedItem == null)
                return;

            selectedItem.Focus();
        }

        public void ShowResults(PackageItem item)
        {
            NoResultText.text = string.Empty;
            UIUtils.SetElementDisplay(NoResult, false);

            Select(item);

            EditorApplication.delayCall += ScrollIfNeeded;

            UpdateGroups();
        }

        public void ShowNoResults()
        {
            if (string.IsNullOrEmpty(PackageSearchFilter.Instance.SearchText))
                NoResultText.text = "No packages found";
            else
                NoResultText.text = string.Format("No results for \"{0}\"", PackageSearchFilter.Instance.SearchText);

            UIUtils.SetElementDisplay(NoResult, true);
            foreach (var group in Groups)
            {
                UIUtils.SetElementDisplay(group, false);
            }
            Select(null);
            OnSelected(null);
        }

        private void UpdateGroups()
        {
            foreach (var group in Groups)
            {
                UIUtils.SetElementDisplay(group, group.packageItems.Any(item => !UIUtils.IsElementDisplayNone(item)));
            }
        }

        private void OnEnterPanel(AttachToPanelEvent e)
        {
            panel.visualTree.RegisterCallback<KeyDownEvent>(OnKeyDownShortcut);
        }

        private void OnLeavePanel(DetachFromPanelEvent e)
        {
            panel.visualTree.UnregisterCallback<KeyDownEvent>(OnKeyDownShortcut);
        }

        private void ScrollIfNeeded()
        {
            EditorApplication.delayCall -= ScrollIfNeeded;

            if (selectedItem == null)
                return;

            var minY = List.worldBound.yMin;
            var maxY = List.worldBound.yMax;
            var itemMinY = selectedItem.worldBound.yMin;
            var itemMaxY = selectedItem.worldBound.yMax;
            var scroll = List.scrollOffset;

            if (itemMinY < minY)
            {
                scroll.y -= minY - itemMinY;
                if (scroll.y <= minY)
                    scroll.y = 0;
                List.scrollOffset = scroll;
            }
            else if (itemMaxY > maxY)
            {
                scroll.y += itemMaxY - maxY;
                List.scrollOffset = scroll;
            }
        }

        private static VisualElement FindNextSiblingByCondition(VisualElement element, Func<VisualElement, bool> matchFunc, bool reverseOrder)
        {
            if (element == null)
                return null;

            var parent = element.parent;
            var index = parent.IndexOf(element);
            if (reverseOrder)
            {
                for (var i = index - 1; i >= 0; i--)
                {
                    var nextElement = parent.ElementAt(i);
                    if (matchFunc(nextElement))
                        return nextElement;
                }
            }
            else
            {
                for (var i = index + 1; i < parent.childCount; i++)
                {
                    var nextElement = parent.ElementAt(i);
                    if (matchFunc(nextElement))
                        return nextElement;
                }
            }
            return null;
        }

        private static PackageItem FindNextVisiblePackageItem(PackageItem item, bool reverseOrder)
        {
            PackageItem nextVisibleItem = null;
            if (item.packageGroup.IsExpanded)
                nextVisibleItem = FindNextSiblingByCondition(item, UIUtils.IsElementVisible, reverseOrder) as PackageItem;

            if (nextVisibleItem == null)
            {
                Func<VisualElement, bool> expandedNonEmptyGroup = (element) =>
                {
                    var group = element as PackageGroup;
                    return group.IsExpanded && group.packageItems.Any(p => !UIUtils.IsElementDisplayNone(p));
                };
                var nextGroup = FindNextSiblingByCondition(item.packageGroup, expandedNonEmptyGroup, reverseOrder) as PackageGroup;
                if (nextGroup != null)
                    nextVisibleItem = reverseOrder ? nextGroup.packageItems.LastOrDefault(p => !UIUtils.IsElementDisplayNone(p)) : nextGroup.packageItems.FirstOrDefault(p => !UIUtils.IsElementDisplayNone(p));
            }
            return nextVisibleItem;
        }

        private static PackageItem FindPackageItemPageUpOrPageDown(PackageItem item, bool isPageUp)
        {
            PackageItem targetItem = null;
            if (item.packageGroup.IsExpanded)
                targetItem = (isPageUp ? item.parent.Children().FirstOrDefault(p => !UIUtils.IsElementDisplayNone(p)) 
                    : item.parent.Children().LastOrDefault(p => !UIUtils.IsElementDisplayNone(p))) as PackageItem;

            if (targetItem == item)
                targetItem = null;

            if (targetItem == null)
            {
                Func<VisualElement, bool> expandedNonEmptyGroup = (element) =>
                {
                    var group = element as PackageGroup;
                    return group.IsExpanded && group.packageItems.Any(p => !UIUtils.IsElementDisplayNone(p));
                };
                var nextGroup = FindNextSiblingByCondition(item.packageGroup, expandedNonEmptyGroup, isPageUp) as PackageGroup;
                if (nextGroup != null)
                    targetItem = (isPageUp ? nextGroup.packageItems.LastOrDefault(p => p != item && !UIUtils.IsElementDisplayNone(p))
                    : nextGroup.packageItems.FirstOrDefault(p => p != item && !UIUtils.IsElementDisplayNone(p))) as PackageItem;
            }
            return targetItem;
        }

        private void OnKeyDownShortcut(KeyDownEvent evt)
        {
            if (selectedItem == null)
                return;

            if (evt.keyCode == KeyCode.Tab)
            {
                OnFocusChange();
                evt.StopPropagation();
                return;
            }

            if (evt.keyCode == KeyCode.UpArrow)
            {
                var nextVisibleItem = FindNextVisiblePackageItem(selectedItem, true);
                if (nextVisibleItem != null)
                {
                    Select(nextVisibleItem);
                    ScrollIfNeeded();
                }
                evt.StopPropagation();
                return;
            }

            if (evt.keyCode == KeyCode.DownArrow)
            {
                var nextVisibleItem = FindNextVisiblePackageItem(selectedItem, false);
                if (nextVisibleItem != null)
                {
                    Select(nextVisibleItem);
                    ScrollIfNeeded();
                }
                evt.StopPropagation();
                return;
            }

            if (evt.keyCode == KeyCode.PageUp)
            {
                var targetItem = FindPackageItemPageUpOrPageDown(selectedItem, true);
                if (targetItem != null)
                {
                    Select(targetItem);
                    ScrollIfNeeded();
                }
                evt.StopPropagation();
                return;
            }

            if (evt.keyCode == KeyCode.PageDown)
            {
                var targetItem = FindPackageItemPageUpOrPageDown(selectedItem, false);
                if (targetItem != null)
                {
                    Select(targetItem);
                    ScrollIfNeeded();
                }
                evt.StopPropagation();
                return;
            }
        }

        private void Reload()
        {
            // Force a re-init to initial condition
            PackageCollection.Instance.UpdatePackageCollection();
            SelectLastSelection();
        }

        private void ClearAll()
        {
            List.Clear();
            Groups.Clear();

            UIUtils.SetElementDisplay(Empty, false);
            UIUtils.SetElementDisplay(NoResult, false);
        }

        private void SetPackages(IEnumerable<Package> packages)
        {
            if (PackageCollection.Instance.Filter == PackageFilter.Modules)
            {
                packages = packages.Where(pkg => pkg.IsBuiltIn);
            }
            else if (PackageCollection.Instance.Filter == PackageFilter.Unity)
            {
                packages = packages.Where(pkg => !pkg.IsBuiltIn && pkg.IsUnityPackage && !(pkg.Current != null && (pkg.Current.IsLocal || pkg.Current.IsInDevelopment)));
            }
            else if (PackageCollection.Instance.Filter == PackageFilter.Other)
            {
                packages = packages.Where(pkg => !pkg.IsBuiltIn && !pkg.IsUnityPackage && !(pkg.Current != null && (pkg.Current.IsLocal || pkg.Current.IsInDevelopment)));
            }
            else // PackageCollection.Instance.Filter == PackageFilter.Local
            {
                packages = packages.Where(pkg => !pkg.IsBuiltIn && pkg.Current != null);
            }

            OnLoaded();
            ClearAll();

            var lastSelection = PackageCollection.Instance.SelectedPackage;
            Select(null);

            PackageItem defaultSelection = null;

            foreach (var package in packages)
            {
                var item = AddPackage(package);

                if (null == selectedItem && defaultSelection == null)
                    defaultSelection = item;

                if (null == selectedItem && !string.IsNullOrEmpty(lastSelection) && package.Name.Equals(lastSelection))
                    Select(item);
            }

            if (selectedItem == null)
                Select(defaultSelection);

            PackageFiltering.FilterPackageList(this);
        }

        public void SelectLastSelection()
        {
            var lastSelection = PackageCollection.Instance.SelectedPackage;
            if (lastSelection == null)
                return;

            var list = List.Query<PackageItem>().ToList();
            PackageItem defaultSelection = null;

            foreach (var item in list)
            {
                if (defaultSelection == null)
                    defaultSelection = item;

                if (!string.IsNullOrEmpty(lastSelection) && item.package.Name.Equals(lastSelection))
                {
                    defaultSelection = item;
                    break;
                }
            }

            selectedItem = null;
            Select(defaultSelection);
        }

        private PackageItem AddPackage(Package package)
        {
            var groupName = PackageCollection.GetGroupName(package);
            var group = GetOrCreateGroup(groupName, package.IsBuiltIn || (PackageCollection.Instance.Filter == PackageFilter.Unity && package.IsUnityPackage));
            var packageItem = group.AddPackage(package);

            packageItem.OnSelected += Select;

            return packageItem;
        }

        private PackageGroup GetOrCreateGroup(string groupName, bool hidden)
        {
            foreach (var g in Groups)
            {
                if (g.name == groupName)
                    return g;
            }

            var group = new PackageGroup(groupName, hidden);
            if (!hidden)
            {
                group.OnGroupToggle += value =>
                {
                    if (value)
                    {
                        PackageCollection.Instance.CollapsedGroups.Remove(group.name);

                        if (group.Contains(selectedItem))
                            EditorApplication.delayCall += ScrollIfNeeded;
                    }
                    else
                    {
                        if (!PackageCollection.Instance.CollapsedGroups.Contains(group.name))
                            PackageCollection.Instance.CollapsedGroups.Add(group.name);
                    }
                };
            }

            group.SetExpanded(!PackageCollection.Instance.CollapsedGroups.Contains(groupName));

            Groups.Add(group);
            List.Add(group);
            return group;
        }

        private void Select(PackageItem packageItem)
        {
            if (packageItem == selectedItem)
            {
                if (selectedItem != null)
                    selectedItem.SetSelected(true);
                return;
            }

            var selectedPackageName = packageItem != null ? packageItem.package.Name : null;
            PackageCollection.Instance.SelectedPackage = selectedPackageName;

            if (selectedItem != null)
                selectedItem.SetSelected(false); // Clear Previous selection

            selectedItem = packageItem;
            if (selectedItem == null)
            {
                OnSelected(null);
                return;
            }

            selectedItem.SetSelected(true);
            ScrollIfNeeded();
            OnSelected(selectedItem.package);
        }

        private ScrollView List { get { return root.Q<ScrollView>("scrollView"); } }
        private VisualElement Empty { get { return root.Q<VisualElement>("emptyArea"); } }
        private VisualElement NoResult { get { return root.Q<VisualElement>("noResult"); } }
        private Label NoResultText { get { return root.Q<Label>("noResultText"); } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor.PackageManager.UI
{
#if !UNITY_2018_3_OR_NEWER
    internal class ArrowToggleFactory : UxmlFactory<ArrowToggle>
    {
        protected override ArrowToggle DoCreate(IUxmlAttributes bag, CreationContext cc)
        {
            return new ArrowToggle();
        }
    }
#endif

    internal class ArrowToggle : TextElement
    {
#if UNITY_2018_3_OR_NEWER
        internal new class UxmlFactory : UxmlFactory<ArrowToggle> {}
#endif
        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        private bool m_Expanded;
        public bool expanded
        {
            get { return m_Expanded; }
            set
            {
                m_Expanded = value;

                if (m_Expanded)
                {
                    SetDirection(Direction.Down);
                    RemoveFromClassList("collapsed");
                    AddToClassList("expanded");
                }
                else
                {
                    SetDirection(Direction.Right);
                    RemoveFromClassList("expanded");
                    AddToClassList("collapsed");
                }
            }
        }
        public ArrowToggle()
            : this(Direction.Right, "arrow")
        {
        }


        public ArrowToggle(Direction direction, string defaultClass)
        {
            AddToClassList(defaultClass);
            SetDirection(direction);
            expanded = true;
        }

        public void Toggle()
        {
            expanded = !expanded;
        }

        public void SetDirection(Direction direction)
        {
            if (direction == Direction.Left)
                text = "◄";
            else if (direction == Direction.Right)
                text = "►";
            else if (direction == Direction.Up)
                text = "▲";
            else if (direction == Direction.Down)
                text = "▼";
        }
    }

#if !UNITY_2018_3_OR_NEWER
    internal class PackageGroupFactory : UxmlFactory<PackageGroup>
    {
        protected override PackageGroup DoCreate(IUxmlAttributes bag, CreationContext cc)
        {
            return new PackageGroup(bag.GetPropertyString("name"));
        }
    }
#endif

    internal class PackageGroup : VisualElement
    {
#if UNITY_2018_3_OR_NEWER
        internal new class UxmlFactory : UxmlFactory<PackageGroup> {}
#endif

        private const string k_HiddenHeaderClass = "hidden";

        public event Action<bool> OnGroupToggle = delegate {};

        private readonly VisualElement root;

        public IEnumerable<PackageItem> packageItems { get { return List.Children().Cast<PackageItem>(); } }

        public bool IsExpanded { get { return HeaderCaret != null && HeaderCaret.expanded; } }

        public PackageGroup() : this(string.Empty)
        {
        }

        public PackageGroup(string groupName, bool hidden = false)
        {
            name = groupName;
            root = Resources.GetTemplate("PackageGroup.uxml");
            Add(root);

            HeaderTitle.text = groupName;
            HeaderTitle.ShowTextTooltipOnSizeChange();

            if (hidden)
                HeaderContainer.AddToClassList(k_HiddenHeaderClass);
            HeaderContainer.RegisterCallback<MouseDownEvent>(TogglePackageGroup);
        }

        private void TogglePackageGroup(MouseDownEvent evt)
        {
            if (evt.button == 0)
            {
                SetExpanded(!HeaderCaret.expanded);
                if (OnGroupToggle != null)
                    OnGroupToggle.Invoke(HeaderCaret.expanded);
            }
        }

        public bool Contains(PackageItem item)
        {
            return List.Contains(item);
        }

        public void SetExpanded(bool value)
        {
            if (HeaderCaret == null)
                return;

            HeaderCaret.expanded = value;
            if (value)
            {
                RemoveFromClassList("collapsed");
                AddToClassList("expanded");
            }
            else
            {
                RemoveFromClassList("expanded");
                AddToClassList("collapsed");
            }
        }

        internal PackageItem AddPackage(Package package)
        {
            var packageItem = new PackageItem(package) {packageGroup = this};
            List.Add(packageItem);
            return packageItem;
        }

        private VisualElement List { get { return root.Q<VisualElement>("groupContainer"); } }
        private VisualElement HeaderContainer { get { return root.Q<VisualElement>("headerContainer"); } }
        private Label HeaderTitle { get { return root.Q<Label>("headerTitle"); } }
        private ArrowToggle HeaderCaret { get { return root.Q<ArrowToggle>("headerCaret"); } }
    }
}

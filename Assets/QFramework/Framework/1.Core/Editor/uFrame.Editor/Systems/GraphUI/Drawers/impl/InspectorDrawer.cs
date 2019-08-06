using System.Collections.Generic;
using QF.GraphDesigner;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class InspectorDrawer : Drawer<InspectorViewModel>
    {
        private float _inspectorWidth = 200f;

        public InspectorDrawer(InspectorViewModel viewModel) : base(viewModel)
        {
            
        }

        public override void Refresh(IPlatformDrawer platform)
        {
            base.Refresh(platform);
        }

        public float InspectorWidth
        {
            get { return _inspectorWidth; }
            set { _inspectorWidth = value; }
        }

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);
            var x = position.x;
            var starty = position.y;
            this.Children.Clear();
            Children.AddRange(CreateDrawers());

            var y = position.y;
            var height = 0f;
            var maxWidth = 0f;
            foreach (var child in Children)
            {
                child.Refresh(platform, new Vector2(x + 10, y), hardRefresh);
                var rect = new Rect(child.Bounds);
                rect.y = y;
                child.Bounds = rect;
                y += child.Bounds.height;
                height += child.Bounds.height;
                if (child.Bounds.width > maxWidth)
                {
                    maxWidth = child.Bounds.width;
                }
            }

            this.Bounds = new Rect(x, starty, maxWidth + 24, height);
            foreach (var child in Children)
            {
                var newRect = new Rect(child.Bounds) { width = maxWidth };
                child.Bounds = newRect;
                child.OnLayout();
            }

       
            //Debug.Log("Bounds at " + position);
        }

        public override void OnLayout()
        {
            base.OnLayout();
            if (ViewModel.TargetViewModel != null)
            {
                var targetBounds = ViewModel.TargetViewModel.Bounds;
                this.Bounds = new Rect(targetBounds.x + targetBounds.width, targetBounds.y, Bounds.width, Bounds.height);
            }
            

        }

        private IEnumerable<IDrawer> CreateDrawers()
        {
            InvertApplication.Log("Creating drawers");
            foreach (var item in ViewModel.ContentItems)
            {
                var drawer = InvertGraphEditor.Container.CreateDrawer(item);
                if (drawer == null)
                {
                    InvertApplication.Log(string.Format("Couldn't create drawer for {0} make sure it is registered.",
                        item.GetType().Name));
                    continue;
                }

                yield return drawer;
            }
        }

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);

            //if (ViewModel.IsDirty)
            //{
            Refresh(platform);
            //  ViewModel.IsDirty = false;
            //}
            if (ViewModel.Visible)
            {
                var targetBounds = ViewModel.TargetViewModel.Bounds;
                this.Bounds = new Rect(targetBounds.x + targetBounds.width, targetBounds.y, Bounds.width, Bounds.height);
                var adjustedBounds = this.Bounds;
                adjustedBounds.width += 10f;
                adjustedBounds.x -= 5f;
                adjustedBounds.y -= 10f;
                adjustedBounds.height += 20f;
                platform.DrawStretchBox(adjustedBounds, CachedStyles.NodeBackground, 12f);
                foreach (var child in Children)
                {
                    child.Draw(platform, scale);
                }
            }
            
        }
    }
}
using UnityEngine;

namespace QF.GraphDesigner
{
    public abstract class GenericNodeDrawer<TData, TViewModel> : DiagramNodeDrawer<TViewModel>
        where TViewModel : DiagramNodeViewModel
        where TData : GenericNode
    {


        protected GenericNodeDrawer(TViewModel viewModel)
            : base(viewModel)
        {
        }

        protected GenericNodeDrawer()
        {

        }

        protected override void DrawBeforeBackground(IPlatformDrawer platform, Rect boxRect)
        {
            base.DrawBeforeBackground(platform, boxRect);
            if (NodeViewModel.IsFilter && !NodeViewModel.IsCurrentFilter)
            {
                platform.DrawStretchBox(boxRect.Add(new Rect(6, 6, -2, -1)), CachedStyles.NodeBackgroundBorderless, 18);
            }
        }

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);
            bool hasErrors = false;

            if (ViewModel.ShowHelp)
            {
                for (int index = 1; index <= NodeViewModel.ContentItems.Count; index++)
                {
                    var item = NodeViewModel.ContentItems[index - 1];
                    if (!string.IsNullOrEmpty(item.Comments))
                    {
                        var bounds = new Rect(item.Bounds);
                        bounds.width = item.Bounds.height;
                        bounds.height = item.Bounds.height;
                        bounds.y += (item.Bounds.height / 2) - 12;
                        bounds.x = this.Bounds.x - (bounds.width + 4);
                        platform.DrawLabel(bounds, index.ToString(), CachedStyles.NodeHeader13, DrawingAlignment.MiddleCenter);
                    }
                }
            }
        }

    }
}
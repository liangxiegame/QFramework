using System;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class PropertyFieldDrawer : ItemDrawer<PropertyFieldViewModel>
    {
        public PropertyFieldDrawer(PropertyFieldViewModel viewModelObject) : base(viewModelObject)
        {
        }

        public IInspectorPropertyDrawer CustomDrawer { get; set; }

        public override object TextStyle
        {
            get { return CachedStyles.DefaultLabel; }
        }

        private string  mLeft;
        private string  mRight;
        private Vector2 mLeftSize;
        private Vector2 mRightSize;

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);
            if (hardRefresh)
            {
                ViewModel.CachedValue = ViewModel.Getter();
            }

            if (ViewModel.CustomDrawerType != null && hardRefresh)
            {
                CustomDrawer = (IInspectorPropertyDrawer) Activator.CreateInstance(ViewModel.CustomDrawerType);
            }


            if (CustomDrawer != null)
            {
                CustomDrawer.Refresh(platform, position, this);

            }
            else
            {

                var bounds = new Rect(Bounds) {x = position.x, y = position.y};
                var labelSize = platform.CalculateTextSize(ViewModel.Name, CachedStyles.HeaderStyle);
                bounds.width = labelSize.x * 3;
                if (ViewModel.Type == typeof(Vector2) || ViewModel.Type == typeof(Vector3)
                ) // || ViewModel.Type == typeof(Quaternion))
                {
                    bounds.height *= 2f;
                }

                bounds.x += 3;
                Bounds = bounds;

                switch (ViewModel.InspectorType)
                {
                    case InspectorType.GraphItems:
                        Bounds = Bounds.WithHeight(30);
                        break;
                    case InspectorType.TextArea:
                        Bounds = Bounds.WithHeight(60);
                        break;
                }
            }
        }

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            if (CustomDrawer != null)
            {
                CustomDrawer.Draw(platform, scale, this);
            }
            else
            {
                Rect inspBounds;
                if (ViewModel.InspectorType == InspectorType.GraphItems)
                {
                    inspBounds = Bounds.WithHeight(30).CenterInsideOf(Bounds);
                }
                else if (ViewModel.InspectorType == InspectorType.TextArea)
                {
                    inspBounds = Bounds.WithHeight(60).CenterInsideOf(Bounds);
                }
                else
                {
                    inspBounds = Bounds.WithHeight(17).CenterInsideOf(Bounds);
                }

                platform.DrawPropertyField(inspBounds.Pad(7, 0, 14, 0), this.ViewModel, scale);
            }
        }
    }
}
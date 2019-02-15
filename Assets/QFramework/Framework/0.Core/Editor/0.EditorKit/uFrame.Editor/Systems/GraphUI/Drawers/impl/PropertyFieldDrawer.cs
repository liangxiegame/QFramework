using System;
using UnityEngine;

namespace QFramework.GraphDesigner
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
        private string _left;
        private string _right;
        private Vector2 _leftSize;
        private Vector2 _rightSize;
        //public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        //{
        //    base.Refresh(platform, position, hardRefresh);
        //    if (hardRefresh)
        //    {
              
        //    }


        //    Bounds = new Rect(position.x + 10, position.y, _leftSize.x + 5 + _rightSize.x + 40, 18);
        //}
        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position,hardRefresh);
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
                if (ViewModel.Type == typeof(Vector2) || ViewModel.Type == typeof(Vector3))// || ViewModel.Type == typeof(Quaternion))
                {
                    bounds.height *= 2f;
                }
                bounds.x += 3;
                Bounds = bounds;

                if (ViewModel.InspectorType == InspectorType.GraphItems)
                {
                    Bounds= Bounds.WithHeight(30);
                }
                else if (ViewModel.InspectorType == InspectorType.TextArea)
                {
                    Bounds = Bounds.WithHeight(60);
                }
                //
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
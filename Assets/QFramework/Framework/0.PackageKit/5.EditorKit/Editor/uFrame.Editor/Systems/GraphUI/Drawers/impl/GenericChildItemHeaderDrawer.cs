using System;
using Invert.Common;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class GenericChildItemHeaderDrawer : Drawer<GenericItemHeaderViewModel>
    {

        public GenericChildItemHeaderDrawer(GraphItemViewModel viewModelObject)
            : base(viewModelObject)
        {

        }

        public GenericChildItemHeaderDrawer(DiagramNodeViewModel viewModelObject)
            : base(viewModelObject)
        {
        }



        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);
            var width = platform.CalculateTextSize(ViewModel.Name, CachedStyles.HeaderStyle).x + 12;
            Bounds = new Rect(position.x, position.y, width + 20, 25);
            
        }

        public override void OnLayout()
        {
            base.OnLayout();
            //ViewModel.ConnectorBounds = Bounds;
            ViewModel.ConnectorBounds = new Rect(Bounds.x + 15, Bounds.y, Bounds.width -15, 28);
        }

        public Rect HeaderBounds { get; set; }

        public Rect _AddButtonRect;

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);
     
            platform.DrawStretchBox(Bounds.Scale(scale), CachedStyles.Item6, 0f);
            //platform.DrawStretchBox(Bounds,CachedStyles.Item1, 0);
            _AddButtonRect = new Rect
            {
                y = Bounds.y + ((Bounds.height/2) - 8),
                x = (Bounds.x + Bounds.width) - 22,
                width = 16,
                height = 16
            };
            var b = new Rect(Bounds);
            b.x += 8;
            b.width -= 27;
            platform.DrawLabel(b.Scale(scale), ViewModel.Name, ViewModel.IsBig ? CachedStyles.NodeStyleSchemaBold.SubTitleStyleObject : CachedStyles.HeaderStyle);
            
            if (ViewModel.AddCommand != null && ViewModel.Enabled)
            {

                platform.DoButton(_AddButtonRect.Scale(scale), string.Empty, CachedStyles.AddButtonStyle, () =>
                {
                    var vm = this.ViewModel.NodeViewModel as GraphItemViewModel;
                    vm.Select();
                    ViewModel.Add();
                    //InvertGraphEditor.ExecuteCommand(ViewModel.AddCommand);
                });
               
            }

        }


        public Type HeaderType { get; set; }


    }
}
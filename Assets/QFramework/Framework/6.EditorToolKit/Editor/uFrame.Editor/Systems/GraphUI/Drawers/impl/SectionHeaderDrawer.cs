using System;
using Invert.Common;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class SectionHeaderDrawer : Drawer<SectionHeaderViewModel>
    {

        public SectionHeaderDrawer(SectionHeaderViewModel viewModelObject)
            : base(viewModelObject)
        {
        }

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);


            var width = platform.CalculateTextSize(ViewModel.Name, CachedStyles.HeaderStyle).x + 45;
            //ElementDesignerStyles.HeaderStyle.CalcSize(new GUIContent(ViewModel.Name)).x + 20);

            Bounds = new Rect(position.x, position.y, width, 25);
        }

        public Rect _AddButtonRect;

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);
  
            _AddButtonRect = new Rect
            {
                y = Bounds.y + ((Bounds.height/2) - 8),
                x = Bounds.x + Bounds.width - 18,
                width = 16,
                height = 16
            };

            //.Scale(scale);
            //style.normal.textColor = textColorStyle.normal.textColor;
            //style.fontStyle = FontStyle.Bold;

//            GUI.Box(Bounds.Scale(scale), ViewModel.Name, style);
            platform.DrawLabel(Bounds.Scale(scale),ViewModel.Name,CachedStyles.ItemTextEditingStyle, DrawingAlignment.MiddleLeft);
            //if (ViewModel.AddCommand != null && ViewModel.Enabled)
            //{
            //    platform.DoButton(_AddButtonRect.Scale(scale), string.Empty, CachedStyles.AddButtonStyle, () =>
            //    {
            //        this.ViewModel.Select();
            //        // TODO 2.0 AddCommand ????
            //        //InvertGraphEditor.ExecuteCommand(ViewModel.AddCommand);
            //    });

            //}

        }


        public Type HeaderType { get; set; }


    }
}
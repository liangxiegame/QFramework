using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Invert.Common;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class NoteNodeDrawer : DiagramNodeDrawer<NoteNodeViewModel>
    {
        public NoteNodeDrawer(NoteNodeViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);

            Bounds = new Rect(ViewModel.Position.x, ViewModel.Position.y, Math.Max(NodeViewModel.Size.x, 64), Math.Max(NodeViewModel.Size.y, 64));

        }

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);
            
            var paddedBOunds = Bounds.PadSides(5);
            var headerBounds = paddedBOunds.WithHeight(20).Translate(0,10);
            Rect textBounds;
            var hasHeader = !string.IsNullOrEmpty(NodeViewModel.HeaderText);
            if (hasHeader)
            {
                textBounds = paddedBOunds.Below(headerBounds).Translate(0, 5).Clip(paddedBOunds);
            }
            else
            {
                textBounds = paddedBOunds;
            }

            if (NodeViewModel.ShowMark)
            {
                textBounds = textBounds.Pad(6, 0, 6, 0);
            }

            if (hasHeader)
            {
                var ts =platform.CalculateTextSize(NodeViewModel.HeaderText, CachedStyles.WizardSubBoxTitleStyle);
                platform.DrawLabel(headerBounds, NodeViewModel.HeaderText, CachedStyles.WizardSubBoxTitleStyle);

                var hmRect = new Rect().Align(headerBounds).WithSize(ts.x,2).Below(headerBounds).Translate(0,3);
                platform.DrawRect(hmRect, CachedStyles.GetColor(NodeColor.Gray));
                

            }

            if (NodeViewModel.ShowMark)
            {
                var markRect = textBounds.WithWidth(3).Pad(0,10,0,20).Translate(-6, 0);
                platform.DrawRect(markRect,CachedStyles.GetColor(NodeViewModel.MarkColor));
            }

            platform.DrawLabel(textBounds,ViewModel.Comments,CachedStyles.ListItemTitleStyle);
        }

        public override bool ShowHeader
        {
            get { return false; }
        }
    }


    public class ImageNodeDrawer : DiagramNodeDrawer<ImageNodeViewModel>
    {
        private Texture _image;


        public Texture Image
        {
            get { return _image ?? (_image = ElementDesignerStyles.GetSkinTexture(NodeViewModel.ImageName)); }
            set { _image = value; }
        }


        public ImageNodeDrawer(ImageNodeViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);

            Bounds = new Rect(ViewModel.Position.x, ViewModel.Position.y, Math.Max(NodeViewModel.Size.x, 64), Math.Max(NodeViewModel.Size.y, 64));

        }

        public override void OnMouseDoubleClick(MouseEvent mouseEvent)
        {
            base.OnMouseDoubleClick(mouseEvent);
            NodeViewModel.OpenImage();
        }


        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);
            
            var paddedBOunds = Bounds.PadSides(5);
            var headerBounds = paddedBOunds.WithHeight(20).Translate(0,10);
            Rect imageBounds;
            var hasHeader = !string.IsNullOrEmpty(NodeViewModel.HeaderText);
            var hasComments= !string.IsNullOrEmpty(NodeViewModel.Comments);
            if (hasHeader)
            {
                imageBounds = paddedBOunds.Below(headerBounds).Translate(0, 5).Clip(paddedBOunds);
            }
            else
            {
                imageBounds = paddedBOunds;
            }

            if (hasHeader)
            {
                var ts =platform.CalculateTextSize(NodeViewModel.HeaderText, CachedStyles.WizardSubBoxTitleStyle);
                platform.DrawLabel(headerBounds, NodeViewModel.HeaderText, CachedStyles.WizardSubBoxTitleStyle);

                var hmRect = new Rect().Align(headerBounds).WithSize(ts.x,2).Below(headerBounds).Translate(0,3);
                platform.DrawRect(hmRect, CachedStyles.GetColor(NodeColor.Gray));
                
            }

            if (!string.IsNullOrEmpty(NodeViewModel.ImageName) && Image != null)
            {
                platform.DrawImage(imageBounds,Image,true);
            }
            else platform.DrawLabel(imageBounds,"Image Not Found",CachedStyles.WizardSubBoxTitleStyle);
        }

        public override bool ShowHeader
        {
            get { return false; }
        }
    }
}
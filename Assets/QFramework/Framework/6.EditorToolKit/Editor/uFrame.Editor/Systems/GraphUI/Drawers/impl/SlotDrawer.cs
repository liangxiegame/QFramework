using Invert.Common;
using QF.GraphDesigner;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class SlotDrawer : SlotDrawer<GraphItemViewModel>
    {
        public SlotDrawer(GraphItemViewModel viewModelObject)
            : base(viewModelObject)
        {
        }


    }

    public class SlotDrawer<TViewModel> : Drawer<TViewModel> where TViewModel : GraphItemViewModel
    {

        public SlotDrawer(TViewModel viewModelObject)
            : base(viewModelObject)
        {

        }

        public override void OnMouseDown(MouseEvent mouseEvent)
        {
            base.OnMouseDown(mouseEvent);
            mouseEvent.NoBubble = true;
        }

        public override void OnMouseDoubleClick(MouseEvent mouseEvent)
        {
            //base.OnMouseDoubleClick(mouseEvent);
            
            InvertApplication.SignalEvent<IShowContextMenu>(_ => _.Show(mouseEvent, this.ViewModelObject));
     
        }

        public override void OnRightClick(MouseEvent mouseEvent)
        {
            base.OnRightClick(mouseEvent);
          
        }

        public override Rect Bounds
        {
            get { return ViewModelObject.Bounds; }
            set { ViewModelObject.Bounds = value; }
        }

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);
            var size = platform.CalculateTextSize(ViewModel.Name, CachedStyles.HeaderStyle);
            if (ViewModel.InputConnector != null)
            ViewModel.InputConnector.AlwaysVisible = true;
            if (ViewModel.OutputConnector != null)
                ViewModel.OutputConnector.AlwaysVisible = true;

            _labelWidth = 80f;
            _selectionWidth = platform.CalculateTextSize(IOViewmModel.SelectedItemName, CachedStyles.HeaderStyle).x + 45;

            if (IOViewmModel.AllowSelection)
            {

                Bounds = new Rect(position.x, position.y, _labelWidth + _selectionWidth , 25);
            }
            else
            {
                Bounds = new Rect(position.x, position.y, size.x + 38, 25);    
            }
            
        
            //if (ViewModel.OutputConnector != null)
            //{
            //    guiStyle.alignment = TextAnchor.MiddleRight;
            //   // Bounds = new Rect(position.x, position.y, size.x -25, 28);
            //}

        }

        public override void OnLayout()
        {
            base.OnLayout();
            ViewModel.ConnectorBounds = new Rect(Bounds.x + 15, Bounds.y, Bounds.width - 35, 28);
        }

        public InputOutputViewModel IOViewmModel
        {
            get { return ViewModelObject as InputOutputViewModel; }
        }

        public float _labelWidth = 100f;
        public float _selectionWidth = 125f;
        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);
         
            var adjusted = new Rect(Bounds).Pad(2,0,4,0);


            if(!string.IsNullOrEmpty(ViewModel.Description))
            platform.SetTooltipForRect(adjusted,ViewModel.Description);
           
            if (IOViewmModel.AllowSelection)
            {
                adjusted.width -= 15;
                adjusted.x += 15;
                platform.DrawColumns(adjusted.Scale(scale), new[] { _labelWidth, _selectionWidth }, 
                    _ =>
                    {
                        platform.DrawLabel(_, ViewModel.Name, CachedStyles.HeaderStyle, DrawingAlignment.MiddleLeft);
                    },
                    _ =>
                    {
                        var a = _;
                        a.width -= 4f;
                        a.x += 2f;
                        a.y += 4f;
                        a.height -= 8f;
                        platform.DrawRect(a, new Color(0f,0f,0f));
                        a.width -= 2f;
                        a.x += 1f;
                        a.y += 1f;
                        a.height -= 2f;
                        platform.DrawRect(a, new Color(0.2f,0.2f,0.2f));
                        platform.DoButton(a, IOViewmModel.SelectedItemName, null, IOViewmModel.SelectItem);
                    });
              

            }
            else
            {
                adjusted.width -= 40;
                adjusted.x += 15;
                platform.DrawLabel(adjusted.Scale(scale), ViewModel.Name, CachedStyles.HeaderStyle, ViewModel.OutputConnector != null ? DrawingAlignment.MiddleRight : DrawingAlignment.MiddleLeft);
            }
            //GUI.Label(adjusted.Scale(scale), ViewModel.Name, guiStyle);
        }
    }
}
using Invert.Common;
//using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class ConnectorDrawer : Drawer<ConnectorViewModel>
    {

        public override int ZOrder
        {
            get { return 10; }
        }

        public ConnectorDrawer(ConnectorViewModel viewModelObject) : base(viewModelObject)
        {
        }

        public int TextureWidth
        {
            get { return 16; }
        }

        public int TextureHeight
        {
            get { return 16; }
        }



        //TODO HACK: This hack is for the sake of performance, it updates the texture everytime vm setup is changed is changed
        //It is localized and spreads around the next set of fields + Texture property
        public ConnectorSide _oldSide;
        public ConnectorDirection _oldDirection;
        public bool _oldFilled;
        public Color _oldColor;
        public object _texture;

        public object Texture
        {
            get
            {
                var filled = ViewModel.HasConnections || ViewModel.IsMouseOver;

                if (_texture == null || _texture.Equals(null) || ViewModel.Side != _oldSide ||
                    ViewModel.Direction != _oldDirection || ViewModel.Color != _oldColor || filled != _oldFilled)
                {
                    _texture = ViewModel.StyleSchema.GetTexture(ViewModel.Side, ViewModel.Direction,
                   ViewModel.HasConnections || ViewModel.IsMouseOver, ViewModel.TintColor);

                    _oldFilled = filled;
                    _oldDirection = ViewModel.Direction;
                    _oldColor = ViewModel.Color;
                    _oldSide = ViewModel.Side;

                }
                  return _texture;


//
//                if (ViewModel.HasConnections || ViewModel.IsMouseOver)
//                {
//                    if (ViewModel.Direction == ConnectorDirection.Input)
//                    {
//                        switch (ViewModel.Side)
//                        {
//                            case ConnectorSide.Left:
//                                return "DiagramArrowRight";
//                                break;
//                            case ConnectorSide.Right:
//                                return "DiagramArrowLeft";
//                                break;
//                            case ConnectorSide.Bottom:
//                                return "DiagramArrowUp";
//                            case ConnectorSide.Top:
//                                return "DiagramArrowDown";
//                        }
//                    }
//                    else
//                    {
//                       // return "DiagramCircleConnector";
//                        switch (ViewModel.Side)
//                        {
//                            case ConnectorSide.Left:
//                                return "DiagramArrowLeft";
//                                break;
//                            case ConnectorSide.Right:
//                                return "DiagramArrowRight";
//                                break;
//                            case ConnectorSide.Bottom:
//                                return "DiagramArrowDown";
//                            case ConnectorSide.Top:
//                                return "DiagramArrowUp";
//                        }
//                    }
//                }
//               
//                if (ViewModel.Direction == ConnectorDirection.Input)
//                {
//                    switch (ViewModel.Side)
//                    {
//                        case ConnectorSide.Left:
//                            return "DiagramArrowRightEmpty";
//                            break;
//                        case ConnectorSide.Right:
//                            return "DiagramArrowLeftEmpty";
//                            break;
//                        case ConnectorSide.Bottom:
//                            return "DiagramArrowUp";
//                        case ConnectorSide.Top:
//                            return "DiagramArrowDown";
//                    }
//                }
//                else
//                {
//                    switch (ViewModel.Side)
//                    {
//                        case ConnectorSide.Left:
//                            return "DiagramArrowLeftEmpty";
//                            break;
//                        case ConnectorSide.Right:
//                            return "DiagramArrowRightEmpty";
//                            break;
//                        case ConnectorSide.Bottom:
//                            return "DiagramArrowDown";
//                        case ConnectorSide.Top:
//                            return "DiagramArrowUp";
//                    }
//                }
              //  return "DiagramArrowLeftEmpty";
            }
        }

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            _texture = null;
            base.Refresh(platform, position, hardRefresh);
        }

        public override Rect Bounds
        {
            get { return ViewModelObject.Bounds; }
            set { ViewModelObject.Bounds = value; }
        }

        public override void OnMouseDown(MouseEvent mouseEvent)
        {
            base.OnMouseDown(mouseEvent);
            if (ViewModel.Disabled) return;

            if (mouseEvent.MouseButton == 0 &&
                (ViewModel.Direction == ConnectorDirection.Output || ViewModel.Direction == ConnectorDirection.TwoWay))
            {
                mouseEvent.Begin(new ConnectionHandler(ViewModel.DiagramViewModel, ViewModel));
                mouseEvent.NoBubble = true;
                return;
            }
        }

        public override void OnRightClick(MouseEvent mouseEvent)
        {
            base.OnRightClick(mouseEvent);

        }

        public override void Refresh(IPlatformDrawer platform)
        {
            base.Refresh(platform);
            var connectorFor = ViewModel.ConnectorFor;
            var connectorBounds = ViewModel.ConnectorFor.ConnectorBounds;
            var forItem = connectorFor as ItemViewModel;
            if (forItem != null)
            {
                if (forItem.NodeViewModel.IsCollapsed)
                {
                    connectorBounds = forItem.NodeViewModel.ConnectorBounds;
                }
            }
            var nodePosition = connectorBounds;
            var texture = Texture;
            var pos = new Vector2(0f, 0f);

            if (ViewModel.Side == ConnectorSide.Left)
            {
                
                pos.x = nodePosition.x;
                pos.y = nodePosition.y + (nodePosition.height * ViewModel.SidePercentage);
                pos.y -= (TextureHeight / 2f);
                pos.x -= (TextureWidth) + 2;
            }
            else if (ViewModel.Side == ConnectorSide.Right)
            {
                pos.x = nodePosition.x + nodePosition.width;
                pos.y = nodePosition.y + (nodePosition.height * ViewModel.SidePercentage);
                pos.y -= (TextureHeight / 2f);
                pos.x += 2;
            }
            else if (ViewModel.Side == ConnectorSide.Bottom)
            {
                pos.x = nodePosition.x + (nodePosition.width * ViewModel.SidePercentage);
                pos.y = nodePosition.y + nodePosition.height;
                pos.x -= (TextureWidth / 2f);
                //pos.y += TextureHeight;
            }
            else if (ViewModel.Side == ConnectorSide.Top)
            {
                pos.x = nodePosition.x + (nodePosition.width * ViewModel.SidePercentage);
                pos.y = nodePosition.y;
                pos.x -= (TextureWidth / 2f);
                pos.y -= TextureHeight;
            }

            Bounds = new Rect(pos.x, pos.y, TextureWidth, TextureHeight);
            //if (!ViewModel.IsMouseOver)
            //{
            //    var mouseOverBounds = new Rect(Bounds);
            //    //mouseOverBounds.x -= mouseOverBounds.width*0.2f;
            //    mouseOverBounds.y += mouseOverBounds.height * 0.125f;
            //    mouseOverBounds.x += mouseOverBounds.width * 0.125f;
            //    //mouseOverBounds.width *= 0.75f;
            //    //mouseOverBounds.height *= 0.75f;
            //    Bounds = mouseOverBounds;
            //}
        }

        public override void OnLayout()
        {
            base.OnLayout();
            var connectorFor = ViewModel.ConnectorFor;
            var connectorBounds = ViewModel.ConnectorFor.ConnectorBounds;
            var forItem = connectorFor as ItemViewModel;
            if (forItem != null)
            {
                if (forItem.NodeViewModel.IsCollapsed)
                {
                    connectorBounds = forItem.NodeViewModel.ConnectorBounds;
                }
            }
            var nodePosition = connectorBounds;
            var texture = Texture;
            var pos = new Vector2(0f, 0f);

            if (ViewModel.Side == ConnectorSide.Left)
            {
                pos.x = nodePosition.x;
                pos.y = nodePosition.y + (nodePosition.height * ViewModel.SidePercentage);
                pos.y -= (TextureHeight / 2f);
                pos.x -= (TextureWidth) + 2;
            }
            else if (ViewModel.Side == ConnectorSide.Right)
            {
                pos.x = nodePosition.x + nodePosition.width;
                pos.y = nodePosition.y + (nodePosition.height * ViewModel.SidePercentage);
                pos.y -= (TextureHeight / 2f);
                pos.x += 2;
            }
            else if (ViewModel.Side == ConnectorSide.Bottom)
            {
                pos.x = nodePosition.x + (nodePosition.width * ViewModel.SidePercentage);
                pos.y = nodePosition.y + nodePosition.height;
                pos.x -= (TextureWidth / 2f);
                //pos.y += TextureHeight;
            }
            else if (ViewModel.Side == ConnectorSide.Top)
            {
                pos.x = nodePosition.x + (nodePosition.width * ViewModel.SidePercentage);
                pos.y = nodePosition.y;
                pos.x -= (TextureWidth / 2f);
                pos.y -= TextureHeight;
            }


            //if (ViewModel.IsMouseOver)
            //{
            //    var mouseOverBounds = new Rect(bounds);
            //    ////mouseOverBounds.x -= mouseOverBounds.width*0.2f;
            //    //mouseOverBounds.y += mouseOverBounds.height * 0.125f;
            //    //mouseOverBounds.x += mouseOverBounds.width * 0.125f;
            //    mouseOverBounds.width = 20;
            //    mouseOverBounds.height = 20;
            //    bounds = mouseOverBounds;
            //}

            //if (ViewModelObject.IsMouseOver)
            //{
            //    EditorGUI.DrawRect(Bounds.Scale(scale), Color.black);
            //}
            //if (!ViewModel.HasConnections)
            //if (!ViewModel.ConnectorFor.IsMouseOver && !ViewModel.ConnectorFor.IsSelected && !ViewModel.IsMouseOver) return;
            if (!ViewModel.AlwaysVisible)
            {
                if (!ViewModel.ConnectorFor.IsMouseOver && !ViewModel.ConnectorFor.IsSelected && !ViewModel.IsMouseOver && !ViewModel.HasConnections) return;
            }

            //if (ViewModel.HasConnections)
            //{
            //    platform.DrawImage(bounds, Texture, true);



            //}
            if (ViewModel.Direction == ConnectorDirection.Output && ViewModel.Side == ConnectorSide.Right)
            {
                Bounds = new Rect(pos.x, pos.y, TextureWidth, TextureHeight);
                //return;
            }
            else
            {
                Bounds = new Rect(pos.x, pos.y, TextureWidth, TextureHeight);
            }
        }

        //uncomment to debug connector bounds
        //Color bc = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value,0.3f);

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);
        
            //uncomment to debug connector bounds
            //platform.DrawRect(Bounds,bc);
            
            //InvertGraphEditor.PlatformDrawer.DrawConnector(scale, ViewModel);
            var connectorFor = ViewModel.ConnectorFor;
            var connectorBounds = ViewModel.ConnectorFor.ConnectorBounds;
            var forItem = connectorFor as ItemViewModel;
            if (forItem != null)
            {
                if (forItem.NodeViewModel.IsCollapsed)
                {
                    connectorBounds = forItem.NodeViewModel.ConnectorBounds;
                }
            }
            var nodePosition = connectorBounds;
            var texture = Texture;
            var pos = new Vector2(0f, 0f);
            
            if (ViewModel.Side == ConnectorSide.Left)
            {
                pos.x = nodePosition.x;
                pos.y = nodePosition.y + (nodePosition.height * ViewModel.SidePercentage);
                pos.y -= (TextureHeight / 2f);
                pos.x -= (TextureWidth) + 2;
            }
            else if (ViewModel.Side == ConnectorSide.Right)
            {
                pos.x = nodePosition.x + nodePosition.width;
                pos.y = nodePosition.y + (nodePosition.height * ViewModel.SidePercentage);
                pos.y -= (TextureHeight / 2f);
                pos.x += 2;
            }
            else if (ViewModel.Side == ConnectorSide.Bottom)
            {
                pos.x = nodePosition.x + (nodePosition.width * ViewModel.SidePercentage);
                pos.y = nodePosition.y + nodePosition.height;
                pos.x -= (TextureWidth / 2f);
                //pos.y += TextureHeight;
            }
            else if (ViewModel.Side == ConnectorSide.Top)
            {
                pos.x = nodePosition.x + (nodePosition.width * ViewModel.SidePercentage);
                pos.y = nodePosition.y;
                pos.x -= (TextureWidth / 2f);
                pos.y -= TextureHeight;
            }
         
           
            //if (ViewModel.IsMouseOver)
            //{
            //    var mouseOverBounds = new Rect(bounds);
            //    ////mouseOverBounds.x -= mouseOverBounds.width*0.2f;
            //    //mouseOverBounds.y += mouseOverBounds.height * 0.125f;
            //    //mouseOverBounds.x += mouseOverBounds.width * 0.125f;
            //    mouseOverBounds.width = 20;
            //    mouseOverBounds.height = 20;
            //    bounds = mouseOverBounds;
            //}
        
            //if (ViewModelObject.IsMouseOver)
            //{
            //    EditorGUI.DrawRect(Bounds.Scale(scale), Color.black);
            //}
            //if (!ViewModel.HasConnections)
                //if (!ViewModel.ConnectorFor.IsMouseOver && !ViewModel.ConnectorFor.IsSelected && !ViewModel.IsMouseOver) return;
            if (!ViewModel.AlwaysVisible)
            {
                if (!ViewModel.ConnectorFor.IsMouseOver && !ViewModel.ConnectorFor.IsSelected && !ViewModel.IsMouseOver && !ViewModel.HasConnections) return;
            }
          
            //if (ViewModel.HasConnections)
            //{
            //    platform.DrawImage(bounds, Texture, true);


         
            //}
            if (ViewModel.Direction == ConnectorDirection.Output && ViewModel.Side == ConnectorSide.Right)
            {
                Bounds = new Rect(pos.x, pos.y,TextureWidth, TextureHeight);

                //return;
            }
            else
            {
                Bounds = new Rect(pos.x, pos.y, TextureWidth, TextureHeight);

            }

            var padding = ViewModel.StyleSchema.Padding;
            Bounds = Bounds.Pad(padding.x, padding.y, padding.width, padding.height);

            var bounds = Bounds.Scale(scale);
          // if (ViewModel.IsMouseOver)
         //   {
                platform.DrawImage(bounds, Texture, true);
              //  platform.DrawImage(bounds, Texture, true);
          //  }
            
            //platform.DrawImage(bounds, Texture, true);


            if (ViewModel.Direction == ConnectorDirection.Output && !string.IsNullOrEmpty(ViewModel.OutputDesctiption))
                platform.SetTooltipForRect(bounds, ViewModel.OutputDesctiption);
            else if ( !string.IsNullOrEmpty(ViewModel.InputDesctiption))
            {
                platform.SetTooltipForRect(bounds, ViewModel.InputDesctiption);
                
            }
            //if (InvertGraphEditor.Settings.ShowGraphDebug && ViewModel.IsMouseOver)
            //{
            //    GUI.Label(new Rect(Bounds.x + 20, Bounds.y - 10, 500, 50),
                    
            //        this.ViewModel.DataObject.GetType().Name,
            //        EditorStyles.miniBoldLabel);
            //}

        }


    }
}
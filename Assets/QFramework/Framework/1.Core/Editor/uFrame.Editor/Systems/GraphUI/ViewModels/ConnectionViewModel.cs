using System;
using Invert.uFrame.Editor.ViewModels;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class ConnectionViewModel : GraphItemViewModel
    {
        private Color _color;
        public ConnectorViewModel ConnectorA { get; set; }
        public ConnectorViewModel ConnectorB { get; set; }
        

        public virtual bool IsStateLink { get; set; }

        public Action<ConnectionViewModel> Apply { get; set; }
        public Action<ConnectionViewModel> Remove { get; set; }

        public ConnectionViewModel(DiagramViewModel diagramViewModel)
        {
            DiagramViewModel = diagramViewModel;
        }

        public override Vector2 Position { get; set; }

        public Color InActiveColor { get; set; }
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                InActiveColor = new Color(value.r,value.g,value.b,0.5f);
            }
        }

        public override string Name
        {
            get { return  ConnectorA.Name + " -> " + ConnectorB.Name; }
            set { }
        }

        public bool IsActive { get; set; }

        public bool IsFullColor
        {
            get
            {
                if (ConnectorA.ConnectorFor.IsSelected || ConnectorB.ConnectorFor.IsSelected)
                    return true;

                if (ConnectorA.ConnectorFor.IsMouseOver || ConnectorB.ConnectorFor.IsMouseOver)
                    return true;
                return false;
            }
        }

        public Color CurrentColor
        {
            get
            {
                if (IsActive)
                    return Color.green;

                if (IsFullColor)
                    return Color;

                return InActiveColor;
            }
        }

    }
}
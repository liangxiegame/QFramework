using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QF.GraphDesigner
{
    public abstract class ConnectorStyleSchema : IConnectorStyleSchema
    {
        private readonly Dictionary<SideDirectionItem, object> TexturesCache = new Dictionary<SideDirectionItem, object>();
        protected string _emptyInputIconCode;
        protected string _emptyOutputIconCode;
        protected string _filledInputIconCode;
        protected string _filledOutputIconCode;
        protected string _emptyTwoWayIconCode;
        protected string _filledTwoWayIconCode;

        public object GetTexture(ConnectorSide side, ConnectorDirection direction, bool connected, Color tint = default(Color))
        {
            var item = new SideDirectionItem()
            {
                Side = side,
                Direction = direction,
                IsConnected = connected,
                Tint = tint
            };


            object value = null;
            TexturesCache.TryGetValue(item, out value);
            if (value == null || value.Equals(null))
            {
                TexturesCache[item] = ConstructTexture(side, direction, connected, tint);
            }

            return TexturesCache[item];

        }

        protected abstract object ConstructTexture(ConnectorSide side, ConnectorDirection direction, bool connected, Color tint = default(Color));

        public IConnectorStyleSchema WithInputIcons(string emptyIcon, string filledIcon)
        {
            _emptyInputIconCode = emptyIcon;
            _filledInputIconCode = filledIcon;
            return this;
        }

        public IConnectorStyleSchema WithOutputIcons(string emptyIcon, string filledIcon)
        {
            _emptyOutputIconCode = emptyIcon;
            _filledOutputIconCode = filledIcon;
            return this;
        }

        public IConnectorStyleSchema WithTwoWayIcons(string emptyIcon, string filledIcon)
        {
            _emptyTwoWayIconCode = emptyIcon;
            _filledTwoWayIconCode = filledIcon;
            return this;
        }

        public IConnectorStyleSchema WithDefaultIcons()
        {
            return WithInputIcons("DiagramArrowRightEmpty", "DiagramArrowRight").
                   WithTwoWayIcons("DiagramArrowRightEmpty", "DiagramArrowRight").
                   WithOutputIcons("DiagramArrowRightEmpty", "DiagramArrowRight");
        }

        public IConnectorStyleSchema WithPad(float left, float top, float right, float bottom)
        {
            Padding = new Rect(left, top, right, bottom);
            return this;
        }

        public Rect Padding { get; set; }

        internal struct SideDirectionItem
        {
            public ConnectorSide Side { get; set; }
            public ConnectorDirection Direction { get; set; }
            public bool IsConnected { get; set; }
            public Color Tint { get; set; }
        }
    }
}

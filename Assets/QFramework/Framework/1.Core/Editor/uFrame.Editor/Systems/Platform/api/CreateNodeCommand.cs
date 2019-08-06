using System;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class CreateNodeCommand : Command
    {
        public Type NodeType { get; set; }
        public MouseEvent LastMouseEvent { get; set; }
        public IGraphData GraphData { get; set; }
        public Vector2 Position { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF;
using UnityEngine;
using Invert.Data;
using QF.Json;

namespace QF.GraphDesigner
{
    public class NoteNode : GenericNode
    {
        private Vector2 _size = new Vector2(225f,72f);
        private NodeColor _markColor;
        private bool _showMark;
        private string _header;
        private string _comments1;


        [JsonProperty, InspectorProperty(InspectorType.TextArea)]
        public override string Comments
        {
            get { return !string.IsNullOrEmpty(_comments1) ? _comments1 : "Open uFrame Inspector Window and select this node.\nChange the text using node comments."; }
            set { _comments1 = value; }
        }

        public override bool AllowInputs
        {
            get { return false; }
        }

        public override bool AllowOutputs
        {
            get { return false; }
        }

        [JsonProperty, InspectorProperty]
        public NodeColor MarkColor
        {
            get { return _markColor; }
            set { this.Changed("MarkColor", ref _markColor, value); }
        }

        [JsonProperty, InspectorProperty]
        public string HeaderText
        {
            get { return _header; }
            set { this.Changed("Header", ref _header, value); }
        }

        [JsonProperty, InspectorProperty]
        public bool ShowMark
        {
            get { return _showMark; }
            set { this.Changed("ShowMark", ref _showMark, value); }
        }

        [JsonProperty, InspectorProperty]
        public Vector2 Size
        {
            get { return _size; }
            set { this.Changed("Size", ref _size, value); }
        }

    }
}

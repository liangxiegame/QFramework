using Invert.Data;
using QF.Json;
using QF;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class ImageNode : GenericNode
    {
        private Vector2 _size = new Vector2(225f, 72f);
        private string _header;
        private string _comments1;
        private string _imageName;


        [JsonProperty, InspectorProperty(InspectorType.TextArea)]
        public override string Comments
        {
            get { return !string.IsNullOrEmpty(_comments1) ? _comments1 : "Change this text to add description to the image"; }
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
        public string HeaderText
        {
            get { return _header; }
            set { this.Changed("Header", ref _header, value); }
        }

        [JsonProperty, InspectorProperty]
        public string ImageName
        {
            get { return _imageName; }
            set { this.Changed("ImageName", ref _imageName, value); }
        }

        [JsonProperty, InspectorProperty]
        public Vector2 Size
        {
            get { return _size; }
            set { this.Changed("Size", ref _size, value); }
        }

    }
}
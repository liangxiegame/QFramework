using UnityEngine;

namespace QF.GraphDesigner
{
    public class TokenViewModel : GraphItemViewModel
    {
        private LineViewModel _lineViewModel;

        public TokenViewModel(LineViewModel lineViewModel, string text, Color color)
        {
            _lineViewModel = lineViewModel;
            Text = text;
            Color = color;
        }

        public TokenViewModel(LineViewModel lineViewModel)
        {
            LineViewModel = lineViewModel;
        }

        public string Text { get; set; }
        public Color Color { get; set; }
        public bool Bold { get; set; }
        public override Vector2 Position { get; set; }
        public override string Name { get; set; }

        public LineViewModel LineViewModel
        {
            get { return _lineViewModel; }
            set { _lineViewModel = value; }
        }

        public Vector2 TextSize { get; set; }
    }
}
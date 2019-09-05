using System.Collections.Generic;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class LineViewModel : GraphItemViewModel
    {
        private readonly SyntaxViewModel _syntaxViewModel;
        private LinkedList<TokenViewModel> _tokens;

        public LineViewModel(SyntaxViewModel syntaxViewModel)
        {
            _syntaxViewModel = syntaxViewModel;
        }

        public LinkedList<TokenViewModel> Tokens
        {
            get { return _tokens ?? (_tokens = new LinkedList<TokenViewModel>()); }
            set { _tokens = value; }
        }

        public SyntaxViewModel SyntaxViewModel
        {
            get { return _syntaxViewModel; }
        }

        public override Vector2 Position { get; set; }
        public override string Name { get; set; }
    }
}
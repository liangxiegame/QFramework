using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class SyntaxViewModel : GraphItemViewModel
    {
        private string _text;
        private LinkedList<LineViewModel> _lines;
        private int _endLine = Int32.MaxValue;

        public SyntaxViewModel(string text, string name, int startLine = 0)
        {
            StartLine = startLine;
            Text = text;
            Name = name;
          
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                if (value != null)
                {
                    Lines.Clear();
                    var lines = value.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.None);
                    for (int index = StartLine; index < Math.Min(EndLine, lines.Length); index++)
                    {
                        var line = lines[index];
                        var lineViewModel = ParseLine(line);
                        Lines.AddLast(lineViewModel);
                    }
                }
                
            }
        }
        public const string CSHARP_TOKENS = @"@?[_\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}][\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]*|\{|\}";
        private LineViewModel ParseLine(string line)
        {
            var lineViewModel = new LineViewModel(this);
            var matchList = Regex.Matches(line, CSHARP_TOKENS + @"|.|\s+",RegexOptions.None);
            foreach (Match match in matchList)
            {
                var token = new TokenViewModel(lineViewModel, match.Value, Color.gray);
                GetColor(token);
                lineViewModel.Tokens.AddLast(token);
            }
            return lineViewModel;
        }

        public string[] Keywords =
        {
            "class",
            "int",
            "public",
            "void",
            "return",
            "virtual",
            "protected",
            "override",
            "var",
            "partial",
            "private",
            "using",
            "if",
            "foreach",
            "for",
            "string",
            "bool",
            "float",
            "decimal",
            "base",
            "get",
            "set"
        };
        public string[] Literals =
        {
            "{",
            "}",
            "(",
            ")",
          
        };

        private bool lastWasKeyword = false;
        private void GetColor(TokenViewModel value)
        {
       
            if (value.Text == "\"")
            {
                value.Color = Color.green;
                return;
            }
            if (Keywords.Contains(value.Text))
            {
                value.Color = Color.blue;
                value.Bold = true;
                lastWasKeyword = true;
                return;
            }
            if (lastWasKeyword && !value.Text.Any(char.IsWhiteSpace))
            {
                value.Color = Color.grey;
                lastWasKeyword = false;
                return;
            }
            if (Literals.Contains(value.Text))
            {
                value.Color = Color.gray;
                value.Bold = true;

            }
            value.Color = Color.black;
        }

        public LinkedList<LineViewModel> Lines
        {
            get { return _lines ?? (_lines = new LinkedList<LineViewModel>()); }
            set { _lines = value; }
        }

        public override Vector2 Position { get; set; }
        public override string Name { get; set; }

        public int StartLine { get; set; }

        public int EndLine
        {
            get { return _endLine; }
            set { _endLine = value; }
        }
    }
}
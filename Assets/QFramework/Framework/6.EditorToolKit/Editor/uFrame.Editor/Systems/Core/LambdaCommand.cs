using System;

namespace QF.GraphDesigner
{
    public class LambdaCommand :ICommand
    {
        public LambdaCommand(string title,System.Action action)
        {
            Title = title;
            Action = action;
        }
        public string Title { get; set; }
        public System.Action Action { get; set; }
    }
}
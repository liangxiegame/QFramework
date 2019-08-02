using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class ContextMenuUI
    {
        private List<ContextMenuItem> _commands;
        public bool Flatten { get; set; }

        public List<ContextMenuItem> Commands
        {
            get { return _commands ?? (_commands = new List<ContextMenuItem>()); }
            set { _commands = value; }
        }

        public ContextMenuUI()
        {
            Commands = new List<ContextMenuItem>();
        }

        public void AddCommand(ContextMenuItem command)
        {
            Commands.Add(command);
        }

     
        public virtual void Go()
        {
          
        }

 
        public void GoBottom()
        {
                
        }

        public virtual void AddSeparator()
        {
            
        }
    }
}
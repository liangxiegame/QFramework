using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class ToolbarUI
    {
        public ToolbarUI()
        {
            LeftCommands = new List<ToolbarItem>();
            RightCommands = new List<ToolbarItem>();
            BottomLeftCommands = new List<ToolbarItem>();
            BottomRightCommands = new List<ToolbarItem>();
            AllCommands = new List<ToolbarItem>();
        }

        public List<ToolbarItem> AllCommands { get; set; }
        public List<ToolbarItem> LeftCommands { get; set; }
        public List<ToolbarItem> RightCommands { get; set; }
        public List<ToolbarItem> BottomLeftCommands { get; set; }
        public List<ToolbarItem> BottomRightCommands { get; set; }
       

        public void AddCommand(ToolbarItem command)
        {
            
            var cmd = command;
            if (cmd == null || cmd.Position == ToolbarPosition.Right)
            {
                RightCommands.Add(command);
            }
            else if (cmd.Position == ToolbarPosition.BottomLeft)
            {
                BottomLeftCommands.Add(command);
            }else if (cmd.Position == ToolbarPosition.BottomRight)
            {
                BottomRightCommands.Add(command);
            }
            else
            {
                LeftCommands.Add(command);
            }
        }

 

        public virtual void Go()
        {
    

           
        }

        public virtual void GoBottom()
        {
          
            
        }
        

  
    }
}
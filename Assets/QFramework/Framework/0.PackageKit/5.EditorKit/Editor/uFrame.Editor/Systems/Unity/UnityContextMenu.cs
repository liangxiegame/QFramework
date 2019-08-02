using System.Linq;
using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public class UnityContextMenu : ContextMenuUI
    {
        public override void AddSeparator()
        {
            base.AddSeparator();
            Commands.Add(new ContextMenuItem() {Title = string.Empty});
        }

        public void CreateMenuItems(GenericMenu genericMenu)
        {
            var groups = Commands.GroupBy(p => p==null? "" : p.Group).OrderBy(p => p.Key).ToArray();

            foreach (var group in groups)
            {

                //genericMenu.AddDisabledItem(new GUIContent(group.Key));
                var groupCount = 0;
                foreach (var editorCommand in group.OrderBy(p => p.Order))
                {
                    ICommand command = editorCommand.Command;
                    genericMenu.AddItem(new GUIContent(editorCommand.Title),editorCommand.Checked, () =>
                    {
                        InvertApplication.Execute(command);
                    } );
                    groupCount ++;
                }
                if (group != groups.Last() && groupCount > 0)
                    genericMenu.AddSeparator(null);
            }
        }

        public override void Go()
        {
            base.Go();
            var genericMenu = new GenericMenu();
            CreateMenuItems(genericMenu);
            genericMenu.ShowAsContext();
        }
    }
}
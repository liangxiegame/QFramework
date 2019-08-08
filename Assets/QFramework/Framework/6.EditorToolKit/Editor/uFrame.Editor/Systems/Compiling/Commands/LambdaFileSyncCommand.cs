using System;
using System.Reflection;
using System.Text;
using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public class LambdaFileSyncCommand : LambdaCommand, IFileSyncCommand
    {
        public LambdaFileSyncCommand(string title, System.Action action) : base(title, action)
        {
        }
    }
}

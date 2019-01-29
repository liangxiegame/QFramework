using System;
using System.Reflection;
using System.Text;
using QFramework.GraphDesigner;

namespace QFramework.GraphDesigner
{
    public class LambdaFileSyncCommand : LambdaCommand, IFileSyncCommand
    {
        public LambdaFileSyncCommand(string title, Action action) : base(title, action)
        {
        }
    }
}

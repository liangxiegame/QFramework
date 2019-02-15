using System.Collections.Generic;
using System.Linq;
using System.Text;
using QFramework.GraphDesigner;
using QFramework;

namespace QFramework.GraphDesigner
{
    public class CoreSystem : CorePlugin, IExecuteCommand<LambdaCommand>
    {
        public override bool Enabled { get { return true; } set{}}
        public override void Loaded(QFrameworkContainer container)
        {
            
        }

        public void Execute(LambdaCommand command)
        {
            command.Action();
        }
    }
}

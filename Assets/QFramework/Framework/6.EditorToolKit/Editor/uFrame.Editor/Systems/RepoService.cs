using Invert.Data;
using QF;

namespace QF.GraphDesigner
{
    public class RepoService : DiagramPlugin
    {
        
        public IRepository Repository
        {
            get { return Container.Resolve<IRepository>(); }
        }
    }
}
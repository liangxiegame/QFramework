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

        public override void Initialize(QFrameworkContainer container)
        {
            base.Initialize(container);

        }

        public override void Loaded(QFrameworkContainer container)
        {
            base.Loaded(container);
        }
    }
}
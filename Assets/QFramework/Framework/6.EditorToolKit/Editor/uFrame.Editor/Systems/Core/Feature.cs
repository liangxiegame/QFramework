using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public abstract class Feature : CorePlugin
    {
        public override bool EnabledByDefault
        {
            get { return true; }
        }
        
        public override bool Required
        {
            get { return true; }
        }

        public override bool Enabled
        {
            get { return true; }
            set
            {
                
            }
        }
    }
}
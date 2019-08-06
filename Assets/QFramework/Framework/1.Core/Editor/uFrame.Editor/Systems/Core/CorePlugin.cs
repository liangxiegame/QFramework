using System;
using QF;

namespace QF.GraphDesigner
{
    public abstract class CorePlugin : ICorePlugin
    {
        private QFrameworkContainer _container;

        public void Execute<TCommand>(TCommand command) where TCommand :  ICommand
        {
            InvertApplication.Execute(command);
        }
        public virtual string PackageName
        {
            get { return string.Empty; }
        }

        public virtual bool Required
        {
            get { return false; }
        }

        public virtual bool Ignore
        {
            get { return false; }
        }

        public virtual string Title
        {
            get { return this.GetType().Name; }
        }

        public abstract bool Enabled { get; set; }

        public virtual bool EnabledByDefault
        {
            get { return true; }
        }

        public virtual decimal LoadPriority { get { return 1; } }

        public TimeSpan InitializeTime { get; set; }
        public TimeSpan LoadTime { get; set; }
        
        public virtual void Initialize(QFrameworkContainer container)
        {
            Container = container;
        }

        public QFrameworkContainer Container
        {
            get { return InvertApplication.Container; }
            set { _container = value; }
        }

        public abstract void Loaded(QFrameworkContainer container);
    }
}
using System;
using System.Collections.Generic;
using Invert.Data;
using QF;


namespace QF.GraphDesigner
{
    public abstract class DesignerGeneratorFactory
    {
        public abstract Type DiagramItemType
        {
            get;
        }

        [Inject]
        public IQFrameworkContainer Container { get; set; }


        public object ObjectData { get; set; }

        public abstract IEnumerable<OutputGenerator> GetGenerators(IGraphConfiguration graphConfig, object node);
    }

    public abstract class DesignerGeneratorFactory<TData> : DesignerGeneratorFactory where TData : class
    {
        public override Type DiagramItemType
        {
            get { return typeof(TData); }
        }

        public sealed override IEnumerable<OutputGenerator> GetGenerators(IGraphConfiguration graphConfig, object node)
        {
            return CreateGenerators(graphConfig, node as TData);
        }
        public abstract IEnumerable<OutputGenerator> CreateGenerators(IGraphConfiguration graphConfig,  TData item);

    }



}
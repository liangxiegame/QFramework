using System;
using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class NodeGeneratorConfigBase
    {

     //   private List<NodeChildGeneratorConfigBase> _childItemMemberGenerators = new List<NodeChildGeneratorConfigBase>();
     ////   private List<IMemberGenerator> _memberGenerators = new List<IMemberGenerator>();

     //   public List<NodeChildGeneratorConfigBase> ChildItemMemberGenerators
     //   {
     //       get { return _childItemMemberGenerators; }
     //   }

        public Type GeneratorType { get; set; }

        //public List<IMemberGenerator> MemberGenerators
        //{
        //    get { return _memberGenerators; }
        //}

        //public NodeGeneratorConfig<TType> AddMemberGenerator<TMemberGenerator>()
        //    where TMemberGenerator : IMemberGenerator<TType>
        //{
        //    MemberGenerators.Add(typeof(TMemberGenerator));
        //    return this;
        //}
    }
}
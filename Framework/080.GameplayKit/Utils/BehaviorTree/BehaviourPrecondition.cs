using System;

namespace QFramework
{
    //---------------------------------------------------------------
    public abstract class BehaviourPrecondition : BehaviourTreeNode
    {
        public BehaviourPrecondition(int maxChildCount)
            : base(maxChildCount)
        {}
        public abstract bool IsTrue( /*in*/ BehaviourTreeData wData);
    }
    public abstract class BehaviourPreconditionLeaf : BehaviourPrecondition
    {
        public BehaviourPreconditionLeaf()
            : base(0)
        {}
    }
    public abstract class BehaviourPreconditionUnary : BehaviourPrecondition
    {
        public BehaviourPreconditionUnary(BehaviourPrecondition lhs)
            : base(1)
        {
            AddChild(lhs);
        }
    }
    public abstract class BehaviourPreconditionBinary : BehaviourPrecondition
    {
        public BehaviourPreconditionBinary(BehaviourPrecondition lhs, BehaviourPrecondition rhs)
            : base(2)
        {
            AddChild(lhs).AddChild(rhs);
        }
    }
    //--------------------------------------------------------------
    //basic precondition
    public class BehaviourPreconditionTrue : BehaviourPreconditionLeaf
    {
        public override bool IsTrue( /*in*/ BehaviourTreeData wData)
        {
            return true;
        }
    }
    public class BehaviourPreconditionFalse : BehaviourPreconditionLeaf
    {
        public override bool IsTrue( /*in*/ BehaviourTreeData wData)
        {
            return false;
        }
    }
    //---------------------------------------------------------------
    //unary precondition
    public class BehaviourPreconditionNot : BehaviourPreconditionUnary
    {
        public BehaviourPreconditionNot(BehaviourPrecondition lhs)
            : base(lhs)
        {}
        public override bool IsTrue( /*in*/ BehaviourTreeData wData)
        {
            return !GetChild<BehaviourPrecondition>(0).IsTrue(wData);
        }
    }
    //---------------------------------------------------------------
    //binary precondition
    public class BehaviourPreconditionAnd : BehaviourPreconditionBinary
    {
        public BehaviourPreconditionAnd(BehaviourPrecondition lhs, BehaviourPrecondition rhs)
            : base(lhs, rhs)
        { }
        public override bool IsTrue( /*in*/ BehaviourTreeData wData)
        {
            return GetChild<BehaviourPrecondition>(0).IsTrue(wData) &&
                   GetChild<BehaviourPrecondition>(1).IsTrue(wData);
        }
    }
    public class BehaviourPreconditionOr : BehaviourPreconditionBinary
    {
        public BehaviourPreconditionOr(BehaviourPrecondition lhs, BehaviourPrecondition rhs)
            : base(lhs, rhs)
        { }
        public override bool IsTrue( /*in*/ BehaviourTreeData wData)
        {
            return GetChild<BehaviourPrecondition>(0).IsTrue(wData) ||
                   GetChild<BehaviourPrecondition>(1).IsTrue(wData);
        }
    }
    public class BehaviourPreconditionXor : BehaviourPreconditionBinary
    {
        public BehaviourPreconditionXor(BehaviourPrecondition lhs, BehaviourPrecondition rhs)
            : base(lhs, rhs)
        { }
        public override bool IsTrue( /*in*/ BehaviourTreeData wData)
        {
            return GetChild<BehaviourPrecondition>(0).IsTrue(wData) ^
                   GetChild<BehaviourPrecondition>(1).IsTrue(wData);
        }
    }
}


using System.Collections.Generic;

namespace QFramework
{
    public class BehaviourTreeNode
    {
        //-------------------------------------------------------------------
        private const int defaultChildCount = -1; //TJQ： unlimited count
        //-------------------------------------------------------------------
        private List<BehaviourTreeNode> _children;
        
        private int _maxChildCount;
        //private TBTTreeNode _parent;
        //-------------------------------------------------------------------
        public BehaviourTreeNode(int maxChildCount = -1)
        {
            _children = new List<BehaviourTreeNode>();
            if (maxChildCount >= 0) {
                _children.Capacity = maxChildCount;
            }
            _maxChildCount = maxChildCount;
        }
        public BehaviourTreeNode()
            : this(defaultChildCount)
        {}
        ~BehaviourTreeNode()
        {
            _children = null;
        }
        //-------------------------------------------------------------------
        public BehaviourTreeNode AddChild(BehaviourTreeNode node)
        {
            if (_maxChildCount >= 0 && _children.Count >= _maxChildCount) {
                Log.W("**BT** exceeding child count");
                return this;
            }
            _children.Add(node);
            return this;
        }
        
        public int GetChildCount()
        {
            return _children.Count;
        }
        public bool IsIndexValid(int index)
        {
            return index >= 0 && index < _children.Count;
        }
        public T GetChild<T>(int index) where T : BehaviourTreeNode 
        {
            if (index < 0 || index >= _children.Count) {
                return null;
            }
            return (T)_children[index];
        }
    }
}

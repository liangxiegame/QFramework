using UnityEditorUI;
using UnityEngine;

namespace QF.Editor
{
    public interface IToolbar : IWidget
    {
        IPropertyBinding<int, IToolbar> Index { get; }
    }
    /// <summary>
    /// 要改进，最好可以优化掉这个
    /// </summary>

    public class Toolbar : AbstractWidget,  IToolbar
    {
        private int mIndex;
        private string[] mNames;
        
        private PropertyBinding<int,IToolbar> mIndexProperty;
        
        public Toolbar(ILayout parent,int defaultIndex, string[] names) : base(parent)
        {
            mIndexProperty = new PropertyBinding<int, IToolbar>(this, newValue =>
            {
                mIndex = newValue;
            });

            mNames = names;
            mIndex = defaultIndex;    
        }

        public override void OnGUI()
        {
            var index = GUILayout.Toolbar(mIndex, mNames);

            if (index != mIndex)
            {
                mIndex = index;
                mIndexProperty.UpdateView(mIndex);
            }
        }

        public override void BindViewModel(object viewModel)
        {
            mIndexProperty.BindViewModel(viewModel);
        }

        public IPropertyBinding<int, IToolbar> Index
        {
            get { return mIndexProperty; }
        }
    }
}
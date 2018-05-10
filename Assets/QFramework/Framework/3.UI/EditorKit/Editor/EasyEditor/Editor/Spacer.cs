
using UnityEngine;

namespace UnityEditorUI
{
    /// <summary>
    /// Inserts a space between other widgets.
    /// </summary>
    public interface ISpacer : IWidget
    {
        IPropertyBinding<float, ISpacer> Pixels { get; }
    }

    /// <summary>
    /// Inserts a space between other widgets.
    /// </summary>
    internal class Spacer : AbstractWidget, ISpacer
    {
        private readonly PropertyBinding<float, ISpacer> mPixelsProperty;

        private float mPixels;

        internal Spacer(ILayout parent) : base(parent)
        {
            mPixelsProperty = new PropertyBinding<float, ISpacer>(
                this,
                value => mPixels = value
            );
        }

        public override void OnGUI()
        {
            GUILayout.Space(mPixels);
        }

        public override void BindViewModel(object viewModel)
        {

        }

        public IPropertyBinding<float, ISpacer> Pixels { get { return mPixelsProperty; } }
    }
}

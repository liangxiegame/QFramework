using UnityEngine;

namespace QFramework.CodeGen
{
    public class InspectorViewModel : GraphItemViewModel
    {
        private GraphItemViewModel _targetViewModel;

        public GraphItemViewModel TargetViewModel
        {
            get { return _targetViewModel; }
            set
            {
                _targetViewModel = value;
                if (_targetViewModel != null)
                {
                    DataObject = value.DataObject;
                }
                else
                {
                    DataObject = null;
                }
            }
        }

        public virtual Vector2 Position { get; set; }
        public override string Name { get; set; }
    }
}
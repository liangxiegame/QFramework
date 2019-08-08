using System;
using System.Reflection;

namespace QF.GraphDesigner
{
    public class PropertyFieldViewModel : ItemViewModel
    {
        public Type CustomDrawerType { get; set; }
        public InspectorType InspectorType { get; set; }
        public Type Type { get; set; }
        public override string Name { get; set; }
        public Func<object> Getter { get; set; }
        public Action<object,object> Setter { get; set; }
        public override bool IsEditing { get; set; }
        public object CachedValue { get; set; }
        public string InspectorTip { get; set; }
        public PropertyInfo PropertyInfo { get; set; }


        public PropertyFieldViewModel()
        {
        }

        public PropertyFieldViewModel(DiagramNodeViewModel nodeViewModel) : base(nodeViewModel)
        {
      
        }
    }
}
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class InspectorViewModel : GraphItemViewModel
    {
        private GraphItemViewModel _targetViewModel;

        public bool Visible
        {
            get { return DataObject != null; }
        }

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

        public override Vector2 Position { get; set; }
        public override string Name { get; set; }

        public override void DataObjectChanged()
        {
            base.DataObjectChanged();
            AddPropertyFields();
            IsDirty = true;

        }
        public void AddPropertyFields(string headerText = null)
        {
            if (DataObject == null) return;
            ContentItems.Clear();
            var ps = DataObject.GetPropertiesWithAttribute<InspectorProperty>().ToArray();
            if (ps.Length < 1) return;

            if (!string.IsNullOrEmpty(headerText))
                ContentItems.Add(new SectionHeaderViewModel()
                {
                    Name = headerText,
                });
            var data = DataObject;
            foreach (var property in ps)
            {
                PropertyInfo property1 = property.Key;
                var vm = new PropertyFieldViewModel()
                {
                    Type = property.Key.PropertyType,
                    Name = property.Key.Name,
                    InspectorType = property.Value.InspectorType,
                    CustomDrawerType = property.Value.CustomDrawerType,
                    Getter = () => property1.GetValue(data, null),
                    DataObject = data,
                    Setter = (d,v) =>
                    {

                        property1.SetValue(d, v, null);

                    }
                };
                ContentItems.Add(vm);
            }
            IsDirty = true;
        }
    }
}
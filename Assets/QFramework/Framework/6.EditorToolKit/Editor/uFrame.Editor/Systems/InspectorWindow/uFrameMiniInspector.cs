using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Invert.Common;
using QF.GraphDesigner;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace QF.GraphDesigner.Unity.InspectorWindow
{
    public class uFrameMiniInspector
    {
        private List<PropertyFieldViewModel> _properties;
        private UnityDrawer _drawer;

        public uFrameMiniInspector(object target)
        {
            DiagramViewModel = InvertApplication.Container.Resolve<DesignerWindow>().DiagramViewModel;
            
            foreach (var item in target.GetPropertiesWithAttribute<InspectorProperty>())
            {
                var property = item.Key;
                var attribute = item.Value;
                var fieldViewModel = new PropertyFieldViewModel()
                {
                    Name = property.Name,
                };
                fieldViewModel.Getter = () => property.GetValue(target, null);
                fieldViewModel.Setter = (d,v) => property.SetValue(d, v, null);
                fieldViewModel.InspectorType = attribute.InspectorType;
                fieldViewModel.Type = property.PropertyType;
                fieldViewModel.DiagramViewModel = DiagramViewModel;
                fieldViewModel.DataObject = target;
                fieldViewModel.CustomDrawerType = attribute.CustomDrawerType;
                fieldViewModel.CachedValue = fieldViewModel.Getter();
                if (!string.IsNullOrEmpty(attribute.InspectorTip)) fieldViewModel.InspectorTip = attribute.InspectorTip;
                Properties.Add(fieldViewModel);

                Height += fieldViewModel.InspectorType == InspectorType.GraphItems ? 30 : 17;

            }
        }

        public UnityDrawer Drawer
        {
            get { return _drawer ?? (InvertApplication.Container.Resolve<IPlatformDrawer>() as UnityDrawer); }
            set { _drawer = value; }
        }

        public DiagramViewModel DiagramViewModel { get; set; }

        public List<PropertyFieldViewModel> Properties
        {
            get { return _properties ?? (_properties = new List<PropertyFieldViewModel>()); }
            set { _properties = value; }
        }

        public float Height { get; set; }

        public void Draw(Rect rect)
        {
            var itemRect = rect.WithHeight(17);

            foreach (var prop in Properties)
            {
                if (prop.InspectorType == InspectorType.GraphItems) itemRect = itemRect.WithHeight(30);
                Drawer.DrawInspector(itemRect,prop,ElementDesignerStyles.DarkInspectorLabel);
                itemRect = itemRect.Below(itemRect).WithHeight(17);
            }            
        }

    }
}

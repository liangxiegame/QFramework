using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using QFramework.CodeGen;

namespace QFramework.CodeGen
{
    public class GraphDesignerNavigationViewModel
    {
        private List<NavigationItem> _tabs;
        private List<NavigationItem> _breadcrubs;

        
        public List<NavigationItem> Tabs
        {
            get { return _tabs ?? (_tabs = new List<NavigationItem>()); }
            set { _tabs = value; }
        }

        public List<NavigationItem> Breadcrubs
        {
            get { return _breadcrubs ?? (_breadcrubs = new List<NavigationItem>()); }
            set { _breadcrubs = value; }
        }



        public void Refresh()
        {
            Tabs.Clear();

            Breadcrubs.Clear();

            foreach (var filter in new[] { DiagramViewModel.GraphData.RootFilter }.Concat(this.DiagramViewModel.GraphData.GetFilterPath()))
            {
                var navigationItem = new NavigationItem()
                {
                    Icon = "CommandIcon",
                    Title = filter.Name,
                    State = DiagramViewModel.GraphData != null && DiagramViewModel.GraphData.CurrentFilter == filter ? NavigationItemState.Current : NavigationItemState.Regular,
                    NavigationAction = x =>
                    {
                    }       
                };

                if (filter == DiagramViewModel.GraphData.RootFilter) navigationItem.SpecializedIcon = "RootFilterIcon";

                Breadcrubs.Add(navigationItem);
            }
        }

        public DiagramViewModel DiagramViewModel { get; set; }

    }
}
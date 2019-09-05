using System;
using System.Collections.Generic;

namespace QF.GraphDesigner
{
    public class WorkspaceConfiguration
    {
        private List<WorkspaceGraphConfiguration> _graphTypes;
        public string Title { get; set; }
        public string Description { get; set; }
        public Type WorkspaceType { get; set; }

        public List<WorkspaceGraphConfiguration> GraphTypes
        {
            get { return _graphTypes ?? (_graphTypes = new List<WorkspaceGraphConfiguration>()); }
            set { _graphTypes = value; }
        }

        public WorkspaceConfiguration(Type workspaceType, string title)
            : this(workspaceType, title, null)
        {
        }

        public WorkspaceConfiguration(Type workspaceType, string title, string description)
        {
            WorkspaceType = workspaceType;
            Title = title;
            Description = description;
        }

        public WorkspaceConfiguration(List<WorkspaceGraphConfiguration> graphTypes, string title, string description, Type workspaceType)
        {
            _graphTypes = graphTypes;
            Title = title;
            Description = description;
            WorkspaceType = workspaceType;
        }

        public WorkspaceConfiguration WithGraph<TGraphType>(string title, string description = null)
        {
            GraphTypes.Add(new WorkspaceGraphConfiguration()
            {
                Title = title,
                Description = description,
                GraphType = typeof(TGraphType)
            });
            return this;
        }
    }
}
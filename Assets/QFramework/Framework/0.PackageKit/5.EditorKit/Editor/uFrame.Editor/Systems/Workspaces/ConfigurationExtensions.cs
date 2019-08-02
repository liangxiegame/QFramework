using QF;

namespace QF.GraphDesigner
{
    public static class ConfigurationExtensions
    {
        public static WorkspaceConfiguration AddWorkspaceConfig<TWorkspaceType>(this IQFrameworkContainer container, string title, string description = null)
        {
            var config = new WorkspaceConfiguration(typeof(TWorkspaceType), title, description);
            container.RegisterInstance(config, typeof(TWorkspaceType).Name);
            return config;
        }

        public static WorkspaceConfiguration WorkspaceConfig<TWorkspaceType>(this IQFrameworkContainer container)
        {
            return container.Resolve<WorkspaceConfiguration>(typeof(TWorkspaceType).Name) ?? container.AddWorkspaceConfig<TWorkspaceType>(typeof(TWorkspaceType).Name);
        }
    }
}
namespace QFramework
{
    public class EasyIMGUI : Architecture<EasyIMGUI>
    {
        public static IButton Button()
        {
            return new ButtonView();
        }
        
        private EasyIMGUI()
        {
        }

        protected override void OnSystemConfig(IQFrameworkContainer systemLayer)
        {
        }

        protected override void OnModelConfig(IQFrameworkContainer modelLayer)
        {
        }

        protected override void OnUtilityConfig(IQFrameworkContainer utilityLayer)
        {
        }

        protected override void OnLaunch()
        {
        }
    }
}
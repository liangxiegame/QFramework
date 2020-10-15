namespace QFramework
{
    public class EasyIMGUI : Architecture<EasyIMGUI>
    {
        public static IButton Button()
        {
            return new Button();
        }

        public static ILabel Label()
        {
            return new Label();
        }

        public static ISpace Space()
        {
            return new Space();
        }

        public static IFlexibleSpace FlexibleSpace()
        {
            return new FlexibleSpace();
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
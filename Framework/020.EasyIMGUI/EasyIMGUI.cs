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

        public static ITextField TextField()
        {
            return new TextField();
        }

        public static ITextArea TextArea()
        {
            return new TextArea();
        }

        public static ICustom Custom()
        {
            return new CustomView();
        }

        public static IToggle Toggle()
        {
            return new Toggle();
        }

        public static IToolbar Toolbar()
        {
            return new ToolbarView();
        }

        public static IVerticalLayout Vertical()
        {
            return new VerticalLayout();
        }
        
        public static IHorizontalLayout Horizontal()
        {
            return new HorizontalLayout();
        }

        public static IScrollLayout Scroll()
        {
            return new ScrollLayout();
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
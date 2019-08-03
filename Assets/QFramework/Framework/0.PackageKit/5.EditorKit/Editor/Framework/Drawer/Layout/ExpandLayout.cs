namespace EGO.Framework
{
    public class ExpandLayout : Layout
    {
        public ExpandLayout(string label)
        {
            Label = label;
        }

        public string Label { get; set; }



        protected override void OnGUIBegin()
        {

        }


        protected override void OnGUI()
        {
//            if (GUIHelpers.DoToolbarEx(Label))
//            {
//                foreach (var child in Children)
//                {
//                    child.DrawGUI();
//                }
//            }
        }

        protected override void OnGUIEnd()
        {
            
        }
    }
}
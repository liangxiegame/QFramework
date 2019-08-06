namespace QF.GraphDesigner
{ 
    public class ModifierKeyState
    {
        public bool Alt { get; set; }

        public bool Ctrl { get; set; }

        public bool Shift { get; set; }

        public bool Any
        {
            get
            {
                return Ctrl  || Alt  || Shift; 
            }
        }
    }
}
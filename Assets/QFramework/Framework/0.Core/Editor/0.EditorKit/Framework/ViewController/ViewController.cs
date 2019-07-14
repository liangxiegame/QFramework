namespace EGO.Framework
{
    public abstract class ViewController
    {
        public VerticalLayout View = new VerticalLayout();
        
        public abstract void SetUpView();        
    }
}
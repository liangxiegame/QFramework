namespace Unidux.SceneTransition
{
    public static class SceneCategory
    {
        // Permanent is never removed
        public const int Permanent = 1;
        
        // Page represents exclusive scene transition
        public const int Page = 2;
        
        // Modal is temporary overlay
        public const int Modal = 3;
    }
}
namespace UnityEditorUI
{
    /// <summary>
    /// Root layout - same as VerticalLayout except that it does not contain a parent.
    /// </summary>
    internal class RootLayout : AbstractLayout
    {
        internal RootLayout() : 
            base(null) 
        {
        }
    }
}

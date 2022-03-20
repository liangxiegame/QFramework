namespace QFramework
{
    internal class PackageMakerModel
    {
        public static void InitState()
        {
            InEditorView.Value = true;
            InFinishView.Value = false;
            InUploadingView.Value = false;
            NoticeMessage.Value = "";
            UpdateResult.Value = "";
        }
        
        public static BindableProperty<bool> InEditorView = new BindableProperty<bool>(true);
        public static BindableProperty<bool> InFinishView = new BindableProperty<bool>(true);
        public static BindableProperty<bool> InUploadingView = new BindableProperty<bool>(true);
        public static BindableProperty<string> NoticeMessage = new BindableProperty<string>("");
        public static BindableProperty<string> UpdateResult = new BindableProperty<string>("");
    }
}
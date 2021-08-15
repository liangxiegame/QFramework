namespace QFramework
{
    public class PackageMakerModel
    {
        public static void InitState()
        {
            InEditorView.Value = true;
            InFinishView.Value = false;
            InUploadingView.Value = false;
            NoticeMessage.Value = "";
            UpdateResult.Value = "";
        }
        
        public static Property<bool> InEditorView = new Property<bool>(true);
        public static Property<bool> InFinishView = new Property<bool>(true);
        public static Property<bool> InUploadingView = new Property<bool>(true);
        public static Property<string> NoticeMessage = new Property<string>("");
        public static Property<string> UpdateResult = new Property<string>("");
    }
}
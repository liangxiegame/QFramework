using UnityEngine;

namespace QF.GraphDesigner
{
    public class ScreenshotNodeDrawer : DiagramNodeDrawer<ScreenshotNodeViewModel>
    {
        public ScreenshotNodeDrawer(ScreenshotNodeViewModel viewModel) : base(viewModel)
        {
        }

        public override void Refresh(IPlatformDrawer platform)
        {
            //base.Refresh(platform);
            this.Bounds = new Rect(ViewModel.Position.x, ViewModel.Position.y, NodeViewModel.Width, NodeViewModel.Height);
        }

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            //base.Refresh(platform, position, hardRefresh);
            
        }

        public override void RefreshContent()
        {
            //base.RefreshContent();
        }

        public override void OnMouseDown(MouseEvent mouseEvent)
        {
            base.OnMouseDown(mouseEvent);
            ViewModel.SaveImage = true;
        }

        public override void Draw(IPlatformDrawer platform, float scale)
        {
            //base.Draw(platform, scale);
            platform.DrawStretchBox(this.Bounds,CachedStyles.BoxHighlighter1,50f);
            
            if (ViewModel.SaveImage)
            {
                
                // Texture2D texture2D = new Texture2D(NodeViewModel.GraphItem.Width,NodeViewModel.GraphItem.Height, (TextureFormat)3, false);
                // var bounds = new Rect(this.Bounds);
                // bounds.y += editorWindow.position.height;
                //// bounds.y = lastRect.height - bounds.y - bounds.height;
                // texture2D.ReadPixels(bounds, 0, 0);
                // texture2D.Apply();
                // //string fullPath = Path.GetFullPath(Application.get_dataPath() + "/../" + Path.Combine(this.screenshotsSavePath, actionName) + ".png");
                // //if (!FsmEditorUtility.CreateFilePath(fullPath))
                // //    return;
                // byte[] bytes = texture2D.EncodeToPNG();
                // Object.DestroyImmediate((Object)texture2D, true);
                // File.WriteAllBytes("image.png", bytes);
                // ViewModel.SaveImage = false;
                // Debug.Log(this.Bounds.x.ToString() + " : " + this.Bounds.y);
            }

        }
    }
}
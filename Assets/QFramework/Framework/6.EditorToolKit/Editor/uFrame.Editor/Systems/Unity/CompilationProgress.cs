using Invert.Data;
using QF;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public class CompilationProgress : DiagramPlugin, IDesignerWindowEvents, ITaskProgressEvent, ICompileEvents
    {
        public override bool Required
        {
            get { return true; }
        }

        public override void Initialize(QFrameworkContainer container)
        {
            Percentage = 0f;
        }

        public void ProcessInput()
        {

        }

        public void BeforeDrawGraph(Rect diagramRect)
        {

        }

        public void AfterDrawDesignerWindow(Rect windowRect)
        {

        }

        public void AfterDrawGraph(Rect diagramRect)
        {
            if (Percentage >= 0.99f)
            {
                Percentage = 0f;
            }
            if (Percentage > 0.0f && Percentage < 1.0f)
            {
                var drawer = InvertGraphEditor.PlatformDrawer;
                var width = 400f;
                var height = 75f;
                var boxRect = new Rect();
                if (Modal)
                {
                    boxRect.x = (diagramRect.width / 2f) - (width / 2f);
                    boxRect.y = (diagramRect.height / 2f) - (height / 2f);
                    boxRect.width = width;
                    boxRect.height = height;
                }
                else
                {
                    height = 40f;
                    width = 200f;
                    boxRect.x = (diagramRect.width / 2f) - (width / 2f);
                    boxRect.y = diagramRect.height - height - 10f;
                    boxRect.width = width;
                    boxRect.height = height;
                }


                var progressRect = new Rect(boxRect);
                progressRect.y += (boxRect.height - 35f);

                progressRect.height = 7f;
                progressRect.width = boxRect.width * 0.8f;
                progressRect.x = (diagramRect.width / 2f) - (progressRect.width / 2f);

                var progressFill = new Rect(progressRect);
                progressFill.width = (progressRect.width / 100f) * (Percentage * 100f);
                progressFill.x += 1;
                progressFill.y += 1;
                progressFill.height -= 2f;
                if (Modal)
                {
                    drawer.DrawRect(diagramRect, new Color(0.1f, 0.1f, 0.1f, 0.8f));
                    drawer.DoButton(new Rect(0f, 0f, Screen.width, Screen.height), " ", CachedStyles.ClearItemStyle,
                        () => { });
                    drawer.DrawStretchBox(boxRect, CachedStyles.NodeBackground, 12f);
                    boxRect.x += 15f;
                    boxRect.y += 15f;
                    boxRect.width -= 30f;
                }

             

                drawer.DrawRect(progressRect, Color.black);
                drawer.DrawRect(progressFill, Color.blue);

                drawer.DrawLabel(boxRect, string.Format("{0}", Message), Modal ? CachedStyles.ViewModelHeaderStyle : CachedStyles.HeaderTitleStyle, DrawingAlignment.MiddleCenter);
            }


        }

        public void DrawComplete()
        {

        }
        public string Message { get; set; }
        public float Percentage { get; set; }

        public void Progress(float progress, string message, bool modal)
        {
            Message = message;
            Percentage = progress / 100f;
            Modal = modal;
        }

        public bool Modal { get; set; }


        public void PreCompile(IGraphConfiguration configuration, IDataRecord[] compilingRecords)
        {

        }

        public void PostCompile(IGraphConfiguration configuration, IDataRecord[] compilingRecords)
        {
            Percentage = 0f;
        }

        public void FileGenerated(CodeFileGenerator generator)
        {

        }

        public void FileSkipped(CodeFileGenerator codeFileGenerator)
        {

        }
    }
}
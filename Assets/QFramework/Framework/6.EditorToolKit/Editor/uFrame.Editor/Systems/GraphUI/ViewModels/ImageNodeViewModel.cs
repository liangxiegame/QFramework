using UnityEngine;

namespace QF.GraphDesigner
{
    public class ImageNodeViewModel : DiagramNodeViewModel<ImageNode>
    {
        public ImageNodeViewModel(ImageNode graphItemObject, DiagramViewModel diagramViewModel) : base(graphItemObject, diagramViewModel)
        {
        }

        public Vector2 Size
        {
            get { return GraphItem.Size; }
        }

        public string HeaderText
        {
            get { return GraphItem.HeaderText; }
        }
        public string ImageName
        {
            get { return GraphItem.ImageName; }
        }

        public override string Comments
        {
            get { return GraphItem.Comments; }
        }

        protected override void CreateContent()
        {
            ContentItems.Clear();
        }

        public override INodeStyleSchema StyleSchema
        {
            get { return CachedStyles.NodeStyleSchemaMinimalistic; }
        }

        public void OpenImage()
        {
            
        }
    }
}
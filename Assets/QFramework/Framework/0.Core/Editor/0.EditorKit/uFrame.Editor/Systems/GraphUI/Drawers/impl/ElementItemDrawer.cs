//using Invert.Common;
//using UnityEngine;

//namespace Invert.Core.GraphDesigner
//{
//    public class ElementItemDrawer : ItemDrawer
//    {
//        public TypedItemViewModel ElementItemViewModel
//        {
//            get
//            {
//                return ViewModelObject as TypedItemViewModel;
//            }
//        }
//        public ElementItemDrawer(TypedItemViewModel viewModel)
//        {
//            ViewModelObject = viewModel;
//        }

//        public override void Refresh(IPlatformDrawer platform, Vector2 position)
//        {
//            base.Refresh(platform, position);
//            var nameSize = TextStyle.CalcSize(new GUIContent(ElementItemViewModel.Name));
//            var typeSize = TextStyle.CalcSize(new GUIContent(ElementItemViewModel.TypeLabel));

//            Bounds = new Rect(position.x, position.y, 5 + nameSize.x + 5 + typeSize.x + 10, 18);
//        }

//        public override void DrawOption()
//        {
//            base.DrawOption();

//            if (GUILayout.Button(ElementItemViewModel.TypeLabel + (ElementItemViewModel.IsMouseOver ? "..." : string.Empty),ElementDesignerStyles.ClearItemStyle))
//            {
//                ElementItemViewModel.NodeViewModel.IsSelected = true;
//                OptionClicked();
//            }
//        }

//        public virtual void OptionClicked()
//        {
//            var commandName = ViewModelObject.DataObject.GetType().Name.Replace("Data","") + "TypeSelection";

//            var command = InvertGraphEditor.Container.Resolve<IEditorCommand>(commandName);
//            ElementItemViewModel.Select();

//            InvertGraphEditor.ExecuteCommand(command);
//        }

//        public override void Draw(IPlatformDrawer platform, float scale)
//        {

//            base.Draw(platform, scale);
       
//        }
//    }
//}
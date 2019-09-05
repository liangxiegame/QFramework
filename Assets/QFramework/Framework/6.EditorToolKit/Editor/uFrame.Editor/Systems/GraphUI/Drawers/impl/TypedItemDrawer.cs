using Invert.Common;
using QF.GraphDesigner;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class TypedItemDrawer : ItemDrawer
    {
        private Vector2 _nameSize;

        public TypedItemViewModel TypedItemViewModel
        {
            get { return ViewModelObject as TypedItemViewModel; }
        }

        public TypedItemDrawer(TypedItemViewModel viewModel)
        {
            ViewModelObject = viewModel;
        }

        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);

            if (hardRefresh || string.IsNullOrEmpty(_cachedItemName) || string.IsNullOrEmpty(_cachedTypeName))
            {
                _cachedItemName = TypedItemViewModel.Name;
                _cachedTypeName = TypedItemViewModel.RelatedType;
                _nameSize = platform.CalculateTextSize(_cachedItemName, CachedStyles.ClearItemStyle);
                _typeSize = platform.CalculateTextSize(_cachedTypeName, CachedStyles.ItemTextEditingStyle);
            }


            Bounds = new Rect(position.x, position.y, _nameSize.x + 5 + _typeSize.x + 40, 18);
        }

        private Vector2 _typeSize;
        private string _cachedTypeName;
        private string _cachedItemName;

        public override void DrawOption()
        {
            base.DrawOption();
            // TODO implement in platform drawer

            //if (GUILayout.Button(
            //    TypedItemViewModel.TypeLabel + (TypedItemViewModel.IsMouseOver ? "..." : string.Empty),
            //    ElementDesignerStyles.ClearItemStyle))
            //{
            //    TypedItemViewModel.NodeViewModel.IsSelected = true;
            //    OptionClicked();
            //}
        }

        public override void OnMouseDown(MouseEvent mouseEvent)
        {
            //base.OnMouseDown(mouseEvent);
            //InvertApplication.SignalEvent<IOnMouseDownEvent>(_ => _.OnMouseDown(this, mouseEvent));        
        }

        public virtual void OptionClicked()
        {
            if (TypedItemViewModel.IsEditable) TypedItemViewModel.EndEditing();
            TypedItemViewModel.Select();
            TypedItemViewModel.ShowSelectionListWindow();

        }

        public override void Draw(IPlatformDrawer platform, float scale)
        {

            DrawBackground(platform, scale);
            var b = new Rect(Bounds);
            b.x += 10;
            b.width -= 20;
            //base.Draw(platform, scale);
            platform.DrawColumns(b.Scale(scale), new float[] { _typeSize.x + 5, _nameSize.x },
                _ =>
                {
                    platform.DoButton(_, _cachedTypeName, CachedStyles.ClearItemStyle, OptionClicked, OptionRightClicked);
                },
                _ =>
                {
                    DrawName(_, platform, scale, DrawingAlignment.MiddleRight);
                    platform.DoButton(_, "", CachedStyles.ClearItemStyle, () =>
                    {
                        if (ItemViewModel.IsSelected)
                        {
                            //TODO: Eliminate hack: due to the inconsistent input mechanisms, I cannot fix: when clicking on type, type window appear, then click on the property,
                            //editing will begin, but type window will stay there. So here is a hack of signaling HideSelection event.

                            InvertApplication.SignalEvent<IHideSelectionMenu>(__ => __.HideSelection());

                            ItemViewModel.BeginEditing();
                        }
                        else ItemViewModel.Select();
                    }, OptionRightClicked);
                });
        }

        public virtual void OptionRightClicked()
        {
            if (!this.ItemViewModel.Enabled) return;
            // TODO 2.0 Quick Types Right Click menu
            InvertApplication.SignalEvent<IShowContextMenu>(_ => _.Show(null, this.ViewModelObject));
            //var menu = InvertGraphEditor.CreateCommandUI<ContextMenuUI>(true, typeof(IDiagramNodeItemCommand));

            //var types = InvertGraphEditor.TypesContainer.ResolveAll<GraphTypeInfo>();
            //foreach (var type in types)
            //{
            //    var type1 = type;
            //    menu.AddCommand(new SimpleEditorCommand<GenericTypedChildItem>((_) =>
            //    {
            //        _.RelatedType = type1.Name;
            //    },type1.Label,type1.Group));
            //}
            //foreach (var type in InvertGraphEditor.CurrentDiagramViewModel.CurrentNodes)
            //{
            //    var type1 = type;
            //    menu.AddCommand(new SimpleEditorCommand<GenericTypedChildItem>((_) =>
            //    {
            //        _.RelatedType = type1.Identifier;
            //    }, type1.Label, type1.Group)); 
            //}

            //menu.Go();
        }
    }
}
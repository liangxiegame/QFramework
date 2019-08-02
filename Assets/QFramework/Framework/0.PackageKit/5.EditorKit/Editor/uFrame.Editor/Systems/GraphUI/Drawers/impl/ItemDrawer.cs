using System;
using System.Linq;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class ItemDrawer<TViewModel> : ItemDrawer where TViewModel : ItemViewModel
    {
        public TViewModel ViewModel
        {
            get { return ViewModelObject as TViewModel; }
        }

        public ItemDrawer(TViewModel viewModelObject)
            : base(viewModelObject)
        {
        }
    }

    public class ItemDrawer : Drawer
    {
        public ItemDrawer(GraphItemViewModel viewModelObject)
            : base(viewModelObject)
        {
        }

        public override Rect Bounds
        {
            get { return ViewModelObject.Bounds; }
            set { ViewModelObject.Bounds = value; }
        }

        private object _textStyle;
        private object _backgroundStyle;
        private Vector2 _textSize;

        public string CachedName { get; private set; }


        public ItemViewModel ItemViewModel
        {
            get { return this.ViewModelObject as ItemViewModel; }
        }

        public ItemDrawer()
        {
        }

        public virtual int Padding
        {
            get { return 4; }
        }

        public object BackgroundStyle
        {
            get { return _backgroundStyle ?? (_backgroundStyle = CachedStyles.Item4); }
            set { _backgroundStyle = value; }
        }

        public object SelectedItemStyle
        {
            get
            {
                if (ViewModelObject.IsSelected)
                    return CachedStyles.Item1;
                if (ViewModelObject.IsMouseOver)
                    return CachedStyles.Item5;


                return CachedStyles.ClearItemStyle;
            }
            set {  }
        }

        public virtual object TextStyle
        {
            get { return _textStyle ?? (_textStyle = CachedStyles.ClearItemStyle); }
            set { _textStyle = value; }
        }

        public override void OnMouseEnter(MouseEvent e)
        {
            base.OnMouseEnter(e);
            ViewModelObject.IsMouseOver = true;
            //Debug.Log("Mouse Enter Item");
        }

        public override void OnMouseExit(MouseEvent e)
        {
            base.OnMouseExit(e);
            ViewModelObject.IsMouseOver = false;
        }

        public override void OnMouseDown(MouseEvent mouseEvent)
        {
            base.OnMouseDown(mouseEvent);
            if (!this.Enabled) return;
            if (mouseEvent.MouseButton != 0)
            {
                if (!ViewModelObject.IsSelected)
                ViewModelObject.Select();
            }
            else
            {
                ViewModelObject.Select();
            }
            
            
        }

        public override void OnMouseDoubleClick(MouseEvent mouseEvent)
        {
            base.OnMouseDoubleClick(mouseEvent);
            if (!this.Enabled) return;
            mouseEvent.NoBubble = true;
        }
	    private IFlagItem[] _cachedFlags;
        public override void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true)
        {
            base.Refresh(platform, position, hardRefresh);
            // Calculate the size of the label and add the padding * 2 for left and right
            CachedName = ItemViewModel.Name;
            
	        if (hardRefresh || _cachedFlags == null)
            {
		        _cachedFlags = this.ItemViewModel.ItemFlags.ToArray();
                _textSize = platform.CalculateTextSize(CachedName, CachedStyles.ItemTextEditingStyle);// TextStyle.CalcSize(new GUIContent(ItemViewModel.Name));
            }
            var flagWidth = 4f;

            var width = _textSize.x + (Padding * 2) + (_cachedFlags.Length * flagWidth);
            var height = _textSize.y + (Padding * 2);
            
            

            this.Bounds = new Rect(position.x, position.y, width, height);
            

        }

        public override void OnLayout()
        {
            base.OnLayout();
            var cb = new Rect(Bounds);
            cb.x += 16;
            cb.width -= 14;
            ViewModelObject.ConnectorBounds = cb;
        }

        public virtual void DrawOption()
        {

        }

        public virtual void DrawBackground(IPlatformDrawer platform, float scale)
        {
            if (IsSelected || this.ItemViewModel.IsEditing)
            {
                platform.DrawStretchBox(Bounds.Scale(scale), CachedStyles.Item5, 0f);
            }

            if (_cachedFlags != null)
            for (int index = 0; index < _cachedFlags.Length; index++)
            {
                var item = _cachedFlags[index];
              
                var boundsRect = new Rect(this.Bounds);
                boundsRect.x += (4f * index);
                boundsRect.width = 4f;
                    
	            platform.DrawRect(boundsRect, CachedStyles.GetColor(item.Color));
            }
        }


        public override void Draw(IPlatformDrawer platform, float scale)
        {
            base.Draw(platform, scale);
            
            DrawBackground(platform,scale);
            DrawName(Bounds, platform, scale);

            // TODO Platform specific
#if UNITY_EDITOR
            //GUI.Box(Bounds.Scale(scale), string.Empty, SelectedItemStyle);

            //GUILayout.BeginArea(Bounds.Scale(scale));
            //EditorGUILayout.BeginHorizontal();
            //DrawOption();
            //if (ItemViewModel.IsSelected && ItemViewModel.IsEditable)
            //{
            //    EditorGUI.BeginChangeCheck();
            //    GUI.SetNextControlName("EditingField");
            //    DiagramDrawer.IsEditingField = true;
            //    var newName = EditorGUILayout.TextField(ItemViewModel.Name, ElementDesignerStyles.ItemTextEditingStyle);
            //    if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(newName))
            //    {
            //        ItemViewModel.Rename(newName);
            //    }
            //}
            //else
            //{
            //    GUILayout.Label(ItemViewModel.Label, ElementDesignerStyles.SelectedItemStyle);
            //} 
            //if (ItemViewModel.AllowRemoving && ItemViewModel.IsSelected)
            //    if (GUILayout.Button(string.Empty, ElementDesignerStyles.RemoveButtonStyle.Scale(scale)))
            //    {
            //        InvertGraphEditor.ExecuteCommand(ItemViewModel.RemoveItemCommand);
            //    }
            //EditorGUILayout.EndHorizontal();
            //GUILayout.EndArea();
            //if (!string.IsNullOrEmpty(ItemViewModel.Highlighter))
            //{
            //    var highlighterPosition = new Rect(Bounds) { width = 4 };
            //    highlighterPosition.y += 2;
            //    highlighterPosition.x += 2;
            //    highlighterPosition.height = Bounds.height - 6;
            //    GUI.Box(highlighterPosition.Scale(scale), string.Empty,
            //        ElementDesignerStyles.GetHighlighter(ItemViewModel.Highlighter));
            //}
#endif
        }

        protected void DrawName(Rect rect, IPlatformDrawer platform, float scale,DrawingAlignment alignment = DrawingAlignment.MiddleCenter)
        {
   
            if (ItemViewModel.IsEditing && ItemViewModel.IsEditable && this.ItemViewModel.Enabled)
            {
                platform.DrawTextbox(ItemViewModel.NodeItem.Identifier, rect.Scale(scale), CachedName,
                    CachedStyles.ItemTextEditingStyle,
                    (s, finished) =>
                    {
                       
                        CachedName = s;
                        ItemViewModel.Rename(s);
                        //CachedName = ItemViewModel.Name;
                        if (finished)
                        {
                            ItemViewModel.EndEditing();
                        }
                    });
            }
            else
            {
                platform.DrawLabel(rect.Scale(scale), CachedName, CachedStyles.ItemTextEditingStyle, alignment);
            }
        }

    }

}
using UnityEditor;
using UnityEngine;

namespace EGO.Framework
{
    public class TreeNode : VerticalLayout
    {
        public Property<bool> Spread = null;

        public string Content;


        HorizontalLayout mFirstLine = new HorizontalLayout();

        private VerticalLayout mSpreadView = new VerticalLayout();

        public TreeNode(bool spread, string content,int indent = 0)
        {
            Content = content;
            Spread = new Property<bool>(spread);

            Style = new GUIStyle(EditorStyles.foldout);
            
            mFirstLine.AddTo(this);
            mFirstLine.AddChild(new SpaceView(indent));

            new CustomView(() => { Spread.Value = EditorGUILayout.Foldout(Spread.Value, Content, true,Style); }).AddTo(mFirstLine);
            
            new CustomView(() =>
            {
                if (Spread.Value)
                {
                    mSpreadView.DrawGUI(); 
                } 
                
            }).AddTo(this);
        }

        public TreeNode Add2FirstLine(IView view)
        {
            view.AddTo(mFirstLine);
            return this;
        }

        public TreeNode FirstLineBox()
        {
            mFirstLine.HorizontalStyle = "box";

            return this;
        }

        public TreeNode SpreadBox()
        {
            mSpreadView.VerticalStyle = "box";

            return this;
        }

        public TreeNode Add2Spread(IView view)
        {
            view.AddTo(mSpreadView);
            return this;
        }
    }
}
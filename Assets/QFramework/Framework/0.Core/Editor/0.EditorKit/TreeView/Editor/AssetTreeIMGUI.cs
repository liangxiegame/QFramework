using UnityEngine;
using UnityEditor;
using System.Collections;

/**
 * TreeNode.cs
 * Author: Luke Holland (http://lukeholland.me/)
 */

namespace TreeView {

	public class AssetTreeIMGUI : TreeIMGUI<AssetData>
	{

		public AssetTreeIMGUI(TreeNode<AssetData> root) : base(root)
		{
			
		}

		protected override void OnDrawTreeNode(Rect rect, TreeNode<AssetData> node, bool selected, bool focus)
		{
			GUIContent labelContent = new GUIContent(node.Data.path,AssetDatabase.GetCachedIcon(node.Data.fullPath));

			if(!node.IsLeaf){
				node.Data.isExpanded = EditorGUI.Foldout(new Rect(rect.x-12,rect.y,12,rect.height),node.Data.isExpanded,GUIContent.none);
			}

			EditorGUI.LabelField(rect,labelContent,selected ? EditorStyles.whiteLabel : EditorStyles.label);
		}

	}

}
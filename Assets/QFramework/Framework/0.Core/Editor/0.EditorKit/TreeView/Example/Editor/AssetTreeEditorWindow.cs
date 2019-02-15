using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/**
 * TreeNode.cs
 * Author: Luke Holland (http://lukeholland.me/)
 */

namespace TreeView {

	public class AssetTreeEditorWindow : EditorWindow 
	{

		[MenuItem("Tools/Tree View Example Window")]
		private static void CreateEditorWindow()
		{
			GetWindow<AssetTreeEditorWindow>("Asset Tree");
		}

		private AssetTree _assetTree;
		private AssetTreeIMGUI _assetTreeGUI;
		private Vector2 _scrollPosition;
		private string _searchString;

		protected void OnEnable()
		{
			// setup
			_assetTree = new AssetTree();
			_assetTreeGUI = new AssetTreeIMGUI(_assetTree.Root);

			// fill tree with example data
			_searchString = "t:Script";
			Search(_searchString);
		}

		protected void OnGUI()
		{
			// draw search
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

			EditorGUILayout.LabelField("Search:",EditorStyles.miniLabel,GUILayout.Width(40));

			EditorGUI.BeginChangeCheck();
			_searchString = EditorGUILayout.TextField(_searchString,EditorStyles.toolbarTextField,GUILayout.ExpandWidth(true));
			if(EditorGUI.EndChangeCheck()){
				Search(_searchString);
			}
				
			EditorGUILayout.EndHorizontal();

			// draw tree
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

			_assetTreeGUI.DrawTreeLayout();

			EditorGUILayout.EndScrollView();
		}

		private void Search(string query)
		{
			_assetTree.Clear();

			string[] guids = AssetDatabase.FindAssets(query);
			int i = 0, l = guids.Length;
			for(; i<l; ++i){
				_assetTree.AddAsset(guids[i]);
			}
		}

	}

}
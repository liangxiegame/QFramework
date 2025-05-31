/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

#if UNITY_EDITOR
using System.Xml.Linq;
using UnityEditor;

namespace QFramework
{
    /// <summary>
    /// This asset processor resolves an issue with the new v2 AssetDatabase system present on 2019.3 and later. When
    /// renaming a <see cref="XNode.NodeGraph"/> asset, it appears that sometimes the v2 AssetDatabase will swap which asset
    /// is the main asset (present at top level) between the <see cref="XNode.NodeGraph"/> and one of its <see cref="XNode.Node"/>
    /// sub-assets. As a workaround until Unity fixes this, this asset processor checks all renamed assets and if it
    /// finds a case where a <see cref="XNode.Node"/> has been made the main asset it will swap it back to being a sub-asset
    /// and rename the node to the default name for that node type.
    /// </summary>
    internal sealed class GUIGraphRenameFixAssetProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            for (int i = 0; i < movedAssets.Length; i++)
            {
                GUIGraphNode nodeAsset = AssetDatabase.LoadMainAssetAtPath(movedAssets[i]) as GUIGraphNode;

                // If the renamed asset is a node graph, but the v2 AssetDatabase has swapped a sub-asset node to be its
                // main asset, reset the node graph to be the main asset and rename the node asset back to its default
                // name.
                if (nodeAsset != null && AssetDatabase.IsMainAsset(nodeAsset))
                {
                    AssetDatabase.SetMainObject(nodeAsset.graph, movedAssets[i]);
                    AssetDatabase.ImportAsset(movedAssets[i]);

                    nodeAsset.name = GUIGraphUtilities.NodeDefaultName(nodeAsset.GetType());
                    EditorUtility.SetDirty(nodeAsset);
                }
            }
        }
    }
}
#endif
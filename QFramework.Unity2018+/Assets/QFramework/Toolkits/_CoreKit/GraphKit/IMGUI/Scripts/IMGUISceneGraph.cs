/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    /// <summary> Lets you instantiate a node graph in the scene. This allows you to reference in-scene objects. </summary>
    public class GUISceneGraph : MonoBehaviour
    {
        public GUIGraph graph;
    }

    /// <summary> Derive from this class to create a SceneGraph with a specific graph type. </summary>
    /// <example>
    /// <code>
    /// public class MySceneGraph : GUISceneGraph<MyGraph> {
    /// 	
    /// }
    /// </code>
    /// </example>
    public class GUISceneGraph<T> : GUISceneGraph where T : GUIGraph
    {
        public new T graph
        {
            get => base.graph as T;
            set => base.graph = value;
        }
    }
}
/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

using UnityEngine;

namespace QFramework.Pro
{
    /// <summary> Lets you instantiate a node graph in the scene. This allows you to reference in-scene objects. </summary>
    public class IMGUISceneGraph : MonoBehaviour
    {
        public IMGUIGraph graph;
    }

    /// <summary> Derive from this class to create a SceneGraph with a specific graph type. </summary>
    /// <example>
    /// <code>
    /// public class MySceneGraph : SceneGraph<MyGraph> {
    /// 	
    /// }
    /// </code>
    /// </example>
    public class IMGUISceneGraph<T> : IMGUISceneGraph where T : IMGUIGraph
    {
        public new T graph
        {
            get => base.graph as T;
            set => base.graph = value;
        }
    }
}
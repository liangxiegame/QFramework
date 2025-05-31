/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// Base class for all nodes
    /// </summary>
    /// <example>
    /// Classes extending this class will be considered as valid nodes by xNode.
    /// <code>
    /// [System.Serializable]
    /// public class Adder : Node {
    ///     [Input] public float a;
    ///     [Input] public float b;
    ///     [Output] public float result;
    ///
    ///     // GetValue should be overridden to return a value for any specified output port
    ///     public override object GetValue(NodePort port) {
    ///         return a + b;
    ///     }
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public abstract class GUIGraphNode : ScriptableObject
    {
        /// <summary> Used by <see cref="InputAttribute"/> and <see cref="OutputAttribute"/> to determine when to display the field value associated with a <see cref="NodePort"/> </summary>
        public enum ShowBackingValue
        {
            /// <summary> Never show the backing value </summary>
            Never,

            /// <summary> Show the backing value only when the port does not have any active connections </summary>
            Unconnected,

            /// <summary> Always show the backing value </summary>
            Always
        }

        public enum ConnectionType
        {
            /// <summary> Allow multiple connections</summary>
            Multiple,

            /// <summary> always override the current connection </summary>
            Override,
        }

        /// <summary> Tells which types of input to allow </summary>
        public enum TypeConstraint
        {
            /// <summary> Allow all types of input</summary>
            None,

            /// <summary> Allow connections where input value type is assignable from output value type (eg. ScriptableObject --> Object)</summary>
            Inherited,

            /// <summary> Allow only similar types </summary>
            Strict,

            /// <summary> Allow connections where output value type is assignable from input value type (eg. Object --> ScriptableObject)</summary>
            InheritedInverse,
        }


        /// <summary> Iterate over all ports on this node. </summary>
        public IEnumerable<GUIGraphNodePort> Ports
        {
            get
            {
                foreach (GUIGraphNodePort port in ports.Values) yield return port;
            }
        }

        /// <summary> Iterate over all outputs on this node. </summary>
        public IEnumerable<GUIGraphNodePort> Outputs
        {
            get
            {
                foreach (GUIGraphNodePort port in Ports)
                {
                    if (port.IsOutput) yield return port;
                }
            }
        }

        /// <summary> Iterate over all inputs on this node. </summary>
        public IEnumerable<GUIGraphNodePort> Inputs
        {
            get
            {
                foreach (GUIGraphNodePort port in Ports)
                {
                    if (port.IsInput) yield return port;
                }
            }
        }

        /// <summary> Iterate over all dynamic ports on this node. </summary>
        public IEnumerable<GUIGraphNodePort> DynamicPorts
        {
            get
            {
                foreach (GUIGraphNodePort port in Ports)
                {
                    if (port.IsDynamic) yield return port;
                }
            }
        }

        /// <summary> Iterate over all dynamic outputs on this node. </summary>
        public IEnumerable<GUIGraphNodePort> DynamicOutputs
        {
            get
            {
                foreach (GUIGraphNodePort port in Ports)
                {
                    if (port.IsDynamic && port.IsOutput) yield return port;
                }
            }
        }

        /// <summary> Iterate over all dynamic inputs on this node. </summary>
        public IEnumerable<GUIGraphNodePort> DynamicInputs
        {
            get
            {
                foreach (GUIGraphNodePort port in Ports)
                {
                    if (port.IsDynamic && port.IsInput) yield return port;
                }
            }
        }

        /// <summary> Parent <see cref="NodeGraph"/> </summary>
        [SerializeField] public GUIGraph graph;

        /// <summary> Position on the <see cref="NodeGraph"/> </summary>
        [SerializeField] public Vector2 position;

        /// <summary> It is recommended not to modify these at hand. Instead, see <see cref="InputAttribute"/> and <see cref="OutputAttribute"/> </summary>
        [SerializeField] private NodePortDictionary ports = new NodePortDictionary();

        /// <summary> Used during node instantiation to fix null/misconfigured graph during OnEnable/Init. Set it before instantiating a node. Will automatically be unset during OnEnable </summary>
        public static GUIGraph graphHotfix;

        protected void OnEnable()
        {
            if (graphHotfix != null) graph = graphHotfix;
            graphHotfix = null;
            UpdatePorts();
            Init();
        }

        /// <summary> Update static ports and dynamic ports managed by DynamicPortLists to reflect class fields. This happens automatically on enable or on redrawing a dynamic port list. </summary>
        public void UpdatePorts()
        {
            GUIGraphDataCache.UpdatePorts(this, ports);
        }

        /// <summary> Initialize node. Called on enable. </summary>
        protected virtual void Init()
        {
        }

        /// <summary> Checks all connections for invalid references, and removes them. </summary>
        public void VerifyConnections()
        {
            foreach (GUIGraphNodePort port in Ports) port.VerifyConnections();
        }

        #region Dynamic Ports

        /// <summary> Convenience function. </summary>
        /// <seealso cref="AddInstancePort"/>
        /// <seealso cref="AddInstanceOutput"/>
        public GUIGraphNodePort AddDynamicInput(Type type,
            GUIGraphNode.ConnectionType connectionType = GUIGraphNode.ConnectionType.Multiple,
            GUIGraphNode.TypeConstraint typeConstraint = TypeConstraint.None, string fieldName = null)
        {
            return AddDynamicPort(type, GUIGraphNodePort.IO.Input, connectionType, typeConstraint, fieldName);
        }

        /// <summary> Convenience function. </summary>
        /// <seealso cref="AddInstancePort"/>
        /// <seealso cref="AddInstanceInput"/>
        public GUIGraphNodePort AddDynamicOutput(Type type,
            GUIGraphNode.ConnectionType connectionType = GUIGraphNode.ConnectionType.Multiple,
            GUIGraphNode.TypeConstraint typeConstraint = TypeConstraint.None, string fieldName = null)
        {
            return AddDynamicPort(type, GUIGraphNodePort.IO.Output, connectionType, typeConstraint, fieldName);
        }

        /// <summary> Add a dynamic, serialized port to this node. </summary>
        /// <seealso cref="AddDynamicInput"/>
        /// <seealso cref="AddDynamicOutput"/>
        private GUIGraphNodePort AddDynamicPort(Type type, GUIGraphNodePort.IO direction,
            GUIGraphNode.ConnectionType connectionType = GUIGraphNode.ConnectionType.Multiple,
            GUIGraphNode.TypeConstraint typeConstraint = TypeConstraint.None, string fieldName = null)
        {
            if (fieldName == null)
            {
                fieldName = "dynamicInput_0";
                int i = 0;
                while (HasPort(fieldName)) fieldName = "dynamicInput_" + (++i);
            }
            else if (HasPort(fieldName))
            {
                Debug.LogWarning("Port '" + fieldName + "' already exists in " + name, this);
                return ports[fieldName];
            }

            GUIGraphNodePort port =
                new GUIGraphNodePort(fieldName, type, direction, connectionType, typeConstraint, this);
            ports.Add(fieldName, port);
            return port;
        }

        /// <summary> Remove an dynamic port from the node </summary>
        public void RemoveDynamicPort(string fieldName)
        {
            GUIGraphNodePort dynamicPort = GetPort(fieldName);
            if (dynamicPort == null) throw new ArgumentException("port " + fieldName + " doesn't exist");
            RemoveDynamicPort(GetPort(fieldName));
        }

        /// <summary> Remove an dynamic port from the node </summary>
        public void RemoveDynamicPort(GUIGraphNodePort port)
        {
            if (port == null) throw new ArgumentNullException("port");
            else if (port.IsStatic) throw new ArgumentException("cannot remove static port");
            port.ClearConnections();
            ports.Remove(port.fieldName);
        }

        /// <summary> Removes all dynamic ports from the node </summary>
        [ContextMenu("Clear Dynamic Ports")]
        public void ClearDynamicPorts()
        {
            List<GUIGraphNodePort> dynamicPorts = new List<GUIGraphNodePort>(DynamicPorts);
            foreach (GUIGraphNodePort port in dynamicPorts)
            {
                RemoveDynamicPort(port);
            }
        }

        #endregion

        #region Ports

        /// <summary> Returns output port which matches fieldName </summary>
        public GUIGraphNodePort GetOutputPort(string fieldName)
        {
            GUIGraphNodePort port = GetPort(fieldName);
            if (port == null || port.direction != GUIGraphNodePort.IO.Output) return null;
            else return port;
        }

        /// <summary> Returns input port which matches fieldName </summary>
        public GUIGraphNodePort GetInputPort(string fieldName)
        {
            GUIGraphNodePort port = GetPort(fieldName);
            if (port == null || port.direction != GUIGraphNodePort.IO.Input) return null;
            else return port;
        }

        /// <summary> Returns port which matches fieldName </summary>
        public GUIGraphNodePort GetPort(string fieldName)
        {
            GUIGraphNodePort port;
            if (ports.TryGetValue(fieldName, out port)) return port;
            else return null;
        }

        public bool HasPort(string fieldName)
        {
            return ports.ContainsKey(fieldName);
        }

        #endregion

        #region Inputs/Outputs

        /// <summary> Return input value for a specified port. Returns fallback value if no ports are connected </summary>
        /// <param name="fieldName">Field name of requested input port</param>
        /// <param name="fallback">If no ports are connected, this value will be returned</param>
        public T GetInputValue<T>(string fieldName, T fallback = default(T))
        {
            GUIGraphNodePort port = GetPort(fieldName);
            if (port != null && port.IsConnected) return port.GetInputValue<T>();
            else return fallback;
        }

        /// <summary> Return all input values for a specified port. Returns fallback value if no ports are connected </summary>
        /// <param name="fieldName">Field name of requested input port</param>
        /// <param name="fallback">If no ports are connected, this value will be returned</param>
        public T[] GetInputValues<T>(string fieldName, params T[] fallback)
        {
            GUIGraphNodePort port = GetPort(fieldName);
            if (port != null && port.IsConnected) return port.GetInputValues<T>();
            else return fallback;
        }

        /// <summary> Returns a value based on requested port output. Should be overridden in all derived nodes with outputs. </summary>
        /// <param name="port">The requested port.</param>
        public virtual object GetValue(GUIGraphNodePort port)
        {
            Debug.LogWarning("No GetValue(NodePort port) override defined for " + GetType());
            return null;
        }

        #endregion

        /// <summary> Called after a connection between two <see cref="NodePort"/>s is created </summary>
        /// <param name="from">Output</param> <param name="to">Input</param>
        public virtual void OnCreateConnection(GUIGraphNodePort from, GUIGraphNodePort to)
        {
        }

        /// <summary> Called after a connection is removed from this port </summary>
        /// <param name="port">Output or Input</param>
        public virtual void OnRemoveConnection(GUIGraphNodePort port)
        {
        }

        /// <summary> Disconnect everything from this node </summary>
        public void ClearConnections()
        {
            foreach (GUIGraphNodePort port in Ports) port.ClearConnections();
        }

        #region Attributes

        /// <summary> Mark a serializable field as an input port. You can access this through <see cref="GetInputPort(string)"/> </summary>
        [AttributeUsage(AttributeTargets.Field)]
        public class InputAttribute : Attribute
        {
            public ShowBackingValue backingValue;
            public ConnectionType connectionType;

            [Obsolete("Use dynamicPortList instead")]
            public bool instancePortList
            {
                get { return dynamicPortList; }
                set { dynamicPortList = value; }
            }

            public bool dynamicPortList;
            public TypeConstraint typeConstraint;

            /// <summary> Mark a serializable field as an input port. You can access this through <see cref="GetInputPort(string)"/> </summary>
            /// <param name="backingValue">Should we display the backing value for this port as an editor field? </param>
            /// <param name="connectionType">Should we allow multiple connections? </param>
            /// <param name="typeConstraint">Constrains which input connections can be made to this port </param>
            /// <param name="dynamicPortList">If true, will display a reorderable list of inputs instead of a single port. Will automatically add and display values for lists and arrays </param>
            public InputAttribute(ShowBackingValue backingValue = ShowBackingValue.Unconnected,
                ConnectionType connectionType = ConnectionType.Multiple,
                TypeConstraint typeConstraint = TypeConstraint.None, bool dynamicPortList = false)
            {
                this.backingValue = backingValue;
                this.connectionType = connectionType;
                this.dynamicPortList = dynamicPortList;
                this.typeConstraint = typeConstraint;
            }
        }

        /// <summary> Mark a serializable field as an output port. You can access this through <see cref="GetOutputPort(string)"/> </summary>
        [AttributeUsage(AttributeTargets.Field)]
        public class OutputAttribute : Attribute
        {
            public ShowBackingValue backingValue;
            public ConnectionType connectionType;

            [Obsolete("Use dynamicPortList instead")]
            public bool instancePortList
            {
                get { return dynamicPortList; }
                set { dynamicPortList = value; }
            }

            public bool dynamicPortList;
            public TypeConstraint typeConstraint;

            /// <summary> Mark a serializable field as an output port. You can access this through <see cref="GetOutputPort(string)"/> </summary>
            /// <param name="backingValue">Should we display the backing value for this port as an editor field? </param>
            /// <param name="connectionType">Should we allow multiple connections? </param>
            /// <param name="typeConstraint">Constrains which input connections can be made from this port </param>
            /// <param name="dynamicPortList">If true, will display a reorderable list of outputs instead of a single port. Will automatically add and display values for lists and arrays </param>
            public OutputAttribute(ShowBackingValue backingValue = ShowBackingValue.Never,
                ConnectionType connectionType = ConnectionType.Multiple,
                TypeConstraint typeConstraint = TypeConstraint.None, bool dynamicPortList = false)
            {
                this.backingValue = backingValue;
                this.connectionType = connectionType;
                this.dynamicPortList = dynamicPortList;
                this.typeConstraint = typeConstraint;
            }

            /// <summary> Mark a serializable field as an output port. You can access this through <see cref="GetOutputPort(string)"/> </summary>
            /// <param name="backingValue">Should we display the backing value for this port as an editor field? </param>
            /// <param name="connectionType">Should we allow multiple connections? </param>
            /// <param name="dynamicPortList">If true, will display a reorderable list of outputs instead of a single port. Will automatically add and display values for lists and arrays </param>
            [Obsolete("Use constructor with TypeConstraint")]
            public OutputAttribute(ShowBackingValue backingValue, ConnectionType connectionType, bool dynamicPortList) :
                this(backingValue, connectionType, TypeConstraint.None, dynamicPortList)
            {
            }
        }

        /// <summary> Manually supply node class with a context menu path </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class CreateNodeMenuAttribute : Attribute
        {
            public string menuName;
            public int order;

            /// <summary> Manually supply node class with a context menu path </summary>
            /// <param name="menuName"> Path to this node in the context menu. Null or empty hides it. </param>
            public CreateNodeMenuAttribute(string menuName)
            {
                this.menuName = menuName;
                this.order = 0;
            }

            /// <summary> Manually supply node class with a context menu path </summary>
            /// <param name="menuName"> Path to this node in the context menu. Null or empty hides it. </param>
            /// <param name="order"> The order by which the menu items are displayed. </param>
            public CreateNodeMenuAttribute(string menuName, int order)
            {
                this.menuName = menuName;
                this.order = order;
            }
        }

        /// <summary> Prevents Node of the same type to be added more than once (configurable) to a NodeGraph </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class DisallowMultipleNodesAttribute : Attribute
        {
            // TODO: Make inheritance work in such a way that applying [DisallowMultipleNodes(1)] to type NodeBar : Node
            //       while type NodeFoo : NodeBar exists, will let you add *either one* of these nodes, but not both.
            public int max;

            /// <summary> Prevents Node of the same type to be added more than once (configurable) to a NodeGraph </summary>
            /// <param name="max"> How many nodes to allow. Defaults to 1. </param>
            public DisallowMultipleNodesAttribute(int max = 1)
            {
                this.max = max;
            }
        }

        /// <summary> Specify a color for this node type </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class NodeTintAttribute : Attribute
        {
            public Color color;

            /// <summary> Specify a color for this node type </summary>
            /// <param name="r"> Red [0.0f .. 1.0f] </param>
            /// <param name="g"> Green [0.0f .. 1.0f] </param>
            /// <param name="b"> Blue [0.0f .. 1.0f] </param>
            public NodeTintAttribute(float r, float g, float b)
            {
                color = new Color(r, g, b);
            }

            /// <summary> Specify a color for this node type </summary>
            /// <param name="hex"> HEX color value </param>
            public NodeTintAttribute(string hex)
            {
                ColorUtility.TryParseHtmlString(hex, out color);
            }

            /// <summary> Specify a color for this node type </summary>
            /// <param name="r"> Red [0 .. 255] </param>
            /// <param name="g"> Green [0 .. 255] </param>
            /// <param name="b"> Blue [0 .. 255] </param>
            public NodeTintAttribute(byte r, byte g, byte b)
            {
                color = new Color32(r, g, b, byte.MaxValue);
            }
        }

        /// <summary> Specify a width for this node type </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class NodeWidthAttribute : Attribute
        {
            public int width;

            /// <summary> Specify a width for this node type </summary>
            /// <param name="width"> Width </param>
            public NodeWidthAttribute(int width)
            {
                this.width = width;
            }
        }

        #endregion

        [Serializable]
        private class NodePortDictionary : Dictionary<string, GUIGraphNodePort>, ISerializationCallbackReceiver
        {
            [SerializeField] private List<string> keys = new List<string>();
            [SerializeField] private List<GUIGraphNodePort> values = new List<GUIGraphNodePort>();

            public void OnBeforeSerialize()
            {
                keys.Clear();
                values.Clear();
                foreach (KeyValuePair<string, GUIGraphNodePort> pair in this)
                {
                    keys.Add(pair.Key);
                    values.Add(pair.Value);
                }
            }

            public void OnAfterDeserialize()
            {
                this.Clear();

                if (keys.Count != values.Count)
                    throw new System.Exception("there are " + keys.Count + " keys and " + values.Count +
                                               " values after deserialization. Make sure that both key and value types are serializable.");

                for (int i = 0; i < keys.Count; i++)
                    this.Add(keys[i], values[i]);
            }
        }
    }
}
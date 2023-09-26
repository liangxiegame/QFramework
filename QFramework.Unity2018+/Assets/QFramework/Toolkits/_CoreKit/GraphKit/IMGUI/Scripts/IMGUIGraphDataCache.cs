/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QFramework
{
    /// <summary> Precaches reflection data in editor so we won't have to do it runtime </summary>
    public static class GUIGraphDataCache
    {
        private static PortDataCache portDataCache;

        private static bool Initialized => portDataCache != null;

        /// <summary> Update static ports and dynamic ports managed by DynamicPortLists to reflect class fields. </summary>
        public static void UpdatePorts(GUIGraphNode node, Dictionary<string, GUIGraphNodePort> ports)
        {
            if (!Initialized) BuildCache();

            Dictionary<string, GUIGraphNodePort> staticPorts = new Dictionary<string, GUIGraphNodePort>();
            Dictionary<string, List<GUIGraphNodePort>> removedPorts =
                new Dictionary<string, List<GUIGraphNodePort>>();
            System.Type nodeType = node.GetType();

            List<GUIGraphNodePort> dynamicListPorts = new List<GUIGraphNodePort>();

            List<GUIGraphNodePort> typePortCache;
            if (portDataCache.TryGetValue(nodeType, out typePortCache))
            {
                for (int i = 0; i < typePortCache.Count; i++)
                {
                    staticPorts.Add(typePortCache[i].fieldName, portDataCache[nodeType][i]);
                }
            }

            // Cleanup port dict - Remove nonexisting static ports - update static port types
            // AND update dynamic ports (albeit only those in lists) too, in order to enforce proper serialisation.
            // Loop through current node ports
            foreach (GUIGraphNodePort port in ports.Values.ToList())
            {
                // If port still exists, check it it has been changed
                GUIGraphNodePort staticPort;
                if (staticPorts.TryGetValue(port.fieldName, out staticPort))
                {
                    // If port exists but with wrong settings, remove it. Re-add it later.
                    if (port.IsDynamic || port.direction != staticPort.direction ||
                        port.connectionType != staticPort.connectionType ||
                        port.typeConstraint != staticPort.typeConstraint)
                    {
                        // If port is not dynamic and direction hasn't changed, add it to the list so we can try reconnecting the ports connections.
                        if (!port.IsDynamic && port.direction == staticPort.direction)
                            removedPorts.Add(port.fieldName, port.GetConnections());
                        port.ClearConnections();
                        ports.Remove(port.fieldName);
                    }
                    else port.ValueType = staticPort.ValueType;
                }
                // If port doesn't exist anymore, remove it
                else if (port.IsStatic)
                {
                    port.ClearConnections();
                    ports.Remove(port.fieldName);
                }
                // If the port is dynamic and is managed by a dynamic port list, flag it for reference updates
                else if (IsDynamicListPort(port))
                {
                    dynamicListPorts.Add(port);
                }
            }

            // Add missing ports
            foreach (GUIGraphNodePort staticPort in staticPorts.Values)
            {
                if (!ports.ContainsKey(staticPort.fieldName))
                {
                    GUIGraphNodePort port = new GUIGraphNodePort(staticPort, node);
                    //If we just removed the port, try re-adding the connections
                    List<GUIGraphNodePort> reconnectConnections;
                    if (removedPorts.TryGetValue(staticPort.fieldName, out reconnectConnections))
                    {
                        for (int i = 0; i < reconnectConnections.Count; i++)
                        {
                            GUIGraphNodePort connection = reconnectConnections[i];
                            if (connection == null) continue;
                            if (port.CanConnectTo(connection)) port.Connect(connection);
                        }
                    }

                    ports.Add(staticPort.fieldName, port);
                }
            }

            // Finally, make sure dynamic list port settings correspond to the settings of their "backing port"
            foreach (GUIGraphNodePort listPort in dynamicListPorts)
            {
                // At this point we know that ports here are dynamic list ports
                // which have passed name/"backing port" checks, ergo we can proceed more safely.
                string backingPortName = listPort.fieldName.Split(' ')[0];
                GUIGraphNodePort backingPort = staticPorts[backingPortName];

                // Update port constraints. Creating a new port instead will break the editor, mandating the need for setters.
                listPort.ValueType = GetBackingValueType(backingPort.ValueType);
                listPort.direction = backingPort.direction;
                listPort.connectionType = backingPort.connectionType;
                listPort.typeConstraint = backingPort.typeConstraint;
            }
        }

        /// <summary>
        /// Extracts the underlying types from arrays and lists, the only collections for dynamic port lists
        /// currently supported. If the given type is not applicable (i.e. if the dynamic list port was not
        /// defined as an array or a list), returns the given type itself. 
        /// </summary>
        private static System.Type GetBackingValueType(System.Type portValType)
        {
            if (portValType.HasElementType)
            {
                return portValType.GetElementType();
            }

            if (portValType.IsGenericType && portValType.GetGenericTypeDefinition() == typeof(List<>))
            {
                return portValType.GetGenericArguments()[0];
            }

            return portValType;
        }

        /// <summary>Returns true if the given port is in a dynamic port list.</summary>
        private static bool IsDynamicListPort(GUIGraphNodePort port)
        {
            // Ports flagged as "dynamicPortList = true" end up having a "backing port" and a name with an index, but we have
            // no guarantee that a dynamic port called "output 0" is an element in a list backed by a static "output" port.
            // Thus, we need to check for attributes... (but at least we don't need to look at all fields this time)
            string[] fieldNameParts = port.fieldName.Split(' ');
            if (fieldNameParts.Length != 2) return false;

            FieldInfo backingPortInfo = port.node.GetType().GetField(fieldNameParts[0]);
            if (backingPortInfo == null) return false;

            object[] attribs = backingPortInfo.GetCustomAttributes(true);
            return attribs.Any(x =>
            {
                GUIGraphNode.InputAttribute inputAttribute = x as GUIGraphNode.InputAttribute;
                GUIGraphNode.OutputAttribute outputAttribute = x as GUIGraphNode.OutputAttribute;
                return inputAttribute != null && inputAttribute.dynamicPortList ||
                       outputAttribute != null && outputAttribute.dynamicPortList;
            });
        }

        /// <summary> Cache node types </summary>
        private static void BuildCache()
        {
            portDataCache = new PortDataCache();
            System.Type baseType = typeof(GUIGraphNode);
            List<System.Type> nodeTypes = new List<System.Type>();
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

            // Loop through assemblies and add node types to list
            foreach (Assembly assembly in assemblies)
            {
                // Skip certain dlls to improve performance
                string assemblyName = assembly.GetName().Name;
                int index = assemblyName.IndexOf('.');
                if (index != -1) assemblyName = assemblyName.Substring(0, index);
                switch (assemblyName)
                {
                    // The following assemblies, and sub-assemblies (eg. UnityEngine.UI) are skipped
                    case "UnityEditor":
                    case "UnityEngine":
                    case "System":
                    case "mscorlib":
                    case "Microsoft":
                        continue;
                    default:
                        nodeTypes.AddRange(assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
                            .ToArray());
                        break;
                }
            }

            for (int i = 0; i < nodeTypes.Count; i++)
            {
                CachePorts(nodeTypes[i]);
            }
        }

        public static List<FieldInfo> GetNodeFields(System.Type nodeType)
        {
            List<System.Reflection.FieldInfo> fieldInfo =
                new List<System.Reflection.FieldInfo>(
                    nodeType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

            // GetFields doesnt return inherited private fields, so walk through base types and pick those up
            System.Type tempType = nodeType;
            while ((tempType = tempType.BaseType) != typeof(GUIGraphNode))
            {
                FieldInfo[] parentFields = tempType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                for (int i = 0; i < parentFields.Length; i++)
                {
                    // Ensure that we do not already have a member with this type and name
                    FieldInfo parentField = parentFields[i];
                    if (fieldInfo.TrueForAll(x => x.Name != parentField.Name))
                    {
                        fieldInfo.Add(parentField);
                    }
                }
            }

            return fieldInfo;
        }

        private static void CachePorts(System.Type nodeType)
        {
            List<System.Reflection.FieldInfo> fieldInfo = GetNodeFields(nodeType);

            for (int i = 0; i < fieldInfo.Count; i++)
            {
                //Get InputAttribute and OutputAttribute
                object[] attribs = fieldInfo[i].GetCustomAttributes(true);
                GUIGraphNode.InputAttribute inputAttrib =
                    attribs.FirstOrDefault(x => x is GUIGraphNode.InputAttribute) as GUIGraphNode.InputAttribute;
                GUIGraphNode.OutputAttribute outputAttrib =
                    attribs.FirstOrDefault(x => x is GUIGraphNode.OutputAttribute) as GUIGraphNode.OutputAttribute;

                if (inputAttrib == null && outputAttrib == null) continue;

                if (inputAttrib != null && outputAttrib != null)
                    Debug.LogError("Field " + fieldInfo[i].Name + " of type " + nodeType.FullName +
                                   " cannot be both input and output.");
                else
                {
                    if (!portDataCache.ContainsKey(nodeType))
                        portDataCache.Add(nodeType, new List<GUIGraphNodePort>());
                    portDataCache[nodeType].Add(new GUIGraphNodePort(fieldInfo[i]));
                }
            }
        }

        [System.Serializable]
        private class PortDataCache : Dictionary<System.Type, List<GUIGraphNodePort>>, ISerializationCallbackReceiver
        {
            [SerializeField] private List<System.Type> keys = new List<System.Type>();
            [SerializeField] private List<List<GUIGraphNodePort>> values = new List<List<GUIGraphNodePort>>();

            // save the dictionary to lists
            public void OnBeforeSerialize()
            {
                keys.Clear();
                values.Clear();
                foreach (var pair in this)
                {
                    keys.Add(pair.Key);
                    values.Add(pair.Value);
                }
            }

            // load dictionary from lists
            public void OnAfterDeserialize()
            {
                this.Clear();

                if (keys.Count != values.Count)
                    throw new System.Exception(string.Format(
                        "there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));

                for (int i = 0; i < keys.Count; i++)
                    this.Add(keys[i], values[i]);
            }
        }
    }
}
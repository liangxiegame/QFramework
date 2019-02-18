using System.Collections.Generic;
using System.Linq;
using System.Text;
using QFramework;
using QFramework.Json;
using UnityEngine;

namespace uFrame.Serialization
{
    public class JsonStream : ISerializerStream
    {
        private Stack<JSONNode> _nodeStack;
        private ITypeResolver _typeResolver;
        private Dictionary<string, IUFSerializable> _referenceObjects = new Dictionary<string, IUFSerializable>();
        public JSONNode RootNode { get; set; }

        public Dictionary<string, IUFSerializable> ReferenceObjects
        {
            get { return _referenceObjects; }
            set { _referenceObjects = value; }
        }

        public ITypeResolver TypeResolver
        {
            get { return _typeResolver ?? (_typeResolver = new DefaultTypeResolver()); }
            set { _typeResolver = value; }
        }

        public bool DeepSerialize { get; set; }

        public JsonStream(JSONNode node)
        {
            RootNode = node;
        }

        public JsonStream()
        {
            RootNode = new JSONClass();
        }

        public JsonStream(string json)
        {
            RootNode = JSON.Parse(json);
        }

        public JsonStream(ITypeResolver typeResolver)
        {
            _typeResolver = typeResolver;
            RootNode = new JSONClass();
        }

        public JsonStream(ITypeResolver typeResolver, string json)
        {
            _typeResolver = typeResolver;
            RootNode = JSON.Parse(json);
        }

        public Stack<JSONNode> NodeStack
        {
            get { return _nodeStack ?? (_nodeStack = new Stack<JSONNode>()); }
            set { _nodeStack = value; }
        }

        public JSONNode CurrentNode
        {
            get
            {
                if (NodeStack.Count < 1)
                    return RootNode;
                return NodeStack.Peek();
            }
        }

        public void Push(string name, JSONNode node)
        {
            NodeStack.Push(node);
        }

        public void Pop()
        {
            NodeStack.Pop();
        }

        public IQFrameworkContainer DependencyContainer { get; set; }

        public void SerializeArray<T>(string name, IEnumerable<T> items)
        {
            SerializeObjectArray(name, items.Cast<object>());
        }

        public void SerializeObjectArray(string name, IEnumerable<object> items)
        {

            var array = new JSONArray();
            if (name == null)
                CurrentNode.Add(array);
            else
                CurrentNode.Add(name, array);
            Push(name, array);
            foreach (var item in items)
            {
                SerializeObject(null, item);
            }
            Pop();
        }
        public bool UseReferences { get; set; }

        public void SerializeObject(string name, object value)
        {
            if (value is int)
            {
                SerializeInt(name, (int)value);
                return;
            }
            if (value is string)
            {
                SerializeString(name, (string)value);
                return;
            }
            if (value is bool)
            {
                SerializeBool(name, (bool)value);
                return;
            }
            if (value is Vector2)
            {
                SerializeVector2(name, (Vector2)value);
                return;
            }
            if (value is Vector3)
            {
                SerializeVector3(name, (Vector3)value);
                return;
            }
            if (value is Quaternion)
            {
                SerializeQuaternion(name, (Quaternion)value);
                return;
            }
            if (value is double)
            {
                SerializeDouble(name, (double)value);
                return;
            }
            var cls = new JSONClass();

            if (name == null)
                CurrentNode.Add(cls);
            else
                CurrentNode.Add(name, cls);
            Push(name, cls);

            var serializable = value as IUFSerializable;
            if (serializable != null)
            {

                SerializeString("Identifier", serializable.Identifier);
                if (!UseReferences || !ReferenceObjects.ContainsKey(serializable.Identifier))
                {
                    SerializeString("CLRType", TypeResolver.SetType(value.GetType()));

                    if (UseReferences)
                        ReferenceObjects.Add(serializable.Identifier, serializable);

                    serializable.Write(this);
                }

            }
            Pop();
        }

        public void SerializeInt(string name, int value)
        {
            CurrentNode.Add(name, new JSONData(value));
        }

        public void SerializeBool(string name, bool value)
        {
            CurrentNode.Add(name, new JSONData(value));
        }

        public void SerializeString(string name, string value)
        {
            CurrentNode.Add(name, new JSONData(value));
        }

        public void SerializeVector2(string name, Vector2 value)
        {
            CurrentNode.Add(name, new JSONClass() { AsVector2 = value });
        }

        public void SerializeVector3(string name, Vector3 value)
        {
            CurrentNode.Add(name, new JSONClass() { AsVector3 = value });
        }

        public void SerializeQuaternion(string name, Quaternion value)
        {
            CurrentNode.Add(name, new JSONClass() { AsQuaternion = value });
        }

        public void SerializeDouble(string name, double value)
        {
            CurrentNode.Add(name, new JSONData(value));
        }

        public void SerializeFloat(string name, float value)
        {
            CurrentNode.Add(name, new JSONData(value));
        }

        public void SerializeColor(string name, Color value)
        {
            var node = new JSONClass();
            node.Add("r", new JSONData(value.r));
            node.Add("g", new JSONData(value.g));
            node.Add("b", new JSONData(value.b));
            node.Add("a", new JSONData(value.a));
            CurrentNode.Add(name, node.AsObject);
        }

        public void SerializeBytes(string name, byte[] bytes)
        {
            //        throw new NotImplementedException();
        }

        public IEnumerable<T> DeserializeObjectArray<T>(string name)
        {
            Push(name, CurrentNode[name]);
            foreach (var jsonNode in CurrentNode.Childs)
            {
                if (typeof(T) == typeof(string))
                {
                    yield return (T)(object)DeserializeString(name);
                }
                else
                    if (typeof(T) == typeof(int))
                    {
                        yield return (T)(object)DeserializeInt(name);
                    }
                    else
                        if (typeof(T) == typeof(bool))
                        {
                            yield return (T)(object)DeserializeBool(name);
                        }
                        else
                            if (typeof(T) == typeof(byte[]))
                            {
                                yield return (T)(object)DeserializeBytes(name);
                            }
                            else
                                if (typeof(T) == typeof(double))
                                {
                                    yield return (T)(object)DeserializeDouble(name);
                                }
                                else
                                    if (typeof(T) == typeof(float))
                                    {
                                        yield return (T)(object)DeserializeFloat(name);
                                    }
                                    else
                                        if (typeof(T) == typeof(Quaternion))
                                        {
                                            yield return (T)(object)DeserializeQuaternion(name);
                                        }
                                        else
                                            if (typeof(T) == typeof(Vector2))
                                            {
                                                yield return (T)(object)DeserializeVector2(name);
                                            }
                                            else if (typeof(T) == typeof(Vector3))
                                            {
                                                yield return (T)(object)DeserializeInt(name);
                                            }
                                            else
                                            {
                                                Push(null, jsonNode);
                                                yield return (T)DeserializeObjectFromCurrent();
                                                Pop();
                                            }
            }
            Pop();
        }

        public T DeserializeObject<T>(string name)
        {
            return (T)DeserializeObject(name);
        }


        public object DeserializeObject(string name)
        {
            Push(name, CurrentNode[name]);
            var result = DeserializeObjectFromCurrent();
            Pop();
            return result;
        }

       

        private object DeserializeObjectFromCurrent()
        {
            var identifier = CurrentNode["Identifier"].Value;

            if (UseReferences && ReferenceObjects.ContainsKey(identifier))
            {
                return ReferenceObjects[identifier];
            }
            if (CurrentNode["CLRType"] == null) return null;
            var clrType = CurrentNode["CLRType"].Value;
            var instance = TypeResolver.CreateInstance(clrType, identifier);
            var ufSerializable = instance as IUFSerializable;
            if (ufSerializable != null)
            {
                if (UseReferences)
                    ReferenceObjects.Add(identifier, ufSerializable);
                ufSerializable.Read(this);
            }
            return instance;
        }


        public int DeserializeInt(string name)
        {
            return CurrentNode[name].AsInt;
        }

        public bool DeserializeBool(string name)
        {
            return CurrentNode[name].AsBool;
        }

        public string DeserializeString(string name)
        {
            return CurrentNode[name].Value;
        }

        public Color DeserializeColor(string name)
        {
            return new Color(
                CurrentNode[name]["r"].AsFloat,
                CurrentNode[name]["g"].AsFloat,
                CurrentNode[name]["b"].AsFloat,
                CurrentNode[name]["a"].AsFloat
                );
        }

        public Vector2 DeserializeVector2(string name)
        {
            return CurrentNode[name].AsVector3;
        }

        public Vector3 DeserializeVector3(string name)
        {
            return CurrentNode[name].AsVector3;
        }

        public Quaternion DeserializeQuaternion(string name)
        {
            return CurrentNode[name].AsQuaternion;
        }

        public double DeserializeDouble(string name)
        {
            return CurrentNode[name].AsDouble;
        }

        public float DeserializeFloat(string name)
        {
            return CurrentNode[name].AsFloat;
        }

        public byte[] DeserializeBytes(string name)
        {
            return null;
        }

        public void Load(byte[] readAllBytes)
        {
            var json = Encoding.UTF8.GetString(readAllBytes, 0, readAllBytes.Length);
            RootNode = JSON.Parse(json);
        }

        public byte[] Save()
        {
            return System.Text.Encoding.UTF8.GetBytes(CurrentNode.ToString());
        }


        //public T DeserializeViewModel<T>(string name) where T : ViewModel
        //{
        //    var controller = uFrameKernel.Container.Resolve<Controller>(typeof(T).Name);
        //    if (controller == null)
        //    {
        //        throw new Exception(
        //            "Controller could not be found.  Make sure your subsystem loader has been attached to the kernel.");

        //    }
        //    var instance = controller.Create(Guid.NewGuid().ToString()) as T;

        //    Push(name, CurrentNode[name]);
        //        DeserializeViewModelFromCurrent(instance);
        //    Pop();
        //    return instance;

        //}

        //private T DeserializeViewModelFromCurrent<T>(T container) where T : ViewModel
        //{
        //    var ufSerializable = container as IUFSerializable;
        //        ufSerializable.Read(this);
        //    return container;
        //}


        //public IEnumerable<T> DeserializeViewModelArray<T>(string name) where T : ViewModel
        //{
        //    Push(name, CurrentNode[name]);
        //    foreach (var jsonNode in CurrentNode.Childs)
        //    {
        //        var controller = uFrameKernel.Container.Resolve<Controller>(typeof(T).Name);
        //        if (controller == null)
        //        {
        //            throw new Exception(
        //                "Controller could not be found.  Make sure your subsystem loader has been attached to the kernel.");

        //        }
        //        var instance = controller.Create(Guid.NewGuid().ToString()) as T;

        //        Push(null, jsonNode);
        //        DeserializeViewModelFromCurrent(instance);
        //        Pop();
        //        yield return instance;
        //    }
        //    Pop();
        //}
    }

}

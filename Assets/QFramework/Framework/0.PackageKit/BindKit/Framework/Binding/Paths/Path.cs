/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BindKit.Binding.Paths
{
    [Serializable]
    public class Path : IEnumerator<IPathNode>
    {
        private readonly object _lock = new object();
        private List<IPathNode> nodes = new List<IPathNode>();
        private PathToken token;

        public Path() : this(null)
        {
        }

        public Path(IPathNode root)
        {
            if (root != null)
                this.Prepend(root);
        }

        public IPathNode this[int index]
        {
            get { return this.nodes[index]; }
        }

        public int Count
        {
            get { return this.nodes.Count; }
        }

        public bool IsStatic { get { return nodes.Exists(n => n.IsStatic); } }

        public List<IPathNode> ToList()
        {
            return new List<IPathNode>(nodes);
        }

        public void Append(IPathNode node)
        {
            this.nodes.Add(node);
        }

        public void Prepend(IPathNode node)
        {
            this.nodes.Insert(0, node);
        }

        public void PrependIndexed(string indexValue)
        {
            this.Prepend(new StringIndexedNode(indexValue));
        }

        public void PrependIndexed(int indexValue)
        {
            this.Prepend(new IntegerIndexedNode(indexValue));
        }

        public void AppendIndexed(string indexValue)
        {
            this.Append(new StringIndexedNode(indexValue));
        }

        public void AppendIndexed(int indexValue)
        {
            this.Append(new IntegerIndexedNode(indexValue));
        }

        public PathToken AsPathToken()
        {
            if (this.token != null)
                return this.token;

            lock (_lock)
            {
                if (this.token != null)
                    return this.token;

                if (this.nodes.Count <= 0)
                    throw new InvalidOperationException("The path node is empty");

                this.token = new PathToken(this, 0);
                return this.token;
            }
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            foreach (var node in this.nodes)
            {
                node.AppendTo(buf);
            }
            return buf.ToString();
        }

        #region IEnumerator<IPathNode> Support
        private int index = -1;
        public IPathNode Current
        {
            get { return this.nodes[index]; }
        }

        object IEnumerator.Current
        {
            get { return this.nodes[index]; }
        }

        public bool MoveNext()
        {
            this.index++;
            return this.index >= 0 && index < this.nodes.Count;
        }

        public void Reset()
        {
            this.index = -1;
        }
        #endregion

        #region IDisposable Support
        private bool disposed = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.nodes.Clear();
                    this.index = -1;
                }
                disposed = true;
            }
        }

        ~Path()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public interface IPathNode
    {
        bool IsStatic { get; }

        void AppendTo(StringBuilder output);
    }

    [Serializable]
    public class MemberNode : IPathNode
    {
        private readonly MemberInfo memberInfo;
        private readonly string name;
        private readonly Type type;
        private readonly bool isStatic;
        public MemberNode(string name) : this(null, name, false)
        {
        }

        public MemberNode(Type type, string name, bool isStatic)
        {
            this.name = name;
            this.type = type;
            this.isStatic = isStatic;
        }

        public MemberNode(MemberInfo memberInfo)
        {
            this.memberInfo = memberInfo;
            this.name = memberInfo.Name;
            this.type = memberInfo.DeclaringType;
            this.isStatic = memberInfo.IsStatic();
        }

        public bool IsStatic { get { return this.isStatic; } }

        public Type Type { get { return this.type; } }

        public string Name { get { return this.name; } }

        public MemberInfo MemberInfo { get { return this.memberInfo; } }

        public void AppendTo(StringBuilder output)
        {
            if (output.Length > 0)
                output.Append(".");
            if (IsStatic)
                output.Append(this.type.FullName).Append(".");
            output.Append(this.Name);
        }

        public override string ToString()
        {
            return "MemberNode:" + (this.Name == null ? "null" : this.Name);
        }
    }

    [Serializable]
    public abstract class IndexedNode : IPathNode
    {
        private object _value;
        public IndexedNode(object value)
        {
            this._value = value;
        }

        public bool IsStatic { get { return false; } }

        public object Value
        {
            get { return this._value; }
            private set { this._value = value; }
        }

        public abstract void AppendTo(StringBuilder output);

        public override string ToString()
        {
            return "IndexedNode:" + (this._value == null ? "null" : this._value.ToString());
        }
    }

    [Serializable]
    public abstract class IndexedNode<T> : IndexedNode, IPathNode
    {
        public IndexedNode(T value) : base(value)
        {
        }

        public new T Value { get { return (T)base.Value; } }
    }

    [Serializable]
    public class StringIndexedNode : IndexedNode<string>
    {
        public StringIndexedNode(string indexValue) : base(indexValue)
        {
        }

        public override void AppendTo(StringBuilder output)
        {
            output.AppendFormat("[\"{0}\"]", this.Value);
        }
    }

    [Serializable]
    public class IntegerIndexedNode : IndexedNode<int>
    {
        public IntegerIndexedNode(int indexValue) : base(indexValue)
        {
        }

        public override void AppendTo(StringBuilder output)
        {
            output.AppendFormat("[{0}]", this.Value);
        }
    }
}

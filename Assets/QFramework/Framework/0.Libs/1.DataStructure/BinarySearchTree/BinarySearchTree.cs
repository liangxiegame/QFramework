/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
    using System;

    public class SCBinarySearchTree<T> : Iteratable<T> where T : IBinarySearchTreeElement
    {
        public enum eNodeType
        {
            kLeft = 0,
            kRight = 1,
            kRoot = 2,
        }

        #region 节点类
        public class Node
        {
            public Node LeftChild;
            public Node RightChild;
            private Node mParent;
            protected eNodeType mNodeType;

            public bool IsLeaf()
            {
                if (LeftChild == null && RightChild == null)
                {
                    return true;
                }
                return false;
            }

            public Node Parent
            {
                get { return mParent; }
            }

            public eNodeType NodeType
            {
                get { return mNodeType; }
            }

            public void SetParent(Node parent, eNodeType nodeType)
            {
                mParent = parent;
                mNodeType = nodeType;
            }

            private T mData;

            public float SortScore
            {
                get { return mData.SortScore; }
            }

            public T Data
            {
                get { return mData; }
            }

            public Node(T data)
            {
                mData = data;
            }
        }

        #endregion

        protected Node m_HeadNode;


        #region 插入
        public void Insert(T[] dataArray)
        {
            if (dataArray == null)
            {
                throw new NullReferenceException("BinarySearchTree Not Support Insert Null Object");
            }

            for (int i = 0; i < dataArray.Length; ++i)
            {
                Insert(dataArray[i]);
            }
        }

        public void Insert(T data)
        {
            if (data == null)
            {
                throw new NullReferenceException("BinarySearchTree Not Support Insert Null Object");
            }

            if (m_HeadNode == null)
            {
                m_HeadNode = new Node(data);
                m_HeadNode.SetParent(null, eNodeType.kRoot);
                return;
            }

            Node newNode = new Node(data);

            float score = newNode.SortScore;

            Node preNode = null;
            Node currentNode = m_HeadNode;
            while (currentNode != null)
            {
                preNode = currentNode;
                if (score < currentNode.SortScore)
                {
                    currentNode = currentNode.LeftChild;
                    if (currentNode == null)
                    {
                        newNode.SetParent(preNode, eNodeType.kLeft);
                        preNode.LeftChild = newNode;
                        break;
                    }
                }
                else
                {
                    currentNode = currentNode.RightChild;
                    if (currentNode == null)
                    {
                        newNode.SetParent(preNode, eNodeType.kRight);
                        preNode.RightChild = newNode;
                        break;
                    }
                }
            }
        }

        #endregion

        #region 查找

        protected Node Find(Node head, T data)
        {
            if (data == null)
            {
                return null;
            }

            float score = data.SortScore;
            Node currentNode = head;
            while (currentNode != null)
            {
                if (data.Equals(currentNode.Data))
                {
                    break;
                }
                if (score < currentNode.SortScore)
                {
                    currentNode = currentNode.LeftChild;
                }
                else
                {
                    currentNode = currentNode.RightChild;
                }
            }
            return currentNode;
        }

        #endregion

        #region 删除

        public void Remove(T data)
        {
            if (data == null)
            {
                return;
            }

            Node currentNode = Find(m_HeadNode, data);

            if (currentNode == null)
            {
                Console.WriteLine("Not Find DeleteNode");
                return;
            }

            #region 左右子树都空直接删除

            if (currentNode.LeftChild == null && currentNode.RightChild == null)
            {
                switch (currentNode.NodeType)
                {
                    case eNodeType.kLeft:
                    {
                        currentNode.Parent.LeftChild = null;
                    }
                        break;
                    case eNodeType.kRight:
                    {
                        currentNode.Parent.RightChild = null;
                    }
                        break;
                    case eNodeType.kRoot:
                    {
                        m_HeadNode = null;
                    }
                        break;
                    default:
                        break;
                }
                return;
            }

            #endregion

            #region 右子树不空，接入原父节点的父节点，并绑定原子节点

            if (currentNode.RightChild != null)
            {
                var rightChild = currentNode.RightChild;
                switch (currentNode.NodeType)
                {
                    case eNodeType.kLeft:
                    {
                        currentNode.Parent.LeftChild = rightChild;
                        rightChild.SetParent(currentNode.Parent, eNodeType.kLeft);
                    }
                        break;
                    case eNodeType.kRight:
                    {
                        currentNode.Parent.RightChild = rightChild;
                        rightChild.SetParent(currentNode.Parent, eNodeType.kRight);
                    }
                        break;
                    case eNodeType.kRoot:
                        m_HeadNode = rightChild;
                        rightChild.SetParent(null, eNodeType.kRoot);
                        break;
                    default:
                        break;
                }
                //左子树的根节点是右子树的最左节点
                Node minLeftNode = GetMinNode(rightChild);

                if (currentNode.LeftChild != null)
                {
                    minLeftNode.LeftChild = currentNode.LeftChild;
                    currentNode.LeftChild.SetParent(minLeftNode, eNodeType.kLeft);
                }

                return;
            }

            #endregion

            #region 左子树不空，接入原父节点的父节点

            var leftNode = currentNode.LeftChild;
            switch (currentNode.NodeType)
            {
                case eNodeType.kLeft:
                    currentNode.Parent.LeftChild = leftNode;
                    leftNode.SetParent(currentNode.Parent, eNodeType.kLeft);
                    break;
                case eNodeType.kRight:
                    currentNode.Parent.RightChild = leftNode;
                    leftNode.SetParent(currentNode.Parent, eNodeType.kRight);
                    break;
                case eNodeType.kRoot:
                    m_HeadNode = leftNode;
                    leftNode.SetParent(null, eNodeType.kRoot);
                    break;
                default:
                    break;
            }

            #endregion
        }

        #endregion

        #region 遍历&访问&迭代器

        public delegate void DataVisitor(T data);

        //遍历 通过队列实现
        public void Accept(DataVisitor visitor)
        {
            if (m_HeadNode == null)
            {
                return;
            }

            PTStack<Node> stack = new PTStack<Node>();
            Node current = m_HeadNode;
            while (current != null || !stack.IsEmpty)
            {
                while (current != null)
                {
                    stack.Push(current);
                    current = current.LeftChild;
                }

                if (!stack.IsEmpty)
                {
                    current = stack.Pop();
                    visitor(current.Data);

                    current = current.RightChild;
                }
            }
        }

        public Iterator<T> Iterator()
        {
            return new BinarySearchTreeIterator(m_HeadNode);
        }

        public class BinarySearchTreeIterator : Iterator<T>
        {
            private Node m_HeadNode;
            private Node m_Current;
            PTStack<Node> m_Stack = new PTStack<Node>();

            public BinarySearchTreeIterator(Node headNode)
            {
                m_HeadNode = headNode;
                m_Current = m_HeadNode;
            }

            public bool HasNext
            {
                get
                {
                    if (m_Current != null || !m_Stack.IsEmpty)
                    {
                        return true;
                    }
                    return false;
                }
            }

            public T Next
            {
                get
                {
                    while (m_Current != null)
                    {
                        m_Stack.Push(m_Current);
                        m_Current = m_Current.LeftChild;
                    }

                    if (!m_Stack.IsEmpty)
                    {
                        m_Current = m_Stack.Pop();
                        T result = m_Current.Data;
                        m_Current = m_Current.RightChild;
                        return result;
                    }
                    return default(T);
                }
            }
        }

        protected Node GetMinNode(Node head)
        {
            Node current = head;
            while (current.LeftChild != null)
            {
                current = current.LeftChild;
            }
            return current;
        }

        #endregion
    }
}
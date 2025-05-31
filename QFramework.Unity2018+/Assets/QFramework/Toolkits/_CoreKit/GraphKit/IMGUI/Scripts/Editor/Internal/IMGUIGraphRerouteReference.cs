/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEngine;

namespace QFramework.Internal
{
    public struct GUIGraphRerouteReference
    {
        public GUIGraphNodePort port;
        public int connectionIndex;
        public int pointIndex;

        public GUIGraphRerouteReference(GUIGraphNodePort port, int connectionIndex, int pointIndex)
        {
            this.port = port;
            this.connectionIndex = connectionIndex;
            this.pointIndex = pointIndex;
        }

        public void InsertPoint(Vector2 pos)
        {
            port.GetReroutePoints(connectionIndex).Insert(pointIndex, pos);
        }

        public void SetPoint(Vector2 pos)
        {
            port.GetReroutePoints(connectionIndex)[pointIndex] = pos;
        }

        public void RemovePoint()
        {
            port.GetReroutePoints(connectionIndex).RemoveAt(pointIndex);
        }

        public Vector2 GetPoint()
        {
            return port.GetReroutePoints(connectionIndex)[pointIndex];
        }
    }
}
#endif
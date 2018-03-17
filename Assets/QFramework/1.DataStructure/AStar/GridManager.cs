/****************************************************************************
 * Copyright (c) 2017 liangxie
 * reference blog:http://blog.csdn.net/aisajiajiao/article/details/17622063
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
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// 类处理所有代表地图的格子的属性
    /// </summary>
    public class GridManager : QMonoSingleton<GridManager>
    {
        public int NumOfRows;
        public int NumOfColumns;
        public float GridCellSize;
        public bool ShowGrid = true;
        public bool ShowObstacleBlocks = true;
        private Vector3 mOrigin = new Vector3();
        private GameObject[] mObstacleList;
        public AStarNode[,] Nodes { get; set; }

        public Vector3 Origin
        {
            get { return mOrigin; }
        }

        void Awake()
        {
            mObstacleList = GameObject.FindGameObjectsWithTag("Obstacle");
            CalculateObstacles();
        }

        // Find all the obstacles on the map  
        void CalculateObstacles()
        {
            Nodes = new AStarNode[NumOfColumns, NumOfRows];
            int index = 0;
            for (int i = 0; i < NumOfColumns; i++)
            {
                for (int j = 0; j < NumOfRows; j++)
                {
                    Vector3 cellPos = GetGridCellCenter(index);
                    AStarNode node = new AStarNode(cellPos);
                    Nodes[i, j] = node;
                    index++;
                }
            }
            if (mObstacleList != null && mObstacleList.Length > 0)
            {
                //For each obstacle found on the map, record it in our list  
                foreach (GameObject data in mObstacleList)
                {
                    int indexCell = GetGridIndex(data.transform.position);
                    int col = GetColumn(indexCell);
                    int row = GetRow(indexCell);
                    Nodes[row, col].MarkAsObstacle();
                }
            }
        }

        public int GetGridIndex(Vector3 pos)
        {
            if (!IsInBounds(pos))
            {
                return -1;
            }
            pos -= Origin;
            int col = (int) (pos.x / GridCellSize);
            int row = (int) (pos.z / GridCellSize);
            return (row * NumOfColumns + col);
        }

        public bool IsInBounds(Vector3 pos)
        {
            float width = NumOfColumns * GridCellSize;
            float height = NumOfRows * GridCellSize;
            return (pos.x >= Origin.x && pos.x <= Origin.x + width &&
                    pos.x <= Origin.z + height && pos.z >= Origin.z);
        }

        public Vector3 GetGridCellCenter(int index)
        {
            Vector3 cellPosition = GetGridCellPosition(index);
            cellPosition.x += (GridCellSize / 2.0f);
            cellPosition.z += (GridCellSize / 2.0f);
            return cellPosition;
        }

        public Vector3 GetGridCellPosition(int index)
        {
            int row = GetRow(index);
            int col = GetColumn(index);
            float xPosInGrid = col * GridCellSize;
            float zPosInGrid = row * GridCellSize;
            return Origin + new Vector3(xPosInGrid, 0.0f, zPosInGrid);
        }

        public int GetRow(int index)
        {
            int row = index / NumOfColumns;
            return row;
        }

        public int GetColumn(int index)
        {
            int col = index % NumOfColumns;
            return col;
        }


        public void GetNeighbours(AStarNode node, List<AStarNode> neighbors)
        {
            Vector3 neighborPos = node.Position;
            int neighborIndex = GetGridIndex(neighborPos);
            int row = GetRow(neighborIndex);
            int column = GetColumn(neighborIndex);
            //Bottom  
            int leftNodeRow = row - 1;
            int leftNodeColumn = column;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
            //Top  
            leftNodeRow = row + 1;
            leftNodeColumn = column;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
            //Right  
            leftNodeRow = row;
            leftNodeColumn = column + 1;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
            //Left  
            leftNodeRow = row;
            leftNodeColumn = column - 1;
            AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
        }

        void AssignNeighbour(int row, int column, List<AStarNode> neighbors)
        {
            if (row != -1 && column != -1 &&
                row < NumOfRows && column < NumOfColumns)
            {
                AStarNode nodeToAdd = Nodes[row, column];
                if (!nodeToAdd.IsObstacle)
                {
                    neighbors.Add(nodeToAdd);
                }
            }
        }
        
        void OnDrawGizmos() {  
            if (ShowGrid) {  
                DebugDrawGrid(transform.position, NumOfRows, NumOfColumns,   
                    GridCellSize, Color.blue);  
            }  
            Gizmos.DrawSphere(transform.position, 0.5f);  
            if (ShowObstacleBlocks) {  
                Vector3 cellSize = new Vector3(GridCellSize, 1.0f,  
                    GridCellSize);  
                if (mObstacleList != null && mObstacleList.Length > 0) {  
                    foreach (GameObject data in mObstacleList) {  
                        Gizmos.DrawCube(GetGridCellCenter(  
                            GetGridIndex(data.transform.position)), cellSize);  
                    }  
                }  
            }  
        }  
        public void DebugDrawGrid(Vector3 origin, int numRows, int  
            numCols,float cellSize, Color color) {  
            float width = (numCols * cellSize);  
            float height = (numRows * cellSize);  
            // Draw the horizontal grid lines  
            for (int i = 0; i < numRows + 1; i++) {  
                Vector3 startPos = origin + i * cellSize * new Vector3(0.0f,  
                                       0.0f, 1.0f);  
                Vector3 endPos = startPos + width * new Vector3(1.0f, 0.0f,  
                                     0.0f);  
                Debug.DrawLine(startPos, endPos, color);  
            }  
            // Draw the vertial grid lines  
            for (int i = 0; i < numCols + 1; i++) {  
                Vector3 startPos = origin + i * cellSize * new Vector3(1.0f,  
                                       0.0f, 0.0f);  
                Vector3 endPos = startPos + height * new Vector3(0.0f, 0.0f,  
                                     1.0f);  
                Debug.DrawLine(startPos, endPos, color);  
            }  
        }  
    }
}
using QFramework;
using System.Text;
using UnityEngine;

namespace SnakeGame
{
    public class Node
    {
        public enum E_Type { Block, Wall, Food, Snake }
        public E_Type type;
        public override string ToString() => $"[{type.ToString()}]";
    }
    public interface IGridNodeSystem : ISystem
    {
        void CreateGrid(int w, int h);
        Node GetNode(int x, int y);
        Vector2Int FindBlockPos(int w, int h);
    }
    public class GridNodeSystem : AbstractSystem, IGridNodeSystem
    {
        private Node[,] mNodes;
        Node IGridNodeSystem.GetNode(int x, int y) => mNodes[x, y];
        void IGridNodeSystem.CreateGrid(int w, int h)
        {
            if (mNodes == null || mNodes.GetLength(0) * mNodes.GetLength(1) < w * h) mNodes = new Node[w, h];
            var e = new CreateGridEvent();
            for (int row = 0; row < w; row++)
            {
                for (int col = 0; col < h; col++)
                {
                    if (mNodes[row, col] == null) mNodes[row, col] = new Node();
                    mNodes[row, col].type = row == 0 || row == w - 1 || col == 0 || col == h - 1 ? Node.E_Type.Wall : Node.E_Type.Block;
                    e.type = mNodes[row, col].type;
                    e.pos = new Vector2(col, row);
                    this.SendEvent(e);
                }
            }
        }
        protected override void OnInit()
        {
            this.RegisterEvent<SnakeBiggerEvent>(OnSnakeBigger);
            this.RegisterEvent<SnakePosUpdateEvent>(OnSnakePosUpdate);
            this.RegisterEvent<CreateFoodEvent>(OnFoodCreated);
        }
        private void OnSnakePosUpdate(SnakePosUpdateEvent e)
        {
            Node node = mNodes[(int)e.head.y, (int)e.head.x];
            switch (node.type)
            {
                case Node.E_Type.Snake:
                case Node.E_Type.Wall:
                    this.SendEvent<GameOverEvent>();
                    Debug.Log(node.type);
                    break;
                default:
                    if (node.type == Node.E_Type.Food)
                    {
                        this.SendEvent(new EatFoodEvent() { x = (int)e.last.y, y = (int)e.last.x });
                        this.SendEvent(new CreateFoodEvent() { pos = FindBlockPos(mNodes.GetLength(0), mNodes.GetLength(1)) });
                    }
                    node.type = Node.E_Type.Snake;
                    mNodes[(int)e.last.y, (int)e.last.x].type = Node.E_Type.Block;

                    break;
            }

        }
        private void OnFoodCreated(CreateFoodEvent e) => mNodes[e.pos.x, e.pos.y].type = Node.E_Type.Food;
        private void OnSnakeBigger(SnakeBiggerEvent e) => mNodes[e.x, e.y].type = Node.E_Type.Snake;
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            int rowCount = mNodes.GetLength(0);
            int colCount = mNodes.GetLength(1);
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    builder.Append(mNodes[row, col].ToString());
                    if (colCount - 1 != col) builder.Append(",");
                }
                builder.Append("\n");
            }
            return builder.ToString();
        }
        public Vector2Int FindBlockPos(int w, int h)
        {
            Node node;
            int x, y;
            do
            {
                x = UnityEngine.Random.Range(1, w - 1);
                y = UnityEngine.Random.Range(1, h - 1);
                node = mNodes[x, y];
            }
            while (node.type != Node.E_Type.Block);
            return new Vector2Int(x, y);
        }
    }
}
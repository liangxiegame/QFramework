using UniRx;
using UnityEngine;

namespace QFramework.ILKitDemo.Tetris
{
    public interface IGameModel : ILModel
    {
        bool isDataUpdate { get; set; }

        IntReactiveProperty Score { get; }
        IntReactiveProperty HighScore { get; }
        IntReactiveProperty NumbersGame { get; }
        bool IsGameOver();

        void Load();
        void Save();
        void Restart();
    }

    public class GameModel : ILModel<Tetris>, IGameModel
    {
        public const int NORMAL_ROWS = 20;
        public const int MAX_ROWS = 23;
        public const int MAX_COLUMNS = 10;

        private Transform[,] map = new Transform[MAX_COLUMNS, MAX_ROWS];
        

        public IntReactiveProperty Score { get;} = new IntReactiveProperty();

        public IntReactiveProperty HighScore => new IntReactiveProperty();

        public IntReactiveProperty NumbersGame => new IntReactiveProperty();

        public bool IsValidMapPosition(Transform t)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var child = t.GetChild(i);

                if (child.tag != "Block") continue;
                Vector2 pos = child.position.Round();
                if (IsInsideMap(pos) == false) return false;
                if (map[(int) pos.x, (int) pos.y] != null) return false;
            }

            return true;
        }

        public bool IsGameOver()
        {
            for (int i = NORMAL_ROWS; i < MAX_ROWS; i++)
            {
                for (int j = 0; j < MAX_COLUMNS; j++)
                {
                    if (map[j, i] != null)
                    {
                        NumbersGame.Value++;
                        return true;
                    }
                }
            }

            return false;
        }

        public void Load()
        {
            var storage = GetUtility<IStorage>();
            HighScore.Value = storage.LoadInt("HighScore", 0);
            NumbersGame.Value = storage.LoadInt("NumbersGame", 0);
        }

        public void Save()
        {
            var storage = GetUtility<IStorage>();
            storage.SaveInt("HighScore", HighScore.Value);
            storage.SaveInt("NumbersGame", NumbersGame.Value);
        }

        private bool IsInsideMap(Vector2 pos)
        {
            return pos.x >= 0 && pos.x < MAX_COLUMNS && pos.y >= 0;
        }

        public bool PlaceShape(Transform t)
        {
            foreach (Transform child in t)
            {
                if (child.tag != "Block") continue;
                Vector2 pos = child.position.Round();
                map[(int) pos.x, (int) pos.y] = child;
            }

            return CheckMap();
        }

        //检查地图是否不要消除行
        private bool CheckMap()
        {
            int count = 0;
            for (int i = 0; i < MAX_ROWS; i++)
            {
                bool isFull = CheckIsRowFull(i);
                if (isFull)
                {
                    count++;
                    DeleteRow(i);
                    MoveDownRowsAbove(i + 1);
                    i--;
                }
            }

            if (count > 0)
            {
                Score.Value += count * 100;
                if (Score.Value > HighScore.Value)
                {
                    HighScore.Value = Score.Value;
                }

                isDataUpdate = true;
                return true;
            }
            else return false;
        }

        private bool CheckIsRowFull(int row)
        {
            for (int i = 0; i < MAX_COLUMNS; i++)
            {
                if (map[i, row] == null) return false;
            }

            return true;
        }

        private void DeleteRow(int row)
        {
            for (int i = 0; i < MAX_COLUMNS; i++)
            {
                Object.Destroy(map[i, row].gameObject);
                map[i, row] = null;
            }
        }

        private void MoveDownRowsAbove(int row)
        {
            for (int i = row; i < MAX_ROWS; i++)
            {
                MoveDownRow(i);
            }
        }

        private void MoveDownRow(int row)
        {
            for (int i = 0; i < MAX_COLUMNS; i++)
            {
                if (map[i, row] != null)
                {
                    map[i, row - 1] = map[i, row];
                    map[i, row] = null;
                    map[i, row - 1].position += new Vector3(0, -1, 0);
                }
            }
        }


        public void Restart()
        {
            for (int i = 0; i < MAX_COLUMNS; i++)
            {
                for (int j = 0; j < MAX_ROWS; j++)
                {
                    if (map[i, j] != null)
                    {
                        Object.Destroy(map[i, j].gameObject);
                        map[i, j] = null;
                    }
                }
            }

            Score.Value = 0;
        }

        public void ClearData()
        {
            Score.Value = 0;
            HighScore.Value = 0;
            NumbersGame.Value = 0;
        }

        public bool isDataUpdate { get; set; }
    }
}
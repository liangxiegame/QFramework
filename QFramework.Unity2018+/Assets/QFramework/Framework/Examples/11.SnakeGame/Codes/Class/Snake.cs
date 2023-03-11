using UnityEngine;
using PnFramework;

namespace SnakeGame
{
    public interface ISnake
    {
        int HeadIndex { get; }
        int TailIndex { get; }

        Vector3 NextMoveDir { get; }

        void GetMoveDir(int hor, int ver);
        void Bigger(int index);
        void Move();
    }
    public class Snake : ISnake
    {
        private VernierArray<int> mBodys;
        private Vector3 mNextMoveDir = Vector3.right;

        public Snake() => mBodys = new VernierArray<int>();

        Vector3 ISnake.NextMoveDir => mNextMoveDir;

        int ISnake.HeadIndex => mBodys.GetFirst();
        int ISnake.TailIndex => mBodys.GetLast();

        void ISnake.Move()
        {
            mBodys.LoopPos();
            Debug.Log(mBodys.ToString());
        }
        void ISnake.Bigger(int index) => mBodys.AddLast(index);
        void ISnake.GetMoveDir(int hor, int ver)
        {
            if (mBodys.Count > 1)
            {
                if (mNextMoveDir.x != 0 && mNextMoveDir.y == 0 && ver != 0)
                {
                    mNextMoveDir = Vector3.up * ver;
                }
                else if (mNextMoveDir.y != 0 && mNextMoveDir.x == 0 && hor != 0)
                {
                    mNextMoveDir = Vector3.right * hor;
                }
            }
            else if (hor != 0) mNextMoveDir = Vector3.right * hor;
            else if (ver != 0) mNextMoveDir = Vector3.up * ver;
        }
    }
}
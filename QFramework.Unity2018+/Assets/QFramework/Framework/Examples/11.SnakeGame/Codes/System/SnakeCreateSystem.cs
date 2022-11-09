using QFramework;
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public interface ISnakeCreateSystem : ISystem
    {
    }
    public class SnakeCreateSystem : AbstractSystem, ISnakeCreateSystem
    {
        private List<SpriteRenderer> SnakeBodys;
        private SnakePosUpdateEvent mSnakePosUpdateEvent;
        private Quaternion lastRotation;
        private Sprite mSnakeSprite;

        protected override void OnInit()
        {
            SnakeBodys = new List<SpriteRenderer>(4);
            mSnakeSprite = Resources.Load<Sprite>("Sprites/Triangle");

            this.RegisterEvent<SnakeMoveEvent>(OnSnakeMove);
            this.RegisterEvent<SnakeBiggerEvent>(OnSnakeBigger);
        }
        private void OnSnakeMove(SnakeMoveEvent e)
        {
            var head = SnakeBodys[e.lastIndex];
            var next = SnakeBodys[e.headIndex];
            float angle = GetMoveAngel(e.nextMove);
            if (SnakeBodys.Count > 1)
            {
                next.transform.rotation = Quaternion.Euler(0, 0, angle);
                next.color = Color.cyan;
            }
            lastRotation = head.transform.rotation;
            head.transform.rotation = Quaternion.Euler(0, 0, angle);
            head.color = Color.red;

            mSnakePosUpdateEvent.last = head.transform.position;
            head.transform.localPosition = mSnakePosUpdateEvent.head = next.transform.position + e.nextMove;

            this.SendEvent(mSnakePosUpdateEvent);
        }
        private void OnSnakeBigger(SnakeBiggerEvent e)
        {
            var snake = new GameObject("Snake").AddComponent<SpriteRenderer>();
            snake.sprite = mSnakeSprite;
            snake.sortingOrder = 2;
            snake.color = SnakeBodys.Count > 0 ? Color.cyan : Color.red;
            snake.transform.rotation = SnakeBodys.Count > 0 ? lastRotation : Quaternion.Euler(0, 0, GetMoveAngel(e.dir));
            snake.transform.localPosition = new Vector2(e.y, e.x);
            SnakeBodys.Add(snake);
        }
        /// <summary>
        /// 根据输入方向获取身体节点的旋转角度
        /// </summary>
        private float GetMoveAngel(Vector3 dir)
        {
            if (dir == Vector3.left) return 90;
            if (dir == Vector3.right) return -90;
            if (dir == Vector3.down) return 180;
            return 0;
        }
    }
}
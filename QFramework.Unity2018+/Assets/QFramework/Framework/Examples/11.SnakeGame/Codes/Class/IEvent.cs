using UnityEngine;

namespace SnakeGame
{
    public interface IEvent { }
    public struct EatFoodEvent
    {
        public int x, y;
    }
    public struct SnakeBiggerEvent
    {
        public int x, y;
        public Vector3 dir;
    }
    public struct SnakeMoveEvent
    {
        public int headIndex;
        public int lastIndex;
        public Vector3 nextMove;
    }
    public struct SnakePosUpdateEvent
    {
        public Vector2 head;
        public Vector2 last;
    }
    public struct CreateGridEvent
    {
        public Node.E_Type type;
        public Vector2 pos;
    }
    public struct CreateFoodEvent
    {
        public Vector2Int pos;
    }
    internal struct DirInputEvent
    {
        public int hor, ver;
    }
    public struct GameOverEvent { }
    public struct GameInitEndEvent { }
}
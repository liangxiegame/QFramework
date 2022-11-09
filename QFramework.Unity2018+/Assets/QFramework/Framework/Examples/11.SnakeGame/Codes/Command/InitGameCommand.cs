using QFramework;
using UnityEngine;

namespace SnakeGame
{
    public class InitGameCommand : AbstractCommand
    {
        private readonly int mapW, mapH;

        public InitGameCommand(int w, int h)
        {
            mapW = w;
            mapH = h;
        }
        protected override void OnExecute()
        {
            CenterCamera(mapW, mapH);
            var map = this.GetSystem<IGridNodeSystem>();
            map.CreateGrid(mapW, mapH);
            var p = map.FindBlockPos(mapW, mapH);
            this.GetSystem<ISnakeSystem>().CreateSnake(p.x, p.y);
            this.SendEvent(new CreateFoodEvent() { pos = map.FindBlockPos(mapW, mapH) });
            this.SendEvent<GameInitEndEvent>();
        }
        /// <summary>
        /// 居中摄像机
        /// </summary>
        private void CenterCamera(int w, int h)
        {
            Camera.main.transform.localPosition = new Vector3((w - 1) * 0.5f, (h - 1) * 0.5f, -10f);
            Camera.main.orthographicSize = w > h ? w * 0.5f : h * 0.5f;
        }
    }
}
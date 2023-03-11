using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace SnakeGame
{
    public interface IGridCreateSystem : ISystem {}
    public class GridCreateSystem : AbstractSystem, IGridCreateSystem
    {
        private List<SpriteRenderer> renders;
        private Transform MapRoot;
        private Sprite mWallSprite;
        private int mBlockIndex = 0;
    
        private Transform mFoodTrans;

        protected override void OnInit()
        {
            mWallSprite = Resources.Load<Sprite>("Sprites/Block");
            MapRoot = new GameObject("GameMap").transform;
            renders = new List<SpriteRenderer>(16);

            var foodRender = new GameObject("Food").AddComponent<SpriteRenderer>();
            foodRender.sprite = Resources.Load<Sprite>("Sprites/Circle");
            foodRender.color = Color.yellow;
            foodRender.sortingOrder = 1;
            mFoodTrans = foodRender.transform;

            this.RegisterEvent<CreateGridEvent>(OnGridCreated);            
            this.RegisterEvent<CreateFoodEvent>(OnFoodCreated);           
            this.RegisterEvent<GameInitEndEvent>(OnGameInitEnd);
        }
        private void OnFoodCreated(CreateFoodEvent e) => mFoodTrans.localPosition = new Vector2(e.pos.y, e.pos.x);
        private void OnGameInitEnd(GameInitEndEvent e) => mBlockIndex = 0;
        private void OnGridCreated(CreateGridEvent e)
        {
            if (mBlockIndex == renders.Count) renders.Add(new GameObject(e.type.ToString()).AddComponent<SpriteRenderer>());
            renders[mBlockIndex].color = e.type == Node.E_Type.Wall ? Color.black : Color.gray;
            renders[mBlockIndex].transform.localPosition = e.pos;
            renders[mBlockIndex].transform.SetParent(MapRoot);
            renders[mBlockIndex].sprite = mWallSprite;
            mBlockIndex++;
        }
    }
}
using UnityEngine;
using QFramework;

namespace SnakeGame
{
    //暂时表现为点击开始按钮的效果
    public class GameStart : MonoBehaviour, IController
    {
        public void Start() => this.SendCommand(new InitGameCommand(20, 20));
        IArchitecture IBelongToArchitecture.GetArchitecture() => SnakeGame.Interface;
    }
}
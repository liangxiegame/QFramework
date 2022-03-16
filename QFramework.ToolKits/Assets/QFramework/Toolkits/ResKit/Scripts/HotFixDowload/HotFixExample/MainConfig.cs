///****************************************************
//    文件：MainConfig.cs
//	作者：汪泽泽
//    邮箱: 1178092718@qq.com
//    日期：2020/1/11 16:27:34
//	功能：Nothing
//*****************************************************/

//using System.CodeDom;
//using UnityEngine;

//namespace MyGame
//{
//    public class MainConfig :MonoBehaviour
//    {
//        Transform common;
//        public CommonConfirm commonConfirm;
//        public HotFixedPanel hotFixedPanel;

//        public static MainConfig Instance;

//        private void Awake()
//        {
//            Instance = this;
//        }

//        public void Start()
//        {
//            common = GameObject.Find("Common").transform;
//            commonConfirm = common.Find("CommonConfirm").GetComponent<CommonConfirm>();
//            hotFixedPanel = common.Find("HotFixedPanel").GetComponent<HotFixedPanel>();
//            hotFixedPanel.Init();
//            commonConfirm.Init();
//            commonConfirm.Hide();
//        }


//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using QFramework.Example;
using UniRx;
using UnityEngine.UI;
using System;
using UniRx.Examples;
using UniRx.Diagnostics;
using System.Text;

namespace LFramework
{
    /// <summary>
    /// 2018/3/30
    /// </summary>
    public class TestUniRx : MonoBehaviour
    {
        #region 变量
        public static readonly UniRx.Diagnostics.Logger         log = new UniRx.Diagnostics.Logger("L Log");
        CompositeDisposable                                     disposables = new CompositeDisposable();
        IReactiveProperty<bool>                                 isChange = new ReactiveProperty<bool>(false);
        IDisposable                                             CurrentSub;
        #endregion

        #region 测试
        void Start()
        {
            ResMgr.Init();
            UIMgr.OpenPanel<UITestUniRx>();

            Dictionary<string, Action> m_FuncList = new Dictionary<string, Action>();

            m_FuncList.Add("a", DoubleClick);   //双击
            m_FuncList.Add("s", WWW);           //WWW
            m_FuncList.Add("d", Property);      //属性监听
            m_FuncList.Add("f", Coroutine);     //和unity协程
            m_FuncList.Add("g", ReactiveSubscribe);//收集器
            m_FuncList.Add("h", TsfSubscribe);   //transform

            ObservableLogger.Listener
                .Subscribe(_ => Debug.Log(_));

            Observable.EveryUpdate()
                .Do((x) =>
                {
                    if (Input.inputString != string.Empty && Input.inputString != " ")
                    {
                        Debug.Log(Input.inputString);

                        if (m_FuncList.ContainsKey(Input.inputString))
                        {
                            if(CurrentSub!=null)
                            CurrentSub.Dispose();
                            m_FuncList[Input.inputString].InvokeGracefully();
                        }
                    }
                })
                .Subscribe()
                .AddTo(this);

            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Space))
                .Subscribe(_ => { disposables.Dispose(); disposables.Clear(); })
                .AddTo(this);
        }
        #endregion

        #region 双击
        void DoubleClick()
        {
            var stream = Observable.EveryUpdate()
              .Where(x => Input.GetMouseButtonDown(0));

            CurrentSub = stream.Buffer(stream.Throttle(TimeSpan.FromSeconds(0.25f)))
                .Do(x => Debug.Log("在检测"))
             .Where(x => x.Count >= 2)
             .Subscribe(x => Debug.Log("按下次数" + x.Count));
        }
        #endregion

        #region WWW
        void WWW()
        {
            ObservableWWW.GetWWW("http://img.taopic.com/uploads/allimg/120428/128240-12042Q4020849.jpg")
                    .Subscribe(down =>
                    {
                        Texture2D temp2d = down.texture;
                        Debug.Log(temp2d.name);
                        Sprite tempSp = Sprite.Create(temp2d, new Rect(0, 0, temp2d.width, temp2d.height), Vector2.zero);
                        UIMgr.GetPanel<UITestUniRx>().GetComponent<Image>().sprite = tempSp;
                    }
                    , x => Debug.Log("请求错误"))
                    .AddTo(disposables);
        }
        #endregion

        #region 属性
        void Property()
        {
            IReactiveProperty<bool> m_Bool = new ReactiveProperty<bool>(false);
            m_Bool.Subscribe(xs => Debug.Log("值改变"));

            CurrentSub = Observable.EveryUpdate()
                      .Where(_ => Input.GetKeyDown(KeyCode.T))
                      .Subscribe(_ => m_Bool.Value = !m_Bool.Value);
        }
        #endregion

        #region 协程
        void Coroutine()
        {
            Sample10_MainThreadDispatcher temp = new Sample10_MainThreadDispatcher();
//            CurrentSub = temp.Run();
        }
        #endregion

        #region GameObj
        void ReactiveSubscribe()
        {
            disposables = new CompositeDisposable();
            ReactiveCollection<int> m_ReaList = new ReactiveCollection<int>();
            m_ReaList.ObserveCountChanged().Subscribe(x => { Debug.Log(x); }).AddTo(disposables);

            m_ReaList.ObserveAdd().Subscribe(x => { Debug.Log(x); }).AddTo(disposables);
            m_ReaList.ObserveRemove().Subscribe(x => Debug.Log(x)).AddTo(disposables);
            m_ReaList.Add(1);
            m_ReaList.Remove(1);

            CurrentSub = disposables;
        }
        #endregion

        #region Tsf
        void TsfSubscribe()
        {
            CurrentSub = transform.ObserveEveryValueChanged(x => x.position).Subscribe(x => Debug.Log(x));
        }
        #endregion
    }
}

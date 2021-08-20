namespace QFramework.PlatformRunner
{
    public interface IGameModel : IModel
    {
        /// <summary>
        /// 金币
        /// </summary>
        BindableProperty<int> Coin { get; }

        /// <summary>
        /// 最佳成绩
        /// </summary>
        BindableProperty<int> BestMeter { get; }

        /// <summary>
        /// 声音
        /// </summary>
        BindableProperty<int> SoundState { get; }

        /// <summary>
        /// 当前主题（实际上是关卡主题) 1
        /// </summary>
        BindableProperty<int> CurTheme { get; }


        /// <summary>
        /// 当前的关卡
        /// </summary>
        BindableProperty<int> CurLevel { get; }

        /// <summary>
        /// 当前的跑的长度
        /// </summary>
        BindableProperty<int> CurMeter { get; }
    }

    public class GameModel : AbstractModel, IGameModel
    {
        public BindableProperty<int> Coin { get; } = new BindableProperty<int>()
        {
            Value = 0
        };

        public BindableProperty<int> BestMeter { get; } = new BindableProperty<int>()
        {
            Value = 0
        };

        public BindableProperty<int> SoundState { get; } = new BindableProperty<int>()
        {
            Value = 0
        };

        public BindableProperty<int> CurTheme { get; } = new BindableProperty<int>()
        {
            Value = 1
        };

        public BindableProperty<int> CurLevel { get; } = new BindableProperty<int>()
        {
            Value = 0
        };

        public BindableProperty<int> CurMeter { get; } = new BindableProperty<int>()
        {
            Value = 0
        };
        
        protected override void OnInit()
        {
            
        }
        

        // public HeroInfo HeroInfo = new HeroInfo();

        // public GameInfo GameInfo = new GameInfo();

//         	public override IManager Manager
// 		{
// 			get { return GameManager.Instance; }
// 		}
//
// 		protected override void ProcessMsg(int eventKey, QMsg msg)
// 		{
// 			switch (msg.EventID)
// 			{
// 				case (ushort) GameEvent.CoinAdd:
// 					GetCoin += ((MsgWithInt) msg).Value;
// 					break;
// 				case (ushort) GameEvent.EnergyAdd:
// 					Energy += ((MsgWithInt) msg).Value;
//
// 					if (Energy >= 1000)
// 					{
// 						Energy = 1000;
// 					}
//
// 					if (Energy < 0)
// 					{
// 						fsm.HandleEvent("over"); // 游戏结束
// 					}
//
// 					break;
// 				case (ushort) GameEvent.SetTimeScale:
// 					TimeScale = ((MsgWithFloat) msg).Value;
// 					break;
// 				case (ushort) GameEvent.MeterAdd:
// 					AddMeter(((MsgWithInt) msg).Value);
// 					break;
// 				case (ushort) GameEvent.SetMeter:
// 					Meter = ((MsgWithInt) msg).Value;
// 					break;
//
// 			}
// 		}
//
// 		private void Awake()
// 		{
//
// 			RegisterEvent(GameEvent.CoinAdd);
// 			RegisterEvent(GameEvent.EnergyAdd);
// 			RegisterEvent(GameEvent.MeterAdd);
// 			RegisterEvent(GameEvent.SetMeter);
// 			RegisterEvent(GameEvent.SetTimeScale);
//
// 			SimpleEventSystem.GetEvent<GameOverEvent>()
// 				.Subscribe(_ => { fsm.HandleEvent("over"); })
// 				.AddTo(this);
// 		}
//
// 		/// <summary>
// 		/// 相关的数据定义
// 		/// </summary>
//
// 		// 已经跑的距离
// 		int m_nMeter = 0;
//
// 		public int Meter
// 		{
// 			get { return m_nMeter; }
//
// 			set
// 			{
// 				m_nMeter = value;
// //			GameManager.Instance.uiCtrl.UpdateMeter ();
// 			}
// 		}
//
// 		// 时间缩放比例,与Auto道具有关
// 		float m_fTimeScale = 1.0f;
//
// 		public float TimeScale
// 		{
// 			get { return m_fTimeScale; }
// 			set
// 			{
// 				m_fTimeScale = value;
// 				Time.timeScale = value;
// 			}
// 		}
//
// 		// 得到的钱
// 		public int GetCoin = 0;
//
// 		// 体力
// 		public int Energy = 1000; // 暂时1000为满
//
// 		public int Level = 0; // 等级 总共有12个等级 // 每个等级4个Theme
//
// 		public float ShowEnergy
// 		{
// 			// 用来显示的Energy
// 			get { return Energy * 0.001f; }
// 		}
//
// 		public float ShowMeter
// 		{
// 			// 用来显示的Meter
// 			get { return (Meter % 100) * 0.01f; }
// 		}
//
// 		/// <summary>
// 		/// 添加米数
// 		/// </summary>
// 		/// <param name="meter">Meter.</param>
// 		public void AddMeter(int meter)
// 		{
// 			Meter += meter;
//
// 			switch (Meter)
// 			{
// 				case 66:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 90:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 100:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
//
// //			Level = 1;
// 					break;
// 				case 166:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 190:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
//
// 				case 200:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(1, 3), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Day ();
// //			Level = 2;
// 					break;
//
// 				case 266:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 290:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 300:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
// //			Level = 3;
// 					break;
// 				case 366:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 390:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
//
// 				case 400:
// //			GameManager.Instance.bgCtrl.SwitchBegan (2);
// //			Level = 4;
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(1, 3), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Day ();
// 					break;
// 				case 466:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 490:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 500:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
// //			Level = 5;
// 					break;
// 				case 566:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 590:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
// 				case 600:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(1, 3), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Day ();
// //			Level = 6;
// 					break;
// 				case 666:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 690:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 700:
// //			Level = 7;
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
// 					break;
// 				case 766:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 790:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
// 				case 800:
// //			Level = 8;
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(1, 3), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Day ();
// 					break;
// 				case 866:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 890:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 900:
// //			Level = 9;
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
// 					break;
// 				case 966:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 990:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
// 				case 1000:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(1, 3), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Day ();
// //			Level = 10;
// 					break;
// 				case 1066:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 1090:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 1100:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
// //			Level = 11;
// 					break;
// 				case 1166:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 1190:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
// 				case 1200:
//
// 					break;
// 			}
// 		}
//
// 		public QFSMLite fsm = new QFSMLite(); // 状态机
//
// 		/// <summary>
// 		/// 初始化数据
// 		/// </summary>
// 		public void InitData()
// 		{
// 			//临时数据清零
// 			GetCoin = 0;
// 			Meter = 0;
// 			Model.GameModel.State.GameInfo.curTheme = 1;
// 			Energy = 1000;
//
// 			GameManager.Instance.bgCtrl.UpdateView();
// //			GameManager.Instance.playerCtrl.transform.localScale = Vector3.one;
// 			PropModel.Instance.InitModel();
// 		}
// 		
// 			public override IManager Manager
// 		{
// 			get { return GameManager.Instance; }
// 		}
//
// 		protected override void ProcessMsg(int eventKey, QMsg msg)
// 		{
// 			switch (msg.EventID)
// 			{
// 				case (ushort) GameEvent.CoinAdd:
// 					GetCoin += ((MsgWithInt) msg).Value;
// 					break;
// 				case (ushort) GameEvent.EnergyAdd:
// 					Energy += ((MsgWithInt) msg).Value;
//
// 					if (Energy >= 1000)
// 					{
// 						Energy = 1000;
// 					}
//
// 					if (Energy < 0)
// 					{
// 						fsm.HandleEvent("over"); // 游戏结束
// 					}
//
// 					break;
// 				case (ushort) GameEvent.SetTimeScale:
// 					TimeScale = ((MsgWithFloat) msg).Value;
// 					break;
// 				case (ushort) GameEvent.MeterAdd:
// 					AddMeter(((MsgWithInt) msg).Value);
// 					break;
// 				case (ushort) GameEvent.SetMeter:
// 					Meter = ((MsgWithInt) msg).Value;
// 					break;
//
// 			}
// 		}
//
// 		private void Awake()
// 		{
//
// 			RegisterEvent(GameEvent.CoinAdd);
// 			RegisterEvent(GameEvent.EnergyAdd);
// 			RegisterEvent(GameEvent.MeterAdd);
// 			RegisterEvent(GameEvent.SetMeter);
// 			RegisterEvent(GameEvent.SetTimeScale);
//
// 			SimpleEventSystem.GetEvent<GameOverEvent>()
// 				.Subscribe(_ => { fsm.HandleEvent("over"); })
// 				.AddTo(this);
// 		}
//
// 		/// <summary>
// 		/// 相关的数据定义
// 		/// </summary>
//
// 		// 已经跑的距离
// 		int m_nMeter = 0;
//
// 		public int Meter
// 		{
// 			get { return m_nMeter; }
//
// 			set
// 			{
// 				m_nMeter = value;
// //			GameManager.Instance.uiCtrl.UpdateMeter ();
// 			}
// 		}
//
// 		// 时间缩放比例,与Auto道具有关
// 		float m_fTimeScale = 1.0f;
//
// 		public float TimeScale
// 		{
// 			get { return m_fTimeScale; }
// 			set
// 			{
// 				m_fTimeScale = value;
// 				Time.timeScale = value;
// 			}
// 		}
//
// 		// 得到的钱
// 		public int GetCoin = 0;
//
// 		// 体力
// 		public int Energy = 1000; // 暂时1000为满
//
// 		public int Level = 0; // 等级 总共有12个等级 // 每个等级4个Theme
//
// 		public float ShowEnergy
// 		{
// 			// 用来显示的Energy
// 			get { return Energy * 0.001f; }
// 		}
//
// 		public float ShowMeter
// 		{
// 			// 用来显示的Meter
// 			get { return (Meter % 100) * 0.01f; }
// 		}
//
// 		/// <summary>
// 		/// 添加米数
// 		/// </summary>
// 		/// <param name="meter">Meter.</param>
// 		public void AddMeter(int meter)
// 		{
// 			Meter += meter;
//
// 			switch (Meter)
// 			{
// 				case 66:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 90:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 100:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
//
// //			Level = 1;
// 					break;
// 				case 166:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 190:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
//
// 				case 200:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(1, 3), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Day ();
// //			Level = 2;
// 					break;
//
// 				case 266:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 290:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 300:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
// //			Level = 3;
// 					break;
// 				case 366:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 390:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
//
// 				case 400:
// //			GameManager.Instance.bgCtrl.SwitchBegan (2);
// //			Level = 4;
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(1, 3), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Day ();
// 					break;
// 				case 466:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 490:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 500:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
// //			Level = 5;
// 					break;
// 				case 566:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 590:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
// 				case 600:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(1, 3), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Day ();
// //			Level = 6;
// 					break;
// 				case 666:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 690:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 700:
// //			Level = 7;
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
// 					break;
// 				case 766:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 790:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
// 				case 800:
// //			Level = 8;
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(1, 3), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Day ();
// 					break;
// 				case 866:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 890:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 900:
// //			Level = 9;
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
// 					break;
// 				case 966:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 990:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
// 				case 1000:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(1, 3), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Day ();
// //			Level = 10;
// 					break;
// 				case 1066:
// 					GameManager.Instance.bgCtrl.SunAppear();
// 					break;
// 				case 1090:
// 					GameManager.Instance.bgCtrl.SunDied();
// 					break;
// 				case 1100:
// //					GameManager.Instance.StartCoroutine(GameManager.Instance.stageCtrl.SwitchBegan(Random.Range(2, 4), false));
// //			GameManager.Instance.uiCtrl.gameWnd.gameProgress.Night ();
// //			Level = 11;
// 					break;
// 				case 1166:
// 					GameManager.Instance.bgCtrl.MoonAppear();
// 					break;
// 				case 1190:
// 					GameManager.Instance.bgCtrl.MoonDied();
// 					break;
// 				case 1200:
//
// 					break;
// 			}
// 		}
//
// 		public QFSMLite fsm = new QFSMLite(); // 状态机
//
// 		/// <summary>
// 		/// 初始化数据
// 		/// </summary>
// 		public void InitData()
// 		{
// 			//临时数据清零
// 			GetCoin = 0;
// 			Meter = 0;
// 			Model.GameModel.State.GameInfo.curTheme = 1;
// 			Energy = 1000;
//
// 			GameManager.Instance.bgCtrl.UpdateView();
// //			GameManager.Instance.playerCtrl.transform.localScale = Vector3.one;
// 			PropModel.Instance.InitModel();
// 		}

    }
}
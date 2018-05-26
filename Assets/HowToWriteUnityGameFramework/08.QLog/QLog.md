# Unity 游戏框架搭建 (八) 减少加班利器-QLog

#### 为毛要实现这个工具?
在我小时候,每当游戏到了测试阶段,交给QA测试,QA测试了一会儿拿着设备过来说游戏闪退了。。。。当我拿到设备后测了好久Bug也没有复现,排查了好久也没有头绪,就算接了Bugly拿到的也只是闪退的异常信息,或者干脆拿不到。很抓狂,因为这个我是没少加班。所以当时想着解决下这个小小的痛点。。。
#### 现在框架中的QLog:
怎么用呢?在初始化的地方调用这句话就够了。
```csharp
QLog.Instance ();
```
其实做成单例也没有必要。。。。
#### 日志获取方法:
PC端或者Mac端,日志存放在工程的如下位置:
![](/content/images/2016/07/-----2016-07-16---3-54-31.png)
打开之后是这样的:
![](/content/images/2016/07/-----2016-07-16---3-54-44.png)
最后一条信息是触发了一个空指针异常,堆栈信息一目了然。
如果是iOS端,需要使用类似同步推或者iTools工具获取日志文件,路径是这样的:
![](/content/images/2016/07/-----2016-07-16---3-58-28.png)
Android端的话,类似的方式,但是具体路径没查过,不好意思。。。
#### 初版
一开始想做一个保存Debug.Log、Debug.LogWaring、Debug.LogErr信息奥本地文件的小工具。上网一查原来有大神实现了,贴上链接:http://www.xuanyusong.com/archives/2477。
其思路是使用Application.RegisterLocCallback注册回调,每次使用Debug.Log等API时候会触发一次回调,在回调中将Log信息保存在本地。而且意外的发现,Application.RegisterLogCallback也能接收到异常和错误信息。<br>
所以将这份实现作为QLog的初版用了一段时间,发现存在一个问题,如果游戏发生闪退,好多Log信息没来得及存到本地,因为刷入到本地操作是通过Update完成的,每帧之间的间隔,其实很长。
#### 现在的版本:
后来找到了一份实现,思路和初版一样区别是将Update改成线程来刷。Application.RegisterLogCallback这时候已经弃用了,改成了Application.logMessageReceived,后来又找到了Application.logMessageReceivedThreaded。
如果只是使用Application.logMessageReceived的时候,在真机上如果发生Error或者Exception时,收不到堆栈信息。但是使用了Application.logMessageReceivedThreaded就可以接收到堆栈信息了,不过在处理Log信息的时候要保证线程安全。

说明部分就这些吧,实现起来其实没什么难点,主要就是好好利用Application.logMessageReceived和Application.logMessageReceivedThreaded这两个API就好了。

下面贴上我的框架中的实现,这里要注意一下,这份实现依赖于上篇文章介绍的App类(已经重命名为QApp了)。<br><br>
接口类ILogOutput:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QFramework {
	/// <summary>
	/// 日志输出接口
	/// </summary>
	public interface ILogOutput
	{
		/// <summary>
		/// 输出日志数据
		/// </summary>
		/// <param name="logData">日志数据</param>
		void Log(QLog.LogData logData);
		/// <summary>
		/// 关闭
		/// </summary>
		void Close();
	}
}
```
接口实现类 QFileLogOutput
```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;

namespace QFramework {
	/// <summary>
	/// 文本日志输出
	/// </summary>
	public class QFileLogOutput : ILogOutput
	{

		#if UNITY_EDITOR
		string mDevicePersistentPath = Application.dataPath + "/../PersistentPath";
		#elif UNITY_STANDALONE_WIN
		string mDevicePersistentPath = Application.dataPath + "/PersistentPath";
		#elif UNITY_STANDALONE_OSX
		string mDevicePersistentPath = Application.dataPath + "/PersistentPath";
		#else
		string mDevicePersistentPath = Application.persistentDataPath;
		#endif


		static string LogPath = "Log";

		private Queue<QLog.LogData> mWritingLogQueue = null;
		private Queue<QLog.LogData> mWaitingLogQueue = null;
		private object mLogLock = null;
		private Thread mFileLogThread = null;
		private bool mIsRunning = false;
		private StreamWriter mLogWriter = null;

		public QFileLogOutput()
		{
			QApp.Instance().onApplicationQuit += Close;
			this.mWritingLogQueue = new Queue<QLog.LogData>();
			this.mWaitingLogQueue = new Queue<QLog.LogData>();
			this.mLogLock = new object();
			System.DateTime now = System.DateTime.Now;
			string logName = string.Format("Q{0}{1}{2}{3}{4}{5}",
				now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
		string logPath = string.Format("{0}/{1}/{2}.txt", mDevicePersistentPath, LogPath, logName);
			if (File.Exists(logPath))
				File.Delete(logPath);
			string logDir = Path.GetDirectoryName(logPath);
			if (!Directory.Exists(logDir))
				Directory.CreateDirectory(logDir);
			this.mLogWriter = new StreamWriter(logPath);
			this.mLogWriter.AutoFlush = true;
			this.mIsRunning = true;
			this.mFileLogThread = new Thread(new ThreadStart(WriteLog));
			this.mFileLogThread.Start();
		}

		void WriteLog()
		{
			while (this.mIsRunning)
			{
				if (this.mWritingLogQueue.Count == 0)
				{
					lock (this.mLogLock)
					{
						while (this.mWaitingLogQueue.Count == 0)
							Monitor.Wait(this.mLogLock);
						Queue<QLog.LogData> tmpQueue = this.mWritingLogQueue;
						this.mWritingLogQueue = this.mWaitingLogQueue;
						this.mWaitingLogQueue = tmpQueue;
					}
				}
				else
				{
					while (this.mWritingLogQueue.Count > 0)
					{
						QLog.LogData log = this.mWritingLogQueue.Dequeue();
						if (log.Level == QLog.LogLevel.ERROR)
						{
							this.mLogWriter.WriteLine("---------------------------------------------------------------------------------------------------------------------");
							this.mLogWriter.WriteLine(System.DateTime.Now.ToString() + "\t" + log.Log + "\n");
							this.mLogWriter.WriteLine(log.Track);
							this.mLogWriter.WriteLine("---------------------------------------------------------------------------------------------------------------------"); 
						}
						else
						{
							this.mLogWriter.WriteLine(System.DateTime.Now.ToString() + "\t" + log.Log);
						}
					}
				}
			}
		}

		public void Log(QLog.LogData logData)
		{
			lock (this.mLogLock)
			{
				this.mWaitingLogQueue.Enqueue(logData);
				Monitor.Pulse(this.mLogLock);
			}
		}

		public void Close()
		{
			this.mIsRunning = false;
			this.mLogWriter.Close();
		}
	}
}
```
QLog类
```csharp
using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Threading;
namespace  QFramework {
	/// <summary>
	/// 封装日志模块
	/// </summary>
	public class QLog : QSingleton<QLog>
	{
		/// <summary>
		/// 日志等级，为不同输出配置用
		/// </summary>
		public enum LogLevel
		{
			LOG     = 0,
			WARNING = 1,
			ASSERT  = 2,
			ERROR   = 3,
			MAX     = 4,
		}

		/// <summary>
		/// 日志数据类
		/// </summary>
		public class LogData
		{
			public string Log { get; set; }
			public string Track { get; set; }
			public LogLevel Level { get; set; }
		}

		/// <summary>
		/// OnGUI回调
		/// </summary>
		public delegate void OnGUICallback();

		/// <summary>
		/// UI输出日志等级，只要大于等于这个级别的日志，都会输出到屏幕
		/// </summary>
		public LogLevel uiOutputLogLevel = LogLevel.LOG;
		/// <summary>
		/// 文本输出日志等级，只要大于等于这个级别的日志，都会输出到文本
		/// </summary>
		public LogLevel fileOutputLogLevel = LogLevel.MAX;
		/// <summary>
		/// unity日志和日志输出等级的映射
		/// </summary>
		private Dictionary<LogType, LogLevel> logTypeLevelDict = null;
		/// <summary>
		/// OnGUI回调
		/// </summary>
		public OnGUICallback onGUICallback = null;
		/// <summary>
		/// 日志输出列表
		/// </summary>
		private List<ILogOutput> logOutputList = null;
		private int mainThreadID = -1;

		/// <summary>
		/// Unity的Debug.Assert()在发布版本有问题
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="info">输出信息</param>
		public static void Assert(bool condition, string info)
		{
			if (condition)
				return;
			Debug.LogError(info);
		}

		private QLog()
		{
			Application.logMessageReceived += LogCallback;
			Application.logMessageReceivedThreaded += LogMultiThreadCallback;

			this.logTypeLevelDict = new Dictionary<LogType, LogLevel>
			{
				{ LogType.Log, LogLevel.LOG },
				{ LogType.Warning, LogLevel.WARNING },
				{ LogType.Assert, LogLevel.ASSERT },
				{ LogType.Error, LogLevel.ERROR },
				{ LogType.Exception, LogLevel.ERROR },
			};

			this.uiOutputLogLevel = LogLevel.LOG;
			this.fileOutputLogLevel = LogLevel.ERROR;
			this.mainThreadID = Thread.CurrentThread.ManagedThreadId;
			this.logOutputList = new List<ILogOutput>
			{
				new QFileLogOutput(),
			};

			QApp.Instance().onGUI += OnGUI;
			QApp.Instance().onDestroy += OnDestroy;
		}

		void OnGUI()
		{
			if (this.onGUICallback != null)
				this.onGUICallback();
		}

		void OnDestroy()
		{
			Application.logMessageReceived -= LogCallback;
			Application.logMessageReceivedThreaded -= LogMultiThreadCallback;
		}

		/// <summary>
		/// 日志调用回调，主线程和其他线程都会回调这个函数，在其中根据配置输出日志
		/// </summary>
		/// <param name="log">日志</param>
		/// <param name="track">堆栈追踪</param>
		/// <param name="type">日志类型</param>
		void LogCallback(string log, string track, LogType type)
		{
			if (this.mainThreadID == Thread.CurrentThread.ManagedThreadId)
				Output(log, track, type);
		}

		void LogMultiThreadCallback(string log, string track, LogType type)
		{
			if (this.mainThreadID != Thread.CurrentThread.ManagedThreadId)
				Output(log, track, type);
		}

		void Output(string log, string track, LogType type)
		{
			LogLevel level = this.logTypeLevelDict[type];
			LogData logData = new LogData
			{
				Log = log,
				Track = track,
				Level = level,
			};
			for (int i = 0; i < this.logOutputList.Count; ++i)
				this.logOutputList[i].Log(logData);
		}
	}
}
```

#### 欢迎讨论!

#### 相关链接:

[我的框架地址](https://github.com/liangxiegame/QFramework):https://github.com/liangxiegame/QFramework

[教程源码](https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework):https://github.com/liangxiegame/QFramework/tree/master/Assets/HowToWriteUnityGameFramework/

QFramework&游戏框架搭建QQ交流群: 623597263

转载请注明地址:[凉鞋的笔记](http://liangxiegame.com/)http://liangxiegame.com/

微信公众号:liangxiegame

![](https://ws1.sinaimg.cn/large/006tKfTcgy1frot0yq1dsj30by0byt9i.jpg)

### 如果有帮助到您:

如果觉得本篇教程或者 QFramework 对您有帮助，不妨通过以下方式赞助笔者一下，鼓励笔者继续写出更多高质量的教程，也让更多的力量加入 QFramework 。

- 给 QFramework 一个 Star:https://github.com/liangxiegame/QFramework
- 下载 Asset Store 上的 QFramework 给个五星(如果有评论小的真是感激不尽):http://u3d.as/SJ9
- 购买 gitchat 话题并给 5 星好评: http://gitbook.cn/gitchat/activity/5abc3f43bad4f418fb78ab77 (6 元，会员免费)
- 购买同名的蛮牛视频课程并给 5 星好评:http://edu.manew.com/course/431 (目前定价 29.8 元)
- 购买同名电子书 :https://www.kancloud.cn/liangxiegame/unity_framework_design( 29.9 元，内容会在 2018 年 10 月份完结)

笔者在这里保证 QFramework、入门教程、文档和此框架搭建系列的专栏永远免费开源。以上捐助产品的内容对于使用 QFramework 的使用来讲都不是必须的，所以大家不用担心，各位使用 QFramework 或者 阅读此专栏 已经是对笔者团队最大的支持了。
using UniRx;
using UnityEngine;

namespace QFramework.Example
{
	/// <summary>
	/// MVCWithBindablePropertyExample.Designer.cs 是 View 层
	/// MVCWithBindablePropertyExample.cs 是 Controller 层
	/// </summary>
	public partial class MVCWithBindablePropertyExample
	{
		CountModel mModel = new CountModel();

		void OnStart()
		{
			// 绑定 Count
			mModel.Count.BindWithInitialValue(count => Text.text = count.ToString()).AddTo(gameObject);

			// 按钮点击事件
			Button.OnClickAsObservable()
				.Subscribe(_ => { mModel.Count.Value++; });
		}

		void OnDestroy()
		{
			mModel = null;
		}
	}

	public class CountModel
	{
		public readonly Property<int> Count;

		public CountModel()
		{
			var count = PlayerPrefs.GetInt("COUNT", 0);
			Count = new Property<int>(count);
			Count.Bind(value => PlayerPrefs.SetInt("COUNT", value));
		}
	}
}

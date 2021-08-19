
using UniRx;

namespace QFramework.Example
{
	public partial class BindablePropertyExample
	{
		/// <summary>
		/// 声明一个 Count 属性
		/// </summary>
		Property<int> mCount = new Property<int>(0);
		
		void OnStart()
		{
			// Code Here
			
			// Count 与 Text 绑定
			// 当 Count 的值有变更时，Text 自动更新
			mCount.BindWithInitialValue(count=> Text.text = count.ToString()).AddTo(gameObject);

			// mCount.BindWithInitialValue() : 默认会把 mCount 的初始值设置给  Text.text
			// mCount.Bind()  不会把默认值设置给  Text.text，只有值变更时会设置给 Text.text
			// mCount.OnValueChanged.AddListener();  Bind 的原生实现，
			
			// 按钮点击
			Button.OnClickAsObservable()
				.Subscribe(_ =>
				{
					// Count 的值 ++
					mCount.Value++;

				}).AddTo(gameObject);
			
			// 结论:
			// BindableProperty 的本质，就是拥有 数据值 + 数据值变更事件 的一个对象
		}

		void OnDestroy()
		{
			// Destory Code Here
		}
	}
}

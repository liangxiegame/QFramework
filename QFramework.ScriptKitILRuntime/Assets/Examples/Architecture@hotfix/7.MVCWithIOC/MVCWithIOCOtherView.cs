using UniRx;

namespace QFramework.Example
{
	public partial class MVCWithIOCOtherView
	{
		void OnStart()
		{
			// Code Here

			
			var model = MVCWithIOCConfig.GetModel<MVCWithIOCModel>();
			
			model.Count.BindWithInitialValue(count=>Text.text = count.ToString()).AddTo(gameObject);
		}

		void OnDestroy()
		{
			// Destory Code Here
		}
	}
}

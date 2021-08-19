using UniRx;

namespace QFramework.Example
{
    /// <summary>
    /// 多个界面中需要共享 Model，可以考虑引入 IOC ，让 IOC 容器管理 Model 对象
    /// </summary>
    public partial class MVCWithIOCExample
    {
        void OnStart()
        {
            var model = MVCWithIOCConfig.GetModel<MVCWithIOCModel>();

            model.Count.BindWithInitialValue(count => Text.text = count.ToString()).AddTo(gameObject);
            
            Button.OnClickAsObservable().Subscribe(_ => { model.Count.Value++; });
        }

        void OnDestroy()
        {
            // Destory Code Here
        }
    }
}
using UnityEngine;

namespace QFramework
{
    [ActionGroup("Unity")]
    public class InstantiateGameObject : ActionKitAction
    {
        protected override void OnBegin()
        {
            base.OnBegin();
            
            
            
            Finish();
        }
    }
}
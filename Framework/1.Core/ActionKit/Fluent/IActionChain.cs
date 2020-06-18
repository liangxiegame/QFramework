using UnityEngine;

namespace QFramework
{
    /// <summary>
    /// 执行链表,
    /// </summary>
    public interface IActionChain : IAction
    {
        MonoBehaviour Executer { get; set; }

        IActionChain Append(IAction node);

        IDisposeWhen Begin();
    }
}
using Unidux.SceneTransition;
using UnityEngine;

namespace Unidux.Example.SceneTransition
{
    public class SceneDispatcher : MonoBehaviour
    {
        void Start()
        {
            Unidux.Dispatch(PageDuck<Page, Scene>.ActionCreator.Reset());
            Unidux.Dispatch(PageDuck<Page, Scene>.ActionCreator.Push(Page.Page1));
        }
    }
}
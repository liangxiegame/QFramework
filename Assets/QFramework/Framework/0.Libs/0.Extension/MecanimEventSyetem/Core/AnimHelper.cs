#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;
[ExecuteInEditMode]
public class AnimHelper : MonoBehaviour
{
    Animator animator;
    public List<StateInfo> clipsInfo = new List<StateInfo>();

    void Start()
    {
        animator = GetComponent<Animator>();
#if UNITY_EDITOR
        ShowClipInfo(animator);
#endif
    }

    private void ShowClipInfo(Animator animator)
    {
        AnimatorController controller = animator ? animator.runtimeAnimatorController as AnimatorController : null;
        if (null == controller)
        {
            Debug.LogError("[严重]：动画机或动画机控制器为空，请检查！");
        }
        else
        {
            for (int i = 0; i < controller.layers.Length; i++)
            {
                ChildAnimatorState[] states = controller.layers[i].stateMachine.states;
                foreach (ChildAnimatorState item in states)
                {
                    StateInfo clipInfo = new StateInfo
                    {
                        stateName = string.Format("{0}.{1}",animator.GetLayerName(i), item.state.name),
                        layerIndex = i
                    };
                    if (item.state.motion.GetType() == typeof(AnimationClip))
                    {
                        clipInfo.clip = (AnimationClip)item.state.motion;
                    }
                    else
                    {
                        clipInfo.clip = null;
                        Debug.LogWarning("暂不支持BlendTree动画片段预览。");
                    }
                    foreach (AnimationEvent ev in clipInfo.clip.events)
                    {
                        clipInfo.funcs.Add(ev.functionName);
                    }
                    clipsInfo.Add(clipInfo);
                }
            }
        }
    }

    [System.Serializable]
    public class StateInfo
    {
        public string stateName;
        public AnimationClip clip;
        public int layerIndex;
        public List<string> funcs = new List<string>();
    }
}
#endif
using UnityEngine;
using UnityEngine.UI;

namespace QFramework.Example
{
    public class FSMFrameCountExample : MonoBehaviour
    {
        enum States
        {
            FadeAlphaIn,
            FadeAlphaOut,
            FadeColorBlue,
            FadeColorRed,
            Delay,
            RotateTo
        }

        private FSM<States> mFSM = new FSM<States>();

        public Image Image;
        
        void Start()
        {
            Application.targetFrameRate = 60;

            mFSM.State(States.FadeAlphaIn)
                .OnEnter(() => Image.ColorAlpha(0))
                .OnUpdate(() =>
                {
                    if (mFSM.FrameCountOfCurrentState <= 60)
                    {
                        Image.ColorAlpha(Mathf.Lerp(0, 1, mFSM.FrameCountOfCurrentState / 60.0f));
                    }
                    else
                    {
                        mFSM.ChangeState(States.FadeAlphaOut);
                    }
                });

            mFSM.State(States.FadeAlphaOut)
                .OnUpdate(() =>
                {
                    if (mFSM.FrameCountOfCurrentState <= 60)
                    {
                        Image.ColorAlpha(Mathf.Lerp(1, 0, mFSM.FrameCountOfCurrentState / 60.0f));
                    }
                    else
                    {
                        mFSM.ChangeState(States.FadeColorBlue);
                    }
                });

            mFSM.State(States.FadeColorBlue)
                .OnUpdate(() =>
                {
                    if (mFSM.FrameCountOfCurrentState <= 60)
                    {
                        Image.color = Color.Lerp(new Color(1, 1, 1, 0), Color.blue,
                            mFSM.FrameCountOfCurrentState / 60.0f);
                    }
                    else
                    {
                        mFSM.ChangeState(States.FadeColorRed);
                    }
                });
            
            mFSM.State(States.FadeColorRed)
                .OnUpdate(() =>
                {
                    if (mFSM.FrameCountOfCurrentState <= 60)
                    {
                        Image.color = Color.Lerp(Color.blue, Color.red,
                            mFSM.FrameCountOfCurrentState / 60.0f);
                    }
                    else
                    {
                        mFSM.ChangeState(States.Delay);
                    }
                });
            
            mFSM.State(States.Delay)
                .OnUpdate(() =>
                {
                    if (mFSM.FrameCountOfCurrentState > 60)
                    {
                        mFSM.ChangeState(States.RotateTo);
                    }
                });
            
            mFSM.State(States.RotateTo)
                .OnUpdate(() =>
                {
                    if (mFSM.FrameCountOfCurrentState <= 60)
                    {
                        Image.Rotation(Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(45, 45, 45),
                            mFSM.FrameCountOfCurrentState / 60.0f));
                    }
                });
            
            mFSM.StartState(States.FadeAlphaIn);
        }

        // Update is called once per frame
        void Update()
        {
            mFSM.Update();
        }

        private void OnDestroy()
        {
            mFSM.Clear();
            mFSM = null;
        }
    }
}

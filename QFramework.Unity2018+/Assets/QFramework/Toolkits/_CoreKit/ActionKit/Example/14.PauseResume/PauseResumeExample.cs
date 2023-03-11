using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace QFramework.Example
{
    public class PauseResumeExample : MonoBehaviour
    {
        // Start is called before the first frame update
        IEnumerator Start()
        {
            // NOT Support Yet
            var action = ActionKit.Repeat()
                .Delay(0.5f)
                .Callback(() => Debug.Log(Time.time))
                .Start(this);

            yield return new WaitForSeconds(3.0f);
            action.Pause();

            yield return new WaitForSeconds(2.0f);
            action.Resume();

        }
    }
}

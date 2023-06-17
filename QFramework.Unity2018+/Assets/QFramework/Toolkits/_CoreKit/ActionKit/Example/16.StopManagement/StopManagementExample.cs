using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class StopManagementExample : MonoBehaviour
{
    private List<IActionController> mActionControllers = new List<IActionController>();
    // Start is called before the first frame update
    void Start()
    {
        var a= default(IActionController);
        a = ActionKit.Sequence()
            .Callback(() =>
            {
            })
            .Delay(1.0f)
            .Start(this, () =>
            {
                ActionKit.Sequence()
                    .Delay(0.5f)
                    .Start(this, () =>
                    {
                        a.Deinit();
                    });
            });
    }

}

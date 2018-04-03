using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class call : MonoBehaviour {

    void Start()
    {

        ISing tempSingTest = singletonTest.Instance;

        this.Repeat()
            .Until(() => { return Input.GetKeyDown(KeyCode.Space); })
            .Event(() => { tempSingTest.Run(); })
            .Begin();

        this.Repeat()
            .Until(() => { return Input.GetKeyDown(KeyCode.S); })
            .Event(() => { tempSingTest = singletonTest3.Instance; })
            .Begin();

        this.Repeat()
            .Until(() => { return Input.GetKeyDown(KeyCode.A); })
            .Event(() => { tempSingTest = singletonTest2.Instance; })
            .Begin();

    }
	

}

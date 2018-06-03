

using System.Diagnostics;
using QFramework;
using UnityEngine;


public class RefCounterExample : MonoBehaviour
{
    void Start()
    {
        var room = new Room();
        
        room.EnterPeople();
        room.EnterPeople();
        room.EnterPeople();
        
        room.LeavePeople();
        room.LeavePeople();
        room.LeavePeople();
    }
}

public class Light
{
    public void SwitchOn()
    {
        Log.E("开灯");    
    }
    
    public void SwitchOff()
    {
        Log.E("关灯");
    }
}

public class Room : SimpleRC
{
    private Light mLight = new Light();

    public void EnterPeople()
    {
        Log.E("进入人了");

        if (RefCount == 0)
        {
            mLight.SwitchOn();
        }

        Retain();
    }

    public void LeavePeople()
    {
        Release();

        Log.E("人出来了");
    }

    protected override void OnZeroRef()
    {
        mLight.SwitchOff();
    }
}
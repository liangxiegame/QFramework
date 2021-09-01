using UnityEngine;

public class Sample : MonoBehaviour
{
	[System.Serializable]
	public struct SampleStruct
	{
		public float hp;
	}

	[Header("Try to change the value")]
	public bool isShowLevel;
	[ShowIf(nameof(isShowLevel))]
	public int level;
	public float speed;

	public bool isMoving => speed > 0;
	[ShowIf(nameof(isMoving))]
	public SampleStruct data;
	
	[ButtonEx("Test")]
    void Start()
    {
	    Debug.Log(1);
    }

    void Test()
    {
	    Debug.Log("Test");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void NameGo()
    {
	    name = $"{GetType()}_Lv{level}";
    }
}

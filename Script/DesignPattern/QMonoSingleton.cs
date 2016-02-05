using UnityEngine;
using System.Collections;

/// <summary>
/// 需要使用Unity生命周期的单例模式
/// </summary>
public abstract class QMonoSingleton<T> : MonoBehaviour where T : QMonoSingleton<T>
{
    protected static T instance = null;

    public static T Instance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<T>();

            if (FindObjectsOfType<T>().Length > 1)
            {
                Debug.LogError("More than 1!");
                return instance;
            }

            if (instance == null)
            {
                string instanceName = typeof(T).Name;
                Debug.Log("Instance Name: " + instanceName);
                GameObject instanceGO = GameObject.Find(instanceName);
                if (instanceGO == null)
                    instanceGO = new GameObject(instanceName);
                instance = instanceGO.AddComponent<T>();
                DontDestroyOnLoad(instanceGO);
                Debug.Log("Add New Singleton " + instance.name + " in Game!");
            }
            else
            {
                Debug.Log("Already exist: " + instance.name);
            }
        }

        return instance;
    }

    protected virtual void OnDestroy()
    {
        instance = null;
    }
}

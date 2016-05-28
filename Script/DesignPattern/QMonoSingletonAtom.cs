using UnityEngine;

namespace QFramework
{
	/// <summary>
	/// Be aware this will not prevent a non singleton constructor
	///   such as `T myT = new T();`
	/// To prevent that, add `protected T () {}` to your singleton class.
	/// 
	/// As a note, this is made as MonoBehaviour because we need Coroutines.
	/// 
	/// 加线程锁的单例
	/// </summary>
	public class QMonoSingletonAtom<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T mInstance;

		private static object mLock = new object();

		public static T Instance
		{
			get
			{
				if (applicationIsQuitting)
				{ 
					QPrint.FrameworkError ("[Singleton] Instance '" + typeof(T) +
					"' already destroyed on application quit." +
					" Won't create again - returning null.");
					return null;
				}

				lock (mLock)
				{
					if (mInstance == null)
					{
						mInstance = (T)FindObjectOfType(typeof(T));

						if (FindObjectsOfType(typeof(T)).Length > 1)
						{
							QPrint.FrameworkError ("[Singleton] Something went really wrong " +
								" - there should never be more than 1 singleton!" +
								" Reopening the scene might fix it.");
							return mInstance;
						}

						if (mInstance == null)
						{
							GameObject singleton = new GameObject();
							mInstance = singleton.AddComponent<T>();
							singleton.name = "(singleton) " + typeof(T).ToString();

							DontDestroyOnLoad(singleton);

							QPrint.FrameworkError ("[Singleton] An instance of " + typeof(T) +
								" is needed in the scene, so '" + singleton +
								"' was created with DontDestroyOnLoad.");
						}
						else
						{
							QPrint.FrameworkError ("[Singleton] Using instance already created: " +
								mInstance.gameObject.name);
						}
					}

					return mInstance;
				}
			}
		}

		private static bool applicationIsQuitting = false;
		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed, 
		///   it will create a buggy ghost object that will stay on the Editor scene
		///   even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		public void OnDestroy()
		{
			applicationIsQuitting = true;
		}
	}
}
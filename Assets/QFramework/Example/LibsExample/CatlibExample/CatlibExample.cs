using System.Collections;
using CatLib;
using UnityEngine;
using Application = CatLib.Application;
using IServiceProvider = CatLib.IServiceProvider;

namespace QFramework.Example
{
	public class CatlibExample : MonoBehaviour
	{
		// Use this for initialization
		IEnumerator Start()
		{

			var application = new Application();
			application.Bootstrap();
			application.Register(new FileSystemProvider());
			application.Init();
			
			yield return new WaitForEndOfFrame();
			
			App.Make<IFileSystem>().HelloWorld();

		}

		
		public interface IFileSystem
		{
			void HelloWorld();
		}
		
		public class FileSystem : IFileSystem
		{
			public void HelloWorld()
			{
				"HelloWorld".LogInfo();
			}
		}
		
		public class FileSystemProvider : IServiceProvider
		{
			public void Init()
			{
			}

			public void Register()
			{
				App.Singleton<IFileSystem, FileSystem>();
			}
		}
	}
}
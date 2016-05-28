using UnityEngine;
using System.Collections;


namespace QFramework {
	/// <summary>
	/// 加载器接口
	/// </summary>
	public interface ILoader : IEnumerator
	{
		/// <summary>
		/// 判断是否加载完成
		/// </summary>
		/// <returns></returns>
		bool IsDone();
	}

	/// <summary>
	/// 资源加载器接口
	/// </summary>
	public interface IResLoader : ILoader 
	{
		/// <summary>
		/// 资源名，规范：从Resources下开始，包含后缀的完整路径名
		/// </summary>
		string ResName { get; set; }
		/// <summary>
		/// 获取资源对象
		/// </summary>
		/// <returns></returns>
		Object GetResObj();
		/// <summary>
		/// 资源加载完成回调
		/// </summary>
		ResourceLoader.LoadResDoneCallback LoadDoneCallback { get; set; }
	}

	/// <summary>
	/// 场景加载器接口
	/// </summary>
	public interface ISceneLoader : ILoader
	{
		/// <summary>
		/// 加载进度
		/// </summary>
		/// <returns></returns>
		float GetProgress();
		/// <summary>
		/// 加载完成回调
		/// </summary>
		SceneLoader.LoadSceneDoneCallback LoadDoneCallback { get; set; }
		/// <summary>
		/// 加载进度更新回调
		/// </summary>
		SceneLoader.LoadSceneUpdateCallback LoadUpdateCallback { get; set; }
	}

	/// <summary>
	/// AssetBundle加载器接口
	/// </summary>
	public interface IAssetBundleLoader
	{
		/// <summary>
		/// 检查依赖AssetBundle是否已经加载完成
		/// </summary>
		void CheckDependences();
		/// <summary>
		/// 卸载AssetBundle
		/// </summary>
		void Unload();
	}

}
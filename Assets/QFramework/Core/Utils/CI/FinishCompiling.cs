
//
//using System;
//using UnityEditor;
//
//namespace QFramework
//{
//	[InitializeOnLoad]
//	public class FinishCompiling
//	{
//		private const string COMPILEING_KEY = "Compiling";
//		private static bool mCompiling;
//
//		static FinishCompiling()
//		{
//			mCompiling = EditorPrefs.GetBool(COMPILEING_KEY, false);
//			EditorApplication.update += Update;
//		}
//
//		private static void Update()
//		{
//			if (mCompiling && !EditorApplication.isCompiling)
//			{
//				Log.I(string.Format("Compiling DONE {0}", DateTime.Now));
//				mCompiling = false;
//				EditorPrefs.SetBool(COMPILEING_KEY, false);
//			}
//			else if (!mCompiling && EditorApplication.isCompiling)
//			{
//				Log.I(string.Format("Compiling START {0}", DateTime.Now));
//				mCompiling = true;
//				EditorPrefs.SetBool(COMPILEING_KEY, true);
//			}
//		}
//	}
//}
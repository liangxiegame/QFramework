/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
****************************************************************************/

using QF;

namespace QFramework
{
	using System.IO;
	using System.Collections.Generic;
	using UnityEngine;
	using ICSharpCode.SharpZipLib.Zip;

    public class FileMgr : Singleton<FileMgr>
	{
		private List<string>    mSearchDirList = new List<string>();
		private string          mStreamingAssetsPath;
		private ZipFile         mZipFile = null;

		public ZipFile GetZipFile()
		{
			return mZipFile;
		}
		
		private FileMgr(){}
		
		~FileMgr()
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if (mZipFile != null)
			{
			mZipFile.Close();
			mZipFile = null;
			}
			#endif
		}


		public override void OnSingletonInit()
		{
			mSearchDirList.Add(FilePath.PersistentDataPath4Res);
			mStreamingAssetsPath = FilePath.StreamingAssetsPath;
			#if (UNITY_ANDROID) && !UNITY_EDITOR
			if (mZipFile == null)
			{
			mZipFile = new ZipFile(File.Open(Application.dataPath, FileMode.Open, FileAccess.Read));
			}
			#endif
		}

		public void InitStreamingAssetPath()
		{
			mStreamingAssetsPath = FilePath.StreamingAssetsPath;
		}

		//在包内查找是否有改资源
		private bool FindResInAppInternal(string fileRelativePath)
		{
			#if UNITY_IPHONE && !UNITY_EDITOR
			string absoluteFilePath = FindFilePathInternal(fileRelativePath);
			return !string.IsNullOrEmpty(absoluteFilePath);
			#elif UNITY_ANDROID && !UNITY_EDITOR
			int entryIndex = mZipFile.FindEntry(string.Format("assets/{0}", fileRelativePath), false);
			return entryIndex != -1;
			#else
			string absoluteFilePath = string.Format("{0}{1}", mStreamingAssetsPath, fileRelativePath);
			return File.Exists(absoluteFilePath);
			#endif

		}

		private void AddSearchPath(string dir)
		{
			mSearchDirList.Add(dir);
		}

		public bool FileExists(string fileRelativePath)
		{
			#if UNITY_IPHONE && !UNITY_EDITOR
			string absoluteFilePath = FindFilePath(fileRelativePath);
			return (!string.IsNullOrEmpty(absoluteFilePath) && File.Exists(absoluteFilePath));
			#elif UNITY_ANDROID && !UNITY_EDITOR
			string absoluteFilePath = FindFilePathInExteral(fileRelativePath);
			//先到外存去找
			if (!string.IsNullOrEmpty(absoluteFilePath))
			{
			return File.Exists(absoluteFilePath);
			}
			else
			{
			if (mZipFile == null)
			{
			return false;
			}

			return mZipFile.FindEntry(string.Format("assets/{0}", fileRelativePath), true) >= 0;
			}
			#else
			string filePathStandalone = string.Format("{0}{1}", mStreamingAssetsPath, fileRelativePath);
			return (!string.IsNullOrEmpty(filePathStandalone) && File.Exists(filePathStandalone));
			#endif
		}

		public Stream OpenReadStream(string absFilePath)
		{
			if (string.IsNullOrEmpty(absFilePath))
			{
				return null;
			}

			#if UNITY_ANDROID && !UNITY_EDITOR
			//Android 包内
			if (absFilePath.Contains(".apk/"))
			{
			return OpenStreamInZip(absFilePath);
			}
			#endif
			FileInfo fileInfo = new FileInfo(absFilePath);

			if (!fileInfo.Exists)
			{
				return null;
			}

			return fileInfo.OpenRead();
		}

		public void GetFileInInner(string fileName, List<string> outResult)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			//Android 包内
			GetFileInZip(mZipFile, fileName, outResult);
			return;
			#endif
			FilePath.GetFileInFolder(FilePath.StreamingAssetsPath, fileName, outResult); 
		}

		public byte[] ReadSync(string fileRelativePath)
		{
			string absoluteFilePath = FindFilePathInExteral(fileRelativePath);
			if (!string.IsNullOrEmpty(absoluteFilePath))
			{
				return ReadSyncExtenal(fileRelativePath);
			}

			return ReadSyncInternal(fileRelativePath);
		}

		public byte[] ReadSyncByAbsoluteFilePath(string absoluteFilePath)
		{

			if (File.Exists(absoluteFilePath))
			{
				FileInfo fileInfo = new FileInfo(absoluteFilePath);
				return ReadFile(fileInfo);
			}
			else
			{
				return null;
			}
		}

		private byte[] ReadSyncExtenal(string fileRelativePath)
		{
			string absoluteFilePath = FindFilePathInExteral(fileRelativePath);

			if (!string.IsNullOrEmpty(absoluteFilePath))
			{
				FileInfo fileInfo = new FileInfo(absoluteFilePath);
				return ReadFile(fileInfo);
			}

			return null;
		}

		private byte[] ReadSyncInternal(string fileRelativePath)
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			return ReadDataInAndriodApk(fileRelativePath);
			#else
			string absoluteFilePath = FindFilePathInternal(fileRelativePath);

			if (!string.IsNullOrEmpty(absoluteFilePath))
			{
				FileInfo fileInfo = new FileInfo(absoluteFilePath);
				return ReadFile(fileInfo);
			}
			#endif

			return null;
		}


		private byte[] ReadFile(FileInfo fileInfo)
		{
			using (FileStream fileStream = fileInfo.OpenRead())
			{
				byte[] byteData = new byte[fileStream.Length];
				fileStream.Read(byteData, 0, byteData.Length);
				return byteData;
			}
		}

		private string FindFilePathInExteral(string file)
		{
			string filePath;
			for (int i = 0; i < mSearchDirList.Count; ++i)
			{
				filePath = string.Format("{0}/{1}", mSearchDirList[i], file);
				if (File.Exists(filePath))
				{
					return filePath;
				}
			}

			return string.Empty;
		}

		private string FindFilePath(string file)
		{
			// 先到搜索列表里找
			string filePath = FindFilePathInExteral(file);
			if (!string.IsNullOrEmpty(filePath))
			{
				return filePath;
			}

			// 在包内找
			filePath = FindFilePathInternal(file);
			if (!string.IsNullOrEmpty(filePath))
			{
				return filePath;
			}

			return null;
		}

		private string FindFilePathInternal(string file)
		{
			string filePath = string.Format("{0}{1}", mStreamingAssetsPath, file);

			if (File.Exists(filePath))
			{
				return filePath;
			}

			return null;
		}


		private Stream OpenStreamInZip(string absPath)
		{
			string tag = "!/assets/";
			string androidFolder = absPath.Substring(0, absPath.IndexOf(tag));

			int startIndex = androidFolder.Length + tag.Length;
			string relativePath = absPath.Substring(startIndex, absPath.Length - startIndex);

			ZipEntry zipEntry = mZipFile.GetEntry(string.Format("assets/{0}", relativePath));

			if (zipEntry != null)
			{
				return mZipFile.GetInputStream(zipEntry);
			}
			else
			{
				Log.E(string.Format("Can't Find File {0}", absPath));
			}

			return null;
		}

		public void GetFileInZip(ZipFile zipFile, string fileName, List<string> outResult)
		{
			int totalCount = 0;

			foreach (var entry in zipFile)
			{
				++totalCount;
				ICSharpCode.SharpZipLib.Zip.ZipEntry e = entry as ICSharpCode.SharpZipLib.Zip.ZipEntry;
				if (e != null)
				{
					if (e.IsFile)
					{
						if (e.Name.EndsWith(fileName))
						{
							outResult.Add(zipFile.Name + "/!/" + e.Name);
						}
					}
				}
			}
		}


		private byte[] ReadDataInAndriodApk(string fileRelativePath)
		{
			byte[] byteData = null;
			//Log.I("Read From In App...");
			if (mZipFile == null)
			{
				Log.E("can't open apk");
				return null;
			}

			//Log.I("Begin Open Zip...");
			ZipEntry zipEntry = mZipFile.GetEntry(string.Format("assets/{0}", fileRelativePath));
			//Log.I("End Open Zip...");
			if (zipEntry != null)
			{
				//Log.I("Begin Read Zip...");
				var stream = mZipFile.GetInputStream(zipEntry);
				byteData = new byte[zipEntry.Size];
				//Log.I("Read Zip Length:" + byteData.Length);
				stream.Read(byteData, 0, byteData.Length);
				stream.Close();
			}
			else
			{
				Log.E(string.Format("Can't Find File {0}", fileRelativePath));
			}

			return byteData;
		}
	}
}

/****************************************************************************
 * Copyright (c) 2017 wanzhenyu@putao.com
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.Net;
using System.IO;
using System;

namespace QFramework
{
	class FTPClient : IFTPInterface
	{
		private string mHost = null;
		private string mUser = null;
		private string mPass = null;
		private FtpWebRequest mFtpRequest = null;
		private FtpWebResponse mFtpResponse = null;
		private Stream mFtpStream = null;
		private int mBufferSize = 2048;

		public bool Connect()
		{

			return true;
		}

		public bool Exist(string dir, string fileName)
		{
			string[] files = directoryListSimple(dir);
			foreach (var file in files)
			{
				UnityEngine.Debug.Log(file + " >>>>>>>>>" + fileName);
				if (file == fileName)
				{
					return true;
				}
			}
			return false;
		}

		public bool Rename(string oldName, string newPath)
		{
			string newFileName = newPath.Substring(newPath.LastIndexOf('/') + 1);
			UnityEngine.Debug.Log(newFileName + " newFileName ******");
			return rename(oldName, newFileName);
		}

		public bool MakeDir(string pathName)
		{
			return createDirectory(pathName);

		}

		public bool Upload(string localPath, string remotePath)
		{
			return upload(remotePath, localPath);
		}

		public void Disconnect()
		{


		}


		/* Construct Object */
		public FTPClient(string hostIP, string userName, string password)
		{
			mHost = hostIP;
			mUser = userName;
			mPass = password;
		}

		/* Download File */
		public void download(string remoteFile, string localFile)
		{
			try
			{
				/* Create an FTP Request */
				mFtpRequest = (FtpWebRequest) FtpWebRequest.Create(mHost + "/" + remoteFile);
				/* Log in to the FTP Server with the User Name and Password Provided */
				mFtpRequest.Credentials = new NetworkCredential(mUser, mPass);
				/* When in doubt, use these options */
				mFtpRequest.UseBinary = true;
				mFtpRequest.UsePassive = true;
				mFtpRequest.KeepAlive = true;
				/* Specify the Type of FTP Request */
				mFtpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
				/* Establish Return Communication with the FTP Server */
				mFtpResponse = (FtpWebResponse) mFtpRequest.GetResponse();
				/* Get the FTP Server's Response Stream */
				mFtpStream = mFtpResponse.GetResponseStream();
				/* Open a File Stream to Write the Downloaded File */
				FileStream localFileStream = new FileStream(localFile, FileMode.Create);
				/* Buffer for the Downloaded Data */
				byte[] byteBuffer = new byte[mBufferSize];
				int bytesRead = mFtpStream.Read(byteBuffer, 0, mBufferSize);
				/* Download the File by Writing the Buffered Data Until the Transfer is Complete */
				try
				{
					while (bytesRead > 0)
					{
						localFileStream.Write(byteBuffer, 0, bytesRead);
						bytesRead = mFtpStream.Read(byteBuffer, 0, mBufferSize);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
				/* Resource Cleanup */
				localFileStream.Close();
				mFtpStream.Close();
				mFtpResponse.Close();
				mFtpRequest = null;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			return;
		}

		/* Upload File */
		public bool upload(string remoteFile, string localFile)
		{
			try
			{
				UnityEngine.Debug.Log(remoteFile + "REMOTEFILE" + localFile);
				/* Create an FTP Request */
				mFtpRequest = (FtpWebRequest) FtpWebRequest.Create(mHost + "/" + remoteFile);
				/* Log in to the FTP Server with the User Name and Password Provided */
				mFtpRequest.Credentials = new NetworkCredential(mUser, mPass);
				/* When in doubt, use these options */
				mFtpRequest.UseBinary = true;

				mFtpRequest.UsePassive = true;
				mFtpRequest.KeepAlive = true;
				/* Specify the Type of FTP Request */
				mFtpRequest.Method = WebRequestMethods.Ftp.UploadFile;
				/* Establish Return Communication with the FTP Server */
//			ftpStream = ftpRequest.GetRequestStream();
				/* Open a File Stream to Read the File for Upload */
//			FileStream localFileStream = new FileStream(localFile, FileMode.Create);
//			/* Buffer for the Downloaded Data */
//			byte[] byteBuffer = new byte[bufferSize];
//			int bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
//			/* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
//			try
//			{
//				while (bytesSent != 0)
//				{
//					ftpStream.Write(byteBuffer, 0, bytesSent);
//					bytesSent = localFileStream.Read(byteBuffer, 0, bufferSize);
//				}
//			}
//			catch (Exception ex) 
//			{
//				Console.WriteLine(ex.ToString());
//			
//				return false;
//			}



//				StreamReader sourceStream = new StreamReader (localFile);
//				byte[] fileContents = Encoding.UTF8.GetBytes (sourceStream.ReadToEnd ());
//				sourceStream.Close ();
//				ftpRequest.ContentLength = fileContents.Length;
//				UnityEngine.Debug.Log(fileContents.Length+" *****ftpRequest.ContentLength ***********"+localFile);
//
				Stream requestStream = mFtpRequest.GetRequestStream();
//				requestStream.Write (fileContents, 0, fileContents.Length);

				UnityEngine.Debug.Log("localFile****** :" + localFile);
				FileStream fileStream = File.Open(localFile, FileMode.Open);
				byte[] buffer = new byte [1024];
				int bytesRead;
				while (true)
				{
					bytesRead = fileStream.Read(buffer, 0, buffer.Length);
					if (bytesRead == 0)
						break;

					//本地的文件流数据写到请求流
					requestStream.Write(buffer, 0, bytesRead);
				}

				requestStream.Close();
				fileStream.Close();

				/* Resource Cleanup */
//				sourceStream.Close ();
				mFtpStream.Close();
				mFtpRequest = null;
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return false;
			}
			return false;
		}

		/* Delete File */
		public void delete(string deleteFile)
		{
			try
			{
				/* Create an FTP Request */
				mFtpRequest = (FtpWebRequest) WebRequest.Create(mHost + "/" + deleteFile);
				/* Log in to the FTP Server with the User Name and Password Provided */
				mFtpRequest.Credentials = new NetworkCredential(mUser, mPass);
				/* When in doubt, use these options */
				mFtpRequest.UseBinary = true;
				mFtpRequest.UsePassive = true;
				mFtpRequest.KeepAlive = true;
				/* Specify the Type of FTP Request */
				mFtpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
				/* Establish Return Communication with the FTP Server */
				mFtpResponse = (FtpWebResponse) mFtpRequest.GetResponse();
				/* Resource Cleanup */
				mFtpResponse.Close();
				mFtpRequest = null;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			return;
		}

		/* Rename File */
		public bool rename(string currentFileNameAndPath, string newFileName)
		{
			try
			{
				UnityEngine.Debug.Log("currentFileNameAndPath:" + currentFileNameAndPath + " newFileName:" + newFileName);
				/* Create an FTP Request */
				mFtpRequest = (FtpWebRequest) WebRequest.Create(mHost + "/" + currentFileNameAndPath);
				/* Log in to the FTP Server with the User Name and Password Provided */
				mFtpRequest.Credentials = new NetworkCredential(mUser, mPass);
				/* When in doubt, use these options */
				mFtpRequest.UseBinary = true;
				mFtpRequest.UsePassive = true;
				mFtpRequest.KeepAlive = true;
				/* Specify the Type of FTP Request */
				mFtpRequest.Method = WebRequestMethods.Ftp.Rename;
				/* Rename the File */
				mFtpRequest.RenameTo = newFileName;
				/* Establish Return Communication with the FTP Server */
				mFtpResponse = (FtpWebResponse) mFtpRequest.GetResponse();
				/* Resource Cleanup */
				mFtpResponse.Close();
				mFtpRequest = null;
				return true;
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.ToString());
				return false;
			}
			return false;
		}

		/* Create a New Directory on the FTP Server */
		public bool createDirectory(string newDirectory)
		{
			try
			{
				/* Create an FTP Request */
				mFtpRequest = (FtpWebRequest) WebRequest.Create(mHost + "/" + newDirectory);
				/* Log in to the FTP Server with the User Name and Password Provided */
				mFtpRequest.Credentials = new NetworkCredential(mUser, mPass);
				/* When in doubt, use these options */
				mFtpRequest.UseBinary = true;
				mFtpRequest.UsePassive = true;
				mFtpRequest.KeepAlive = true;
				/* Specify the Type of FTP Request */
				mFtpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
				/* Establish Return Communication with the FTP Server */
				mFtpResponse = (FtpWebResponse) mFtpRequest.GetResponse();
				/* Resource Cleanup */
				mFtpResponse.Close();
				mFtpRequest = null;
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());

				return true;
			}
			return false;
		}

		/* Get the Date/Time a File was Created */
		public string getFileCreatedDateTime(string fileName)
		{
			try
			{
				/* Create an FTP Request */
				mFtpRequest = (FtpWebRequest) FtpWebRequest.Create(mHost + "/" + fileName);
				/* Log in to the FTP Server with the User Name and Password Provided */
				mFtpRequest.Credentials = new NetworkCredential(mUser, mPass);
				/* When in doubt, use these options */
				mFtpRequest.UseBinary = true;
				mFtpRequest.UsePassive = true;
				mFtpRequest.KeepAlive = true;
				/* Specify the Type of FTP Request */
				mFtpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
				/* Establish Return Communication with the FTP Server */
				mFtpResponse = (FtpWebResponse) mFtpRequest.GetResponse();
				/* Establish Return Communication with the FTP Server */
				mFtpStream = mFtpResponse.GetResponseStream();
				/* Get the FTP Server's Response Stream */
				StreamReader ftpReader = new StreamReader(mFtpStream);
				/* Store the Raw Response */
				string fileInfo = null;
				/* Read the Full Response Stream */
				try
				{
					fileInfo = ftpReader.ReadToEnd();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
				/* Resource Cleanup */
				ftpReader.Close();
				mFtpStream.Close();
				mFtpResponse.Close();
				mFtpRequest = null;
				/* Return File Created Date Time */
				return fileInfo;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			/* Return an Empty string Array if an Exception Occurs */
			return "";
		}

		/* Get the Size of a File */
		public string getFileSize(string fileName)
		{
			try
			{
				/* Create an FTP Request */
				mFtpRequest = (FtpWebRequest) FtpWebRequest.Create(mHost + "/" + fileName);
				/* Log in to the FTP Server with the User Name and Password Provided */
				mFtpRequest.Credentials = new NetworkCredential(mUser, mPass);
				/* When in doubt, use these options */
				mFtpRequest.UseBinary = true;
				mFtpRequest.UsePassive = true;
				mFtpRequest.KeepAlive = true;
				/* Specify the Type of FTP Request */
				mFtpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
				/* Establish Return Communication with the FTP Server */
				mFtpResponse = (FtpWebResponse) mFtpRequest.GetResponse();
				/* Establish Return Communication with the FTP Server */
				mFtpStream = mFtpResponse.GetResponseStream();
				/* Get the FTP Server's Response Stream */
				StreamReader ftpReader = new StreamReader(mFtpStream);
				/* Store the Raw Response */
				string fileInfo = null;
				/* Read the Full Response Stream */
				try
				{
					while (ftpReader.Peek() != -1)
					{
						fileInfo = ftpReader.ReadToEnd();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
				/* Resource Cleanup */
				ftpReader.Close();
				mFtpStream.Close();
				mFtpResponse.Close();
				mFtpRequest = null;
				/* Return File Size */
				return fileInfo;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			/* Return an Empty string Array if an Exception Occurs */
			return "";
		}

		/* List Directory Contents File/Folder Name Only */
		public string[] directoryListSimple(string directory)
		{
			UnityEngine.Debug.Log(directory + "*****DIRECTORY");
			try
			{
				/* Create an FTP Request */
				mFtpRequest = (FtpWebRequest) FtpWebRequest.Create(mHost + "/" + directory + "/");
				/* Log in to the FTP Server with the User Name and Password Provided */
				mFtpRequest.Credentials = new NetworkCredential(mUser, mPass);
				/* When in doubt, use these options */
				mFtpRequest.UseBinary = true;
				mFtpRequest.UsePassive = true;
				mFtpRequest.KeepAlive = true;
				/* Specify the Type of FTP Request */
				mFtpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
				/* Establish Return Communication with the FTP Server */
				mFtpResponse = (FtpWebResponse) mFtpRequest.GetResponse();
				/* Establish Return Communication with the FTP Server */
				mFtpStream = mFtpResponse.GetResponseStream();
				/* Get the FTP Server's Response Stream */
				StreamReader ftpReader = new StreamReader(mFtpStream);
				/* Store the Raw Response */
				string directoryRaw = null;
				/* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
				try
				{
					while (ftpReader.Peek() != -1)
					{
						directoryRaw += ftpReader.ReadLine() + "|";
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
				/* Resource Cleanup */
				ftpReader.Close();
				mFtpStream.Close();
				mFtpResponse.Close();
				mFtpRequest = null;
				/* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
				try
				{
					string[] directoryList = directoryRaw.Split("|".ToCharArray());
					return directoryList;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			/* Return an Empty string Array if an Exception Occurs */
			return new string[] {""};
		}

		/* List Directory Contents in Detail (Name, Size, Created, etc.) */
		public string[] directoryListDetailed(string directory)
		{
			try
			{
				UnityEngine.Debug.Log(mHost + "/" + directory);
				/* Create an FTP Request */
				mFtpRequest = (FtpWebRequest) FtpWebRequest.Create(mHost + "/" + directory);
				/* Log in to the FTP Server with the User Name and Password Provided */
				mFtpRequest.Credentials = new NetworkCredential(mUser, mPass);
				/* When in doubt, use these options */
				mFtpRequest.UseBinary = true;
				mFtpRequest.UsePassive = true;
				mFtpRequest.KeepAlive = true;
				/* Specify the Type of FTP Request */
				mFtpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
				/* Establish Return Communication with the FTP Server */
				mFtpResponse = (FtpWebResponse) mFtpRequest.GetResponse();
				/* Establish Return Communication with the FTP Server */
				mFtpStream = mFtpResponse.GetResponseStream();
				/* Get the FTP Server's Response Stream */
				StreamReader ftpReader = new StreamReader(mFtpStream);
				/* Store the Raw Response */
				string directoryRaw = null;
				/* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
				try
				{
					while (ftpReader.Peek() != -1)
					{
						directoryRaw += ftpReader.ReadLine() + "|";
					}
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.Log(ex.ToString());
				}
				/* Resource Cleanup */
				ftpReader.Close();
				mFtpStream.Close();
				mFtpResponse.Close();
				mFtpRequest = null;
				/* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
				try
				{
					string[] directoryList = directoryRaw.Split("|".ToCharArray());
					return directoryList;
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.Log(ex.ToString());
				}
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log(ex.ToString());
			}
			/* Return an Empty string Array if an Exception Occurs */
			return new string[] {""};
		}
	}
}
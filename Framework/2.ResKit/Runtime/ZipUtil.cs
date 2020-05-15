/****************************************************************************
 * Copyright (c) 2017 liangxie
 *
 * Reference from :http://www.cnblogs.com/tuyile006/archive/2008/04/25/1170894.html
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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

using ICSharpCode.SharpZipLib.Checksum;


namespace QFramework
{
    using System;
    using System.IO;
    using ICSharpCode.SharpZipLib.Zip;

    public class ZipUtil
    {
        /// <summary>
        /// ZIP:压缩单个文件
        /// add yuangang by 2016-06-13
        /// </summary>
        /// <param name="FileToZip">需要压缩的文件（绝对路径）</param>
        /// <param name="ZipedFilePath">压缩后的文件路径（绝对路径）</param>
        /// <param name="ZipedFileName">压缩后的文件名称（文件名，默认 同源文件同名）</param>
        /// <param name="CompressionLevel">压缩等级（0 无 - 9 最高，默认 5）</param>
        /// <param name="BlockSize">缓存大小（每次写入文件大小，默认 2048）</param>
        /// <param name="IsEncrypt">是否加密（默认 加密）</param>
        public static void ZipFile(string FileToZip, string ZipedFilePath, string ZipedFileName = "",
            int CompressionLevel = 5, int BlockSize = 2048, bool IsEncrypt = false)
        {
            //如果文件没有找到，则报错
            if (!System.IO.File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + FileToZip + " 不存在!");
            }

            //文件名称（默认同源文件名称相同）
            string ZipFileName = string.IsNullOrEmpty(ZipedFileName)
                ? ZipedFilePath + Path.DirectorySeparatorChar.ToString() +
                  new FileInfo(FileToZip).Name.Substring(0, new FileInfo(FileToZip).Name.LastIndexOf('.')) + ".zip"
                : ZipedFilePath + Path.DirectorySeparatorChar.ToString() + ZipedFileName + ".zip";

            using (System.IO.FileStream ZipFile = System.IO.File.Create(ZipFileName))
            {
                using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
                {
                    using (System.IO.FileStream StreamToZip =
                        new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        string fileName =
                            FileToZip.Substring(FileToZip.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1);

                        ZipEntry ZipEntry = new ZipEntry(fileName);

                        if (IsEncrypt)
                        {
                            //压缩文件加密
                            ZipStream.Password = "123";
                        }

                        ZipStream.PutNextEntry(ZipEntry);

                        //设置压缩级别
                        ZipStream.SetLevel(CompressionLevel);

                        //缓存大小
                        byte[] buffer = new byte[BlockSize];

                        int sizeRead = 0;

                        try
                        {
                            do
                            {
                                sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                                ZipStream.Write(buffer, 0, sizeRead);
                            } while (sizeRead > 0);
                        }
                        catch (System.Exception ex)
                        {
                            throw ex;
                        }

                        StreamToZip.Close();
                    }

                    ZipStream.Finish();
                    ZipStream.Close();
                }

                ZipFile.Close();
            }
        }


        /// <summary>
        /// ZIP：压缩文件夹
        /// add yuangang by 2016-06-13
        /// </summary>
        /// <param name="DirectoryToZip">需要压缩的文件夹（绝对路径）</param>
        /// <param name="ZipedPath">压缩后的文件路径（绝对路径）</param>
        /// <param name="ZipedFileName">压缩后的文件名称（文件名，默认 同源文件夹同名）</param>
        /// <param name="IsEncrypt">是否加密（默认 加密）</param>
        public static void ZipDirectory(string DirectoryToZip, string ZipedPath, bool IsEncrypt = true)
        {
            //如果目录不存在，则报错
            if (!Directory.Exists(DirectoryToZip))
            {
                throw new FileNotFoundException("指定的目录: " + DirectoryToZip + " 不存在!");
            }

            //文件名称（默认同源文件名称相同）

            using (System.IO.FileStream ZipFile = System.IO.File.Create(ZipedPath))
            {
                using (ZipOutputStream s = new ZipOutputStream(ZipFile))
                {
                    if (IsEncrypt)
                    {
                        //压缩文件加密
//                        s.Password = "123";
                    }

                    ZipSetp(DirectoryToZip, s, "");
                }
            }
        }

        /// <summary>
        /// 递归遍历目录
        /// add yuangang by 2016-06-13
        /// </summary>
        private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath)
        {
            if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar)
            {
                strDirectory += Path.DirectorySeparatorChar;
            }

            Crc32 crc = new Crc32();

            string[] filenames = Directory.GetFileSystemEntries(strDirectory);

            foreach (string file in filenames) // 遍历所有的文件和目录
            {

                if (Directory.Exists(file)) // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    string pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1);
                    pPath += Path.DirectorySeparatorChar.ToString();
                    ZipSetp(file, s, pPath);
                }

                else // 否则直接压缩文件
                {
                    //打开压缩文件
                    using (FileStream fs = File.OpenRead(file))
                    {

                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);

                        string fileName = parentPath +
                                          file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1);
                        ZipEntry entry = new ZipEntry(fileName);

                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;

                        fs.Close();

                        crc.Reset();
                        crc.Update(buffer);

                        entry.Crc = crc.Value;
                        s.PutNextEntry(entry);

                        s.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }


//        /// <summary>
//        /// ZIP:解压一个zip文件
//        /// add yuangang by 2016-06-13
//        /// </summary>
//        /// <param name="ZipFile">需要解压的Zip文件（绝对路径）</param>
//        /// <param name="TargetDirectory">解压到的目录</param>
//        /// <param name="Password">解压密码</param>
//        /// <param name="OverWrite">是否覆盖已存在的文件</param>
//        public static void UnZip(string ZipFile, string TargetDirectory, string Password, bool OverWrite = true)
//        {
//            IOUtils.CreateDirIfNotExists(TargetDirectory);
//            //目录结尾
//            if (!TargetDirectory.EndsWith(Path.DirectorySeparatorChar.ToString())) { TargetDirectory = TargetDirectory + Path.DirectorySeparatorChar.ToString(); }
//
//            using (ZipInputStream zipfiles = new ZipInputStream(File.OpenRead(ZipFile)))
//            {
//                zipfiles.Password = Password;
//                ZipEntry theEntry;
//
//                while ((theEntry = zipfiles.GetNextEntry()) != null)
//                {
//                    string directoryName = "";
//                    string pathToZip = "";
//                    pathToZip = theEntry.Name;
//
//                    if (pathToZip != "")
//                        directoryName = Path.GetDirectoryName(pathToZip) + Path.DirectorySeparatorChar.ToString();
//
//                    string fileName = Path.GetFileName(pathToZip);
//
//                    Directory.CreateDirectory(TargetDirectory + directoryName);
//
//                    if (fileName != "")
//                    {
//                        if ((File.Exists(TargetDirectory + directoryName + fileName) && OverWrite) || (!File.Exists(TargetDirectory + directoryName + fileName)))
//                        {
//                            using (FileStream streamWriter = File.Create(TargetDirectory + directoryName + fileName))
//                            {
//                                int size = 2048;
//                                byte[] data = new byte[2048];
//                                while (true)
//                                {
//                                    size = zipfiles.Read(data, 0, data.Length);
//
//                                    if (size > 0)
//                                        streamWriter.Write(data, 0, size);
//                                    else
//                                        break;
//                                }
//                                streamWriter.Close();
//                            }
//                        }
//                    }
//                }
//
//                zipfiles.Close();
//            }
//        }
        /*
        #region 加压解压方法

//        /// <summary>
//        /// 功能：压缩文件（暂时只压缩文件夹下一级目录中的文件，文件夹及其子级被忽略）
//        /// </summary>
//        /// <param name="dirPath">被压缩的文件夹夹路径</param>
//        /// <param name="zipFilePath">生成压缩文件的路径，为空则默认与被压缩文件夹同一级目录，名称为：文件夹名+.zip</param>
//        /// <param name="err">出错信息</param>
//        /// <returns>是否压缩成功</returns>
//        public static bool ZipFile(string dirPath, string zipFilePath, string ignoreSuffix, string  out string err)
//        {
//            err = "";
//            if (dirPath == string.Empty)
//            {
//                err = "要压缩的文件夹不能为空！";
//                return false;
//            }
//            if (!Directory.Exists(dirPath))
//            {
//                err = "要压缩的文件夹不存在！";
//                return false;
//            }
//            //压缩文件名为空时使用文件夹名＋.zip
//            if (zipFilePath == string.Empty)
//            {
//                if (dirPath.EndsWith(Path.PathSeparator.ToString()))
//                {
//                    dirPath = dirPath.Substring(0, dirPath.Length - 1);
//                }
//                zipFilePath = dirPath + ".zip";
//            }
//
//            try
//            {
//                string[] filenames = IOUtils.GetDirSubFilePathList(dirPath, true, null).ToArray();
//                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
//                {
//                    s.SetLevel(9);
//                    byte[] buffer = new byte[4096];
//                    foreach (string file in filenames)
//                    {
//                        if (!string.IsNullOrEmpty(ignoreSuffix) && file.EndsWith(ignoreSuffix)) continue;
//                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
//                        entry.DateTime = DateTime.Now;
//                        s.PutNextEntry(entry);
//                        using (FileStream fs = File.OpenRead(file))
//                        {
//                            int sourceBytes;
//                            do
//                            {
//                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
//                                s.Write(buffer, 0, sourceBytes);
//                            } while (sourceBytes > 0);
//                        }
//                    }
//                    s.Finish();
//                    s.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                err = ex.Message;
//                return false;
//            }
//            return true;
//        }

        */
        /// <summary>
        /// 功能：解压zip格式的文件。
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
        /// <param name="err">出错信息</param>
        /// <returns>解压是否成功</returns>
        public static bool UnZipFile(string zipFilePath, string unZipDir)
        {
            if (!Directory.Exists(unZipDir))
            {
                Directory.CreateDirectory(unZipDir);
            }
            

            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath),
                    Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                unZipDir += Path.DirectorySeparatorChar;

            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(unZipDir + directoryName);
                        }

                        if (!directoryName.EndsWith(Path.DirectorySeparatorChar.ToString()))
                            directoryName += Path.DirectorySeparatorChar.ToString();
                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                            {

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    } //while
                }
            }
            catch (Exception ex)
            {
                Log.E(ex);
                return false;
            }

            return true;
        } //解压结束
    }
}
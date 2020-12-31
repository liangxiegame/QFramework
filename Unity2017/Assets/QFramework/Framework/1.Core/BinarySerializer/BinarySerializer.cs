/****************************************************************************
 * Copyright (c) 2018 ~ 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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


using System.IO;

namespace QFramework
{
    public class BinarySerializer : IBinarySerializer
    {
        public bool SerializeBinary(string path, object obj)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.W("SerializeBinary Without Valid Path.");
                return false;
            }

            if (obj == null)
            {
                Log.W("SerializeBinary obj is Null.");
                return false;
            }

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(fs, obj);
                return true;
            }
        }

        public object DeserializeBinary(Stream stream)
        {
            if (stream == null)
            {
                Log.W("DeserializeBinary Failed!");
                return null;
            }

            using (stream)
            {
                var bf =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                var data = bf.Deserialize(stream);

                // TODO:这里没风险嘛?
                return data;
            }
        }

        public object DeserializeBinary(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.W("DeserializeBinary Without Valid Path.");
                return null;
            }

            var fileInfo = new FileInfo(path);

            if (!fileInfo.Exists)
            {
                Log.W("DeserializeBinary File Not Exit.");
                return null;
            }

            using (var fs = fileInfo.OpenRead())
            {
                var bf =
                    new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                var data = bf.Deserialize(fs);

                return data;
            }
        }
    }
}
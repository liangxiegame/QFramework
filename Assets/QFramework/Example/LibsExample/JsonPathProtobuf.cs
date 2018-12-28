/****************************************************************************
 * Copyright (c) 2018.3 布鞋 827922094@qq.com
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

using UnityEngine;

namespace QFramework
{
    public class JsonPathProtobuf : MonoBehaviour
    {
        private void Start()
        {
            var tempProto = new ProtoBufTest
            {
                ID = 1,
                Msg = "Hello"
            };

            var tempJson = new JsonTest {Age = 18};

            this.Sequence()
                .Until(() => { return Input.GetKeyDown(KeyCode.P); })
                .Event(()=>  { string path = "Assets/TestJosn".CreateDirIfNotExists(); path += "/testPro.proto"; tempProto.SaveProtoBuff(path);})
                .Begin();

            this.Sequence()
                .Until(() => { return Input.GetKeyDown(KeyCode.O); })
                .Event(() => { ProtoBufTest tempLoadBuf = SerializeHelper.LoadProtoBuff<ProtoBufTest>(Application.dataPath + "/TestJosn/testPro.proto");
                    Debug.Log(tempLoadBuf.ID);
                })
                .Begin();

            this.Sequence()
                .Until(() => { return Input.GetKeyDown(KeyCode.A); })
                .Event(() => { string path = "Assets/TestJosn".CreateDirIfNotExists(); path += "/TestJson.json"; tempJson.SaveJson(path); })
                .Begin();

            this.Sequence()
                .Until(() => { return Input.GetKeyDown(KeyCode.S); })
                .Event(() => {
                    JsonTest tempLoadJson = SerializeHelper.LoadJson<JsonTest>(Application.dataPath+"/TestJosn/TestJson.json");
                    Debug.Log(tempLoadJson.Age);
                })
                .Begin();
        }

    }

    [ProtoBuf.ProtoContract]
    public class ProtoBufTest
    {
        [ProtoBuf.ProtoMember(1)]
        public int ID=0;

        [ProtoBuf.ProtoMember(2)]
        public string Msg="Hello";
    }

    [System.Serializable]
    public class JsonTest
    {
        private string mName;
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        private int mAge;
        public int Age
        {
            get { return mAge; }
            set { mAge = value; }
        }
    }
}

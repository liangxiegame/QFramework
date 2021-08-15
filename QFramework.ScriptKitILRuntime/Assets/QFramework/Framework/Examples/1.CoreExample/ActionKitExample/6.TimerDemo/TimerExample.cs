/****************************************************************************
 * Copyright (c) 2021.3 liangxie
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
using UnityEngine.UI;

namespace QFramework.TimeExtend
{
    public class TimerExample : MonoBehaviour
    {

        Image image;
        Text  text;
        Text  text2;
        Text  text3;
        Timer timer;

        void Start()
        {
            image = GetComponent<Image>();
            image.type = Image.Type.Filled;
            image.fillAmount = 0;

            text = GetComponentInChildren<Text>();
            text2 = GameObject.Find("Text2").GetComponent<Text>();
            text3 = GameObject.Find("Text3").GetComponent<Text>();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("开启定时"))
            {
                text3.text = "";
                if (!Timer.Exist(timer))
                {
                    timer = Timer.AddTimer(5, "12138").OnUpdated((v) =>
                    {
                        image.fillAmount = v;
                        text.text = (v * timer.Duration).ToString();
                    }).OnCompleted(
                        () => { text3.text = "计时完成！"; }
                    );
                    text2.text = "当前设定时间为：" + timer.Duration;
                }
            }

            if (GUILayout.Button("定时2秒"))
            {
                if (Timer.Exist("12138"))
                {

                    Timer.GetTimer("12138").SetTime(2); //当定时器走了大于2秒时按下，会提示错误，并瞬间完成事件
                    text2.text = "当前设定时间为：" + timer.Duration; //当定时器走了小于2秒时按下，会加速完成
                }
            }

            if (GUILayout.Button("定时-2秒")) //当定时器走了大于2秒时按下，会提示参数必须为正值，并瞬间完成事件
            {
                //当定时器走了小于2秒时按下，会提示参数必须为正值，会加速完成
                if (Timer.Exist("12138"))
                {
                    Timer.GetTimer("12138").SetTime(-2);
                    text2.text = "当前设定时间为：" + timer.Duration;
                }
            }

            if (GUILayout.Button("定时10秒")) //当定时器走了小于2秒时按下，会提示参数必须为正值，会加速完成
            {
                if (null != timer)
                {
                    timer.SetTime(10);
                    text2.text = "当前设定时间为：" + timer.Duration;
                }
            }
        }
    }
}
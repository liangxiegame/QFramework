/****************************************************************************
 * Copyright (c) 2018.9 liangxie
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

using DG.Tweening;
using QF.Action;
using QF.Extensions;
using UnityEngine;

namespace QFramework
{
    public class FadeInOut : UITransition
    {
        public Color PanelColor = Color.black;

        public float FadeInDuration  = 0.5f;
        public float FadeOutDuration = 0.5f;

        public override void Do(UITransitionPanel panel)
        {
            panel.ExecuteNode(this);

            if (FromPanel)
            {
                panel.Image.color = PanelColor;
                panel.Image.ColorAlpha(0.0f);

                DOTween.Sequence()
                    .Append(panel.Image.DOFade(1.0f, FadeInDuration).OnComplete(() =>
                    {
                        UIManager.Instance.CloseUI(FromPanel.name);

                        InCompleted.InvokeGracefully();
                    }))
                    .Append(panel.Image.DOFade(0.0f, FadeOutDuration))
                    .OnComplete(() =>
                    {
                        OutCompleted.InvokeGracefully();
                        Finish();
                    });
            }
            else
            {
                InCompleted.InvokeGracefully();
                panel.Image.color = PanelColor;
                panel.Image.ColorAlpha(1.0f);
                panel.Image.DOFade(0.0f, FadeOutDuration)
                    .OnComplete(() =>
                    {
                        OutCompleted.InvokeGracefully();
                        Finish();
                    });
            }
        }
    }
}
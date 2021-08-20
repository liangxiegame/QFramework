/****************************************************************************
 * Copyright (c) 2018.10 liangxie
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

using System.Collections.Generic;
using UnityEngine;

namespace QFrameowrk.PlatformRunner
{
    public class Stage
    {
        public static List<Stage> Object = new List<Stage>()
        {
            new Stage{Id = 1,ThemeLength=27.0f,
                StageNames=new List<string>{"CustomStage1","CustomStage2","CustomStage3","CustomStage4","CustomStage5","CustomStage6","CustomStage7","CustomStage8","CustomStage9","CustomStage10","CustomStage11","CustomStage12","CustomStage13","CustomStage14","CustomStage15"}
            },
            new Stage{Id = 2, ThemeLength=28.0f,
                StageNames=new List<string>{"CustomStage1","CustomStage2","CustomStage3","CustomStage4","CustomStage5","CustomStage6","CustomStage7","CustomStage8","CustomStage9","CustomStage10","CustomStage11","CustomStage12","CustomStage13","CustomStage14","CustomStage15"}
            },
            new Stage{Id = 3,ThemeLength=28.0f,                
                StageNames=new List<string>{"CustomStage1","CustomStage2","CustomStage3","CustomStage4","CustomStage5","CustomStage6","CustomStage7","CustomStage8","CustomStage9","CustomStage10","CustomStage11","CustomStage12","CustomStage13","CustomStage14","CustomStage15"}
            },
            new Stage{Id = 4, ThemeLength=28.0f,                
                StageNames=new List<string>{"CustomStage1","CustomStage2","CustomStage3","CustomStage4","CustomStage5","CustomStage6","CustomStage7","CustomStage8","CustomStage9","CustomStage10","CustomStage11","CustomStage12","CustomStage13","CustomStage14","CustomStage15"}
            },
        };

        /// <summary>
        /// 关卡 Id
        /// </summary>
        public int Id;

        /// <summary>
        /// 每个主题的宽度
        /// </summary>
        public float ThemeLength;

        /// <summary>
        /// 主题中的关卡名字
        /// </summary>
        public List<string> StageNames;
    }
}
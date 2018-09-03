/****************************************************************************
 * Copyright (c) 2018.9 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * https://blog.csdn.net/puppet_master/article/details/52423905
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

namespace QFramework
{
	using UnityEngine;

	//非运行时也触发效果
	[ExecuteInEditMode]
	//屏幕后处理特效一般都需要绑定在摄像机上
	[RequireComponent(typeof(Camera))]
	//提供一个后处理的基类，主要功能在于直接通过Inspector面板拖入shader，生成shader对应的材质
	public sealed class PostFXBehaviour : MonoBehaviour
	{
		//Inspector面板上直接拖入
		public Shader Shader = null;
		
		private Material mMaterial = null;

		public Material Material
		{
			get
			{
				if (mMaterial == null)
					mMaterial = GenerateMaterial(Shader);
				return mMaterial;
			}
		}

		//根据shader创建用于屏幕特效的材质
		private Material GenerateMaterial(Shader shader)
		{
			if (shader == null)
				return null;
			//需要判断shader是否支持
			if (shader.isSupported == false)
				return null;
			Material material = new Material(shader);
			material.hideFlags = HideFlags.DontSave;
			if (material)
				return material;
			return null;
		}


		//覆写OnRenderImage函数
		private void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			//仅仅当有材质的时候才进行后处理，如果_Material为空，不进行后处理
			if (Material)
			{
				//使用Material处理Texture，dest不一定是屏幕，后处理效果可以叠加的！
				Graphics.Blit(src, dest, Material);
			}
			else
			{
				//直接绘制
				Graphics.Blit(src, dest);
			}
		}
	}
}
/****************************************************************************
 * Copyright (c) 2017 liangxie
 * TODO:// read https://blogs.unity3d.com/cn/2015/06/03/il2cpp-internals-method-calls/ 
 * 
 * http://liangxiegame.com
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

namespace QFramework 
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	#if SLUA_SUPPORT
	using SLua;
	[CustomLuaClass]
	#endif
	public class PTVoidDelegate
	{
		public delegate void WithVoid();

		public delegate void WithGo(GameObject go);

		public delegate void WithParams(params object[] paramList);

		public delegate void WithEvent(BaseEventData data);

		public delegate void WithObj(Object obj);

		public delegate void WithBool(bool value);

		public delegate void WithString(string str);

		public delegate void WithGeneric<in T>(T value);

		public delegate void WithGeneric<in T, in K>(T t,K k);
	}

	#if SLUA_SUPPORT
	[CustomLuaClass]
	#endif
	public class PTBoolDelegate
	{
		public delegate bool WithVoid();
	}
}

/****************************************************************************
 * Copyright (c) 2020.10 liangxie
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

namespace QFramework
{
    // http://stackoverflow.com/questions/1171812/multi-key-dictionary-in-c
    public class MutableTuple<T1, T2> //FUCKING Unity: struct is not supported in Mono
    {
        public T1 Item1;
        public T2 Item2;

        public MutableTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override bool Equals(object obj)
        {
            MutableTuple<T1, T2> p = obj as MutableTuple<T1, T2>;
            if (obj == null) return false;

            if (Item1 == null)
            {
                if (p.Item1 != null) return false;
            }
            else
            {
                if (p.Item1 == null || !Item1.Equals(p.Item1)) return false;
            }

            if (Item2 == null)
            {
                if (p.Item2 != null) return false;
            }
            else
            {
                if (p.Item2 == null || !Item2.Equals(p.Item2)) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (Item1 != null)
                hash ^= Item1.GetHashCode();
            if (Item2 != null)
                hash ^= Item2.GetHashCode();
            return hash;
        }
    }
}
/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Text;

namespace BindKit.Binding.Paths
{
    [Serializable]
    public class PathToken
    {
        private Path path;
        private int pathIndex;

        private PathToken nextToken;
        public PathToken(Path path, int pathIndex)
        {
            this.path = path;
            this.pathIndex = pathIndex;
        }

        public Path Path
        {
            get { return this.path; }
        }

        public int Index { get { return this.pathIndex; } }

        public IPathNode Current
        {
            get { return path[pathIndex]; }
        }

        public bool HasNext()
        {
            if (path.Count <= 0 || this.pathIndex >= path.Count - 1)
                return false;
            return true;
        }

        public PathToken NextToken()
        {
            if (!HasNext())
                throw new IndexOutOfRangeException();

            if (this.nextToken == null)
                this.nextToken = new PathToken(path, pathIndex + 1);
            return this.nextToken;
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            this.Current.ToString();
            buf.Append(this.Current.ToString()).Append(" pathIndex:").Append(this.pathIndex);
            return buf.ToString();
        }
    }
}

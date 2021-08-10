/****************************************************************************
 * Copyright (c) 2017 Chris Foulston
 * 
 * https://github.com/cfoulston/Unity-Reorderable-List
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
    public class ReorderableAttribute : PropertyAttribute {

        public bool add;
        public bool remove;
        public bool draggable;
        public bool singleLine;
        public string elementNameProperty;
        public string elementNameOverride;
        public string elementIconPath;

        public ReorderableAttribute()
            : this(null) {
        }

        public ReorderableAttribute(string elementNameProperty)
            : this(true, true, true, elementNameProperty, null, null) {
        }

        public ReorderableAttribute(string elementNameProperty, string elementIconPath)
            : this(true, true, true, elementNameProperty, null, elementIconPath) {
        }

        public ReorderableAttribute(string elementNameProperty, string elementNameOverride, string elementIconPath)
            : this(true, true, true, elementNameProperty, elementNameOverride, elementIconPath) {
        }

        public ReorderableAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null, string elementIconPath = null) 
            : this(add, remove, draggable, elementNameProperty, null, elementIconPath) {
        }

        public ReorderableAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null, string elementNameOverride = null, string elementIconPath = null) {

            this.add = add;
            this.remove = remove;
            this.draggable = draggable;
            this.elementNameProperty = elementNameProperty;
            this.elementNameOverride = elementNameOverride;
            this.elementIconPath = elementIconPath;
        }
    }
}
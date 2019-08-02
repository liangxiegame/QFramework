// *************************************************************************************************
// The MIT License (MIT)
// 
// Copyright (c) 2016 Sean
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// *************************************************************************************************
// Project source: https://github.com/theoxuan/FlexiSocket

using System;

namespace QF
{
    /// <summary>
    /// Message structure type
    /// </summary>
    [Obsolete]
    public enum MessageStructure
    {
        /// <summary>
        /// Head+Body structure type
        /// </summary>
        /// <remarks>
        /// The message head is a 4-byte int type which represents the length of the coming message
        /// </remarks>
        LengthPrefixed,

        /// <summary>
        /// Body+TerminatTag structure type
        /// </summary>
        /// <remarks>
        /// The message tail is a user-defined string like <c>&lt;EOF&gt;</c> which represents the end of a string message
        /// </remarks>
        StringTerminated,

        /// <summary>
        /// Each packed is treated as a single message
        /// </summary>
        /// <remarks>
        /// For large messages, packets should be merged by user manually
        /// </remarks>
        Custom
    }
}
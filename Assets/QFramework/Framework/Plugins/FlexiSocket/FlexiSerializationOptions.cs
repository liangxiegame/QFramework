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
    /// Options applied to serialization
    /// </summary>
    [Flags]
    public enum FlexiSerializationOptions
    {
        /// <summary>
        /// Default option
        /// </summary>
        /// <remarks>
        /// Public properties/fields will be serialized
        /// <para></para>
        /// Public properties/fields with <see cref="FlexiMemberIgnoreAttribute"/> will not be serialized
        /// <para></para>
        /// Non-public properties/fields will not be serialized
        /// <para></para>
        /// Non-public properties/fields with <see cref="FlexiMemberIncludeAttribute"/> will be serialized
        /// </remarks>
        Default = Fields | Properties ,

        /// <summary>
        /// Fields will be serialized
        /// </summary>
        Fields = 1,

        /// <summary>
        /// Properties will be serialized
        /// </summary>
        Properties = 1 << 1,

        /// <summary>
        /// Only members declared by target type will be serialized
        /// </summary>
        DeclaredOnly = 1 << 2
    }
}
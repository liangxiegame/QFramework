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

namespace Loxodon.Log
{
    public static class LogManager
    {
        private static readonly DefaultLogFactory _defaultFactory = new DefaultLogFactory();
        private static ILogFactory _factory;

        public static DefaultLogFactory Default { get { return _defaultFactory; } }

        public static ILog GetLogger(Type type)
        {
            if (_factory != null)
                return _factory.GetLogger(type);

            return _defaultFactory.GetLogger(type);
        }

        public static ILog GetLogger(string name)
        {
            if (_factory != null)
                return _factory.GetLogger(name);

            return _defaultFactory.GetLogger(name);
        }

        public static void Registry(ILogFactory factory)
        {
            if (_factory != null && _factory != factory)
                throw new Exception("Don't register log factory many times");

            _factory = factory;
        }

    }
}

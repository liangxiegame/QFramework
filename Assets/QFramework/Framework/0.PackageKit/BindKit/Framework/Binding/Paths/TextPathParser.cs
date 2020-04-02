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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BindKit.Binding.Paths
{
    public class TextPathParser : IEnumerator<char>
    {
        protected string text;
        protected int total = 0;
        protected int pos = -1;
        protected Path path = null;
        public TextPathParser(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("Invalid argument", "text");

            this.text = text.Replace(" ","");
            if (string.IsNullOrEmpty(this.text) || this.text.StartsWith("."))
                throw new ArgumentException("Invalid argument", "text");

            this.total = this.text.Length;
            this.pos = -1;
        }

        public char Current
        {
            get { return this.text[pos]; }
        }

        object IEnumerator.Current
        {
            get { return this.text[pos]; }
        }

        public void Dispose()
        {
            this.text = null;
            this.pos = -1;
        }

        public bool MoveNext()
        {
            if (this.pos++ < this.total - 1)
                return true;
            return false;
        }

        public void Reset()
        {
            this.pos = -1;
        }

        protected bool IsEOF()
        {
            return this.pos >= this.total;
        }

        public Path Parse()
        {
            if (path != null)
                return path;

            path = new Path();
            this.MoveNext();
            do
            {
                this.SkipWhiteSpaceAndCharacters('.');

                if (this.IsEOF())
                    break;

                if (this.Current.Equals('['))
                {
                    //parse index
                    this.ReadIndex();
                    this.SkipWhiteSpace();
                    if (!this.Current.Equals(']'))
                        throw new BindingException("Error parsing indexer , unterminated in text {0}", this.text);

                    if (this.MoveNext())
                    {
                        if (!this.Current.Equals('.'))
                            throw new BindingException("Error parsing path , unterminated in text {0}", this.text);
                    }
                }
                else if (char.IsLetter(this.Current) || this.Current == '_')
                {
                    //parse member name
                    this.ParseMemberName();
                    if (!this.IsEOF() && !this.Current.Equals('.') && !this.Current.Equals('[') && !char.IsWhiteSpace(this.Current))
                        throw new BindingException("Error parsing path , unterminated in text {0}", this.text);
                }
                else
                {
                    throw new BindingException("Error parsing path , unterminated in text {0}", this.text);
                }
            } while (!this.IsEOF());
            return path;
        }

        protected void ReadIndex()
        {
            if (!this.MoveNext())
                throw new BindingException("Error parsing string indexer , unterminated in text {0}", this.text);

            var ch = this.Current;
            if (ch == '\'' || ch == '\"')
            {
                var index = this.ReadQuotedString();
                path.AppendIndexed(index);
                this.MoveNext();
                return;
            }

            if (char.IsDigit(ch))
            {
                uint index = this.ReadUnsignedInteger();
                path.AppendIndexed((int)index);
                return;
            }

            throw new BindingException("Error parsing indexer , unterminated in text {0}", this.text);
        }

        protected void ParseMemberName()
        {
            var buf = new StringBuilder();
            do
            {
                var ch = this.Current;
                if (!char.IsLetterOrDigit(ch) && ch != '_')
                    break;

                buf.Append(ch);

            } while (this.MoveNext());

            if (buf.Length <= 0)
                throw new BindingException("Error parsing member name , unterminated in text {0}", this.text);

            path.Append(new MemberNode(buf.ToString()));
        }

        protected uint ReadUnsignedInteger()
        {
            var buf = new StringBuilder();
            do
            {
                if (!char.IsDigit(this.Current))
                    break;

                buf.Append(this.Current);

            } while (this.MoveNext());

            uint index;
            if (!uint.TryParse(buf.ToString(), out index))
                throw new BindingException("Unable to parse integer text from {0} in {1}", buf.ToString(), this.text);
            return index;
        }

        protected string ReadQuotedString()
        {
            char ch = this.Current;
            if (ch != '\'' && ch != '\"')
                throw new BindingException("Error parsing string indexer , unexpected quote character {0} in text {1}",
                                       ch, this.text);

            if (!this.MoveNext())
                throw new BindingException("Error parsing string indexer , unterminated in text {0}", this.text);

            var buf = new StringBuilder();
            do
            {
                ch = this.Current;
                if (!char.IsLetterOrDigit(ch) && ch != '_' && ch != '-')
                    break;

                buf.Append(ch);
            } while (this.MoveNext());

            if (buf.Length <= 0 || (ch != '\'' && ch != '\"'))
                throw new BindingException("Error parsing string indexer , unexpected quote character {0} in text {1}",
                                       ch, this.text);
            return buf.ToString();
        }

        protected void SkipWhiteSpace()
        {
            while (char.IsWhiteSpace(this.Current) && this.MoveNext())
            {
            }
        }

        protected bool IsWhiteSpaceOrCharacter(char ch, params char[] characters)
        {
            return char.IsWhiteSpace(ch) || characters.Contains(ch);
        }

        protected void SkipWhiteSpaceAndCharacters(params char[] characters)
        {
            while (IsWhiteSpaceOrCharacter(this.Current, characters) && this.MoveNext())
            {
            }
        }
    }
}
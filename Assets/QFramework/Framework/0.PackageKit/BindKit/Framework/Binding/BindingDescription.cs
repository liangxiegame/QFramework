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
using BindKit.Binding.Converters;
using BindKit.Binding.Paths;
using BindKit.Binding.Proxy.Sources;
using BindKit.Binding.Proxy.Sources.Object;

namespace BindKit.Binding
{
    [Serializable]
    public class BindingDescription
    {
        public string TargetName { get; set; }

        public string UpdateTrigger { get; set; }

        public IConverter Converter { get; set; }

        public BindingMode Mode { get; set; }

        public SourceDescription Source { get; set; }

        public object CommandParameter { get; set; }

        public BindingDescription()
        {
        }

        public BindingDescription(string targetName, Path path, IConverter converter = null, BindingMode mode = BindingMode.Default)
        {
            this.TargetName = targetName;
            this.Mode = mode;
            this.Converter = converter;
            this.Source = new ObjectSourceDescription
            {
                Path = path
            };
        }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("{binding ").Append(this.TargetName);

            if (!string.IsNullOrEmpty(this.UpdateTrigger))
                buf.Append(" UpdateTrigger:").Append(this.UpdateTrigger);

            if (this.Converter != null)
                buf.Append(" Converter:").Append(this.Converter.GetType().Name);

            if (this.Source != null)
                buf.Append(" ").Append(this.Source.ToString());

            if (this.CommandParameter != null)
                buf.Append(" CommandParameter:").Append(this.CommandParameter);

            buf.Append(" Mode:").Append(this.Mode.ToString());
            buf.Append(" }");
            return buf.ToString();
        }
    }
}
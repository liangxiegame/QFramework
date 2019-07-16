using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CI.HttpClient
{
    public class StringContent : IHttpContent
    {
        private const string DEFAULT_MEDIA_TYPE = "text/plain";

        private readonly string _content;
        private readonly Encoding _encoding;

        private byte[] _serialisedContent;

        public ContentReadAction ContentReadAction
        {
            get { return ContentReadAction.ByteArray; }
        }

        /// <summary>
        /// Not currently implemented
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Send content encoded as a string, the encoding will default to UTF-8 and the media type text/plain
        /// </summary>
        /// <param name="content">The string to send</param>
        public StringContent(string content)
            : this(content, Encoding.UTF8, DEFAULT_MEDIA_TYPE)
        {
        }

        /// <summary>
        /// Send content encoded as a string with the specified encoding, the media type will default to text/plain
        /// </summary>
        /// <param name="content">The string to send</param>
        /// <param name="encoding">The encoding of the string</param>
        public StringContent(string content, Encoding encoding)
            : this(content, encoding, DEFAULT_MEDIA_TYPE)
        {
        }

        /// <summary>
        /// Send content encoded as a string with the specified encoding, the specified mediaType sets the Content Type header
        /// </summary>
        /// <param name="content">The string to send</param>
        /// <param name="encoding">The encoding of the string</param>
        /// <param name="mediaType">The media type</param>
        public StringContent(string content, Encoding encoding, string mediaType)
        {
            _content = content;
            _encoding = encoding;
            Headers = new Dictionary<string, string>();
            Headers.Add("Content-Type", mediaType + "; charset=" + _encoding.WebName);
        }

        public long GetContentLength()
        {
            return ReadAsByteArray().Length;
        }

        public string GetContentType()
        {
            if (Headers.ContainsKey("Content-Type"))
            {
                return Headers["Content-Type"];
            }

            return string.Empty;
        }

        public byte[] ReadAsByteArray()
        {
            if(_serialisedContent == null)
            {
                _serialisedContent = _encoding.GetBytes(_content);
            }

            return _serialisedContent;
        }

        public Stream ReadAsStream()
        {
            throw new NotImplementedException();
        }
    }
}
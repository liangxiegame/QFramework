using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CI.HttpClient
{
    public class FormUrlEncodedContent : IHttpContent
    {
        private readonly IEnumerable<KeyValuePair<string, string>> _nameValueCollection;

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
        /// Send content encoded as name/value pairs, the Content Type header will be set to application/x-www-form-urlencoded
        /// </summary>
        /// <param name="nameValueCollection">The key/value pairs to send</param>
        public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            _nameValueCollection = nameValueCollection;
            Headers = new Dictionary<string, string>();
            Headers.Add("Content-Type", "application/x-www-form-urlencoded");
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
            if (_serialisedContent == null)
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (KeyValuePair<string, string> nameValue in _nameValueCollection)
                {
                    UrlEncoded(stringBuilder, nameValue.Key, nameValue.Value);
                }

                _serialisedContent = Encoding.ASCII.GetBytes(stringBuilder.ToString());
            }

            return _serialisedContent;
        }

        public Stream ReadAsStream()
        {
            throw new NotImplementedException();
        }

        private void UrlEncoded(StringBuilder sb, string name, string value)
        {
            if (sb.Length != 0)
                sb.Append("&");
            sb.Append(Uri.EscapeUriString(name));
            sb.Append("=");
            sb.Append(Uri.EscapeUriString(value));
        }
    }
}
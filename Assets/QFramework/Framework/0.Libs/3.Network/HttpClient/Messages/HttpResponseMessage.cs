using System;
using System.Net;

namespace CI.HttpClient
{
    /// <summary>
    /// Represents a HTTP response message including the status code and data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpResponseMessage<T>
    {
        /// <summary>
        /// The original request
        /// </summary>
        public HttpWebRequest OriginalRequest
        {
            get; set;
        }

        /// <summary>
        /// The original response
        /// </summary>
        public HttpWebResponse OriginalResponse
        {
            get; set;
        }

        /// <summary>
        /// Data that has been read from the server
        /// </summary>
        public T Data
        {
            get; set;
        }

        /// <summary>
        /// Length of the content being downloaded
        /// </summary>
        public long ContentLength
        {
            get; set;
        }

        /// <summary>
        /// How much content as been downloaded so far
        /// </summary>
        public long TotalContentRead
        {
            get; set;
        }

        /// <summary>
        /// How much content has been downloaded since the last http response message was raised
        /// </summary>
        public long ContentReadThisRound
        {
            get; set;
        }

        /// <summary>
        /// Percentage completion of the download
        /// </summary>
        public int PercentageComplete
        {
            get
            {
                if (ContentLength <= 0)
                {
                    return 100;
                }
                else
                {
                    return (int)(((double)TotalContentRead / ContentLength) * 100);
                }
            }
        }

        /// <summary>
        /// The http status code
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get; set;
        }

        /// <summary>
        /// The reason for the http status code
        /// </summary>
        public string ReasonPhrase
        {
            get; set;
        }

        /// <summary>
        /// Can the status code be considered a success code
        /// </summary>
        public bool IsSuccessStatusCode
        {
            get { return ((int)StatusCode >= 200) && ((int)StatusCode <= 299); }
        }

        /// <summary>
        /// The exception raised (if there was one)
        /// </summary>
        public Exception Exception
        {
            get; set;
        }
    }
}
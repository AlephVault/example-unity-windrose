using System;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;

namespace ResourceServers
{
    public static class JSON
    {
        /// <summary>
        ///   This exception is the base exception class
        ///     for errors occurring on a JSON request.
        /// </summary>
        public class Exception : System.Exception
        {
            public Exception() : base() {}
            public Exception(string message) : base(message) {}
        }

        /// <summary>
        ///   This exception is thrown when a JSON request
        ///     returns a status code not in 2xx.
        /// </summary>
        public class HTTPError : Exception
        {
            public long Code { get; private set; }
            public byte[] Content { get; private set; }
            public HTTPError(long code, byte[] content) : base() {
                Code = code;
                Content = content;
            }
        }

        /// <summary>
        ///   This exception is thrown when a JSON request
        ///     does not conclude due to a networking error.
        /// </summary>
        public class NetworkError : Exception
        {
            public NetworkError(string message) : base(message) {}
        }

        /// <summary>
        ///   This exception is thrown when a successful
        ///     result is, however, not of JSON format.
        /// </summary>
        public class ExpectedJSON : Exception {}

        /// <summary>
        ///   Attempts an HTTP request, and could raise network,
        ///     http or format errors.
        /// </summary>
        /// <param name="url">The URL to fetch data from</param>
        /// <param name="process">A callback to process the retrieved data</param>
        public static IEnumerable Fetch(string url, Func<string, IEnumerable> process)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                throw new NetworkError(www.error);
            }
            else if (www.isHttpError)
            {
                throw new HTTPError(www.responseCode, www.downloadHandler.data);
            }
            else if (!www.GetResponseHeader("Content-Type").StartsWith("application/json"))
            {
                throw new ExpectedJSON();
            }
            else
            {
                foreach(var result in process(www.downloadHandler.text))
                {
                    yield return result;
                }
            }
        }

        /// <summary>
        ///   Parses a JSON structure into an instance of a class.
        /// </summary>
        /// <typeparam name="T">The type to use as parse target</typeparam>
        /// <param name="text">The text to parse</param>
        /// <returns>A parsed object</returns>
        public static T Parse<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
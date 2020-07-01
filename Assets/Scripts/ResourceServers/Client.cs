using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ResourceServers
{
    /// <summary>
    ///   Provides a method to fetch data from a json-returning
    ///     root URL (the resources server), and populating a
    ///     given registry (according to the server's version).
    /// </summary>
    public static class Client
    {
        /// <summary>
        ///   This class is just a version picker for the JSON's
        ///     body (it will serve to get the appropriate
        ///     loader later).
        /// </summary>
        private class WithVersion
        {
            public string Version { get; set; }
        }

        /// <summary>
        ///   This exception is raised when an unsupported
        ///     version is found in the resources root.
        /// </summary>
        public class UnsupportedVersion : System.Exception
        {
            public string Version { get; private set; }
            public UnsupportedVersion(string version) : base("Unsupported version: " + version) {
                Version = version;
            }
        }

        /// <summary>
        ///   Fetches data from a JSON root in a resources
        ///     server
        /// </summary>
        /// <param name="url"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable Fetch(string url, Registries.Registry target)
        {
            foreach(var result in JSON.Fetch(url, delegate(string data) { return fetched(url, data, target); }))
            {
                yield return result;
            }
        }

        private static string GetVersion(string body)
        {
            return "2"; // TODO
        }

        private static IEnumerable fetched(string baseUrl, string data, Registries.Registry target)
        {

            string version = GetVersion(data);
            switch(version)
            {
                case "2":
                    return new Loaders.V2.Loader().Populate(baseUrl, data, target);
                default:
                    throw new UnsupportedVersion(version);
            }
        }
    }
}

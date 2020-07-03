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
        ///     loader later). Either "version" or "Version"
        ///     will be used, for V1 was lowercased in the keys.
        /// </summary>
        private class WithVersion
        {
            public string Version { get; set; }
            public string version { get; set; }
            public string GetVersion()
            {
                return version != "" ? version : Version;
            }
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
        ///     server, according to the version and using
        ///     the appropriate loader.
        /// </summary>
        /// <param name="url">The root url to fetch</param>
        /// <param name="target">The registry to populate</param>
        /// <returns>An enumerable, suitable for coroutines</returns>
        public static IEnumerable Fetch(string url, Registries.Registry target)
        {
            foreach(var result in JSON.Fetch(url, delegate(string body) { return fetched(url, body, target); }))
            {
                yield return result;
            }
        }

        private static string GetVersion(string body)
        {
            return JSON.Parse<WithVersion>(body).GetVersion();
        }

        private static Loaders.Loader GetLoader(string body)
        {
            string version = GetVersion(body);
            switch (version)
            {
                case "2":
                    return new Loaders.V2.Loader();
                default:
                    throw new UnsupportedVersion(version);
            }
        }

        private static IEnumerable fetched(string baseUrl, string body, Registries.Registry target)
        {
            return GetLoader(body).Populate(baseUrl, body, target);
        }
    }
}

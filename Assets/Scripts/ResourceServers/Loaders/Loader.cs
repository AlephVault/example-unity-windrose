using System.Collections;
using UnityEngine;

namespace ResourceServers
{
    namespace Loaders
    {
        /// <summary>
        ///   A loader is picked by a client, and each version
        ///     will have a different loader. Each loader is
        ///     compatible with a class of <see cref="Registries.Registry" />
        ///     (or perhaps more of them) and will try to
        ///     populate it given the definition of the root
        ///     path of the resources server (/).
        /// </summary>
        public abstract class Loader
        {
            /// <summary>
            ///   Populates a registry given a root json body.
            ///   The body will have the appropriate version,
            ///     and the loader will be able to traverse
            ///     it as long as it is well-formed. The target
            ///     registry must be not null and also must be
            ///     compatible with what the loader is able
            ///     to fill from a well-formed JSON structure
            ///     of resources.
            /// </summary>
            /// <param name="baseUrl">The base url for the root request</param>
            /// <param name="jsonBody">A root-retrieved JSON body</param>
            /// <param name="target">The target registry to populate</param>
            public abstract IEnumerable Populate(string baseUrl, string jsonBody, Registries.Registry target);
        }
    }
}


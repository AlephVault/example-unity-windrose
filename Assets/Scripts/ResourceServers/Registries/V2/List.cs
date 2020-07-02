using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceServers
{
    namespace Registries
    {
        namespace V2
        {
            /// <summary>
            ///   Lists have to be implemented to return an
            ///     arbitrary Unity object given an id. If
            ///     somehow this fails (e.g. invalid ID)
            ///     then a V2.Registry.FetchError must be
            ///     raised.
            /// </summary>
            public abstract class List : ScriptableObject
            {
                public abstract Object Find(ulong id);
            }
        }
    }
}

using AlephVault.Unity.Meetgard.Scopes.Types.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Server
            {
                public partial class ScopeServerSide : MonoBehaviour
                {
                    // These methods are internal and are invoked in the context
                    // of the global server - never by this scope on itself. This
                    // also means these methods will be invoked in the context of
                    // a queued task in the main global server.

                    // The set of connections in the current scope. They will be
                    // also managed in the context of the server.
                    internal HashSet<ulong> connections = new HashSet<ulong>();

                    /// <summary>
                    ///   Initializes the scope. Typically, this invokes
                    ///   registered callbacks to work. This method does
                    ///   not rely on PrefabId, ID, and Protocol values.
                    /// </summary>
                    internal async Task Load()
                    {
                        // TODO implement.
                        throw new NotImplementedException();
                    }

                    /// <summary>
                    ///   Finalizes the scope. Typically, this invokes
                    ///   registered callbacks to work. This method does
                    ///   not rely on PrefabID, ID, and Protocol values.
                    /// </summary>
                    internal async Task Unload()
                    {
                        // TODO implement.
                        throw new NotImplementedException();
                    }
                }
            }
        }
    }
}

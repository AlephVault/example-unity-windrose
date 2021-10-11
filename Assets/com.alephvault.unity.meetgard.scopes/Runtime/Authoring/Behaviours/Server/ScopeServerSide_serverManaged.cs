using AlephVault.Unity.Support.Utils;
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
                    ///   This invocation is queued in the per-scope
                    ///   async queue.
                    /// </summary>
                    internal async Task Load()
                    {
                        await queueManager.QueueTask(async () => {
                            await (OnLoad?.InvokeAsync() ?? Task.CompletedTask);
                        });
                    }

                    /// <summary>
                    ///   Finalizes the scope. Typically, this invokes
                    ///   registered callbacks to work. This method does
                    ///   not rely on PrefabID, ID, and Protocol values.
                    ///   This invocation is queued in the per-scope
                    ///   async queue.
                    /// </summary>
                    internal async Task Unload()
                    {
                        await queueManager.QueueTask(async () => {
                            await (OnUnload?.InvokeAsync() ?? Task.CompletedTask);
                        });
                    }

                    /// <summary>
                    ///   Triggers the <see cref="OnJoining"/> event.
                    ///   A default implementation will synchronize
                    ///   all the existing objects into the connection.
                    ///   This invocation is queued in the per-scope
                    ///   async queue.
                    /// </summary>
                    /// <param name="connectionId">The id of the joining connection</param>
                    internal async Task TriggerOnJoining(ulong connectionId)
                    {
                        await queueManager.QueueTask(async () => {
                            await (OnJoining?.InvokeAsync(connectionId) ?? Task.CompletedTask);
                        });
                    }

                    /// <summary>
                    ///   Triggers the <see cref="OnLeaving"/> event.
                    ///   This invocation is queued in the per-scope
                    ///   async queue.
                    /// </summary>
                    internal async Task TriggerOnLeaving(ulong connectionId)
                    {
                        await queueManager.QueueTask(async () => {
                            await (OnLeaving?.InvokeAsync(connectionId) ?? Task.CompletedTask);
                        });
                    }

                    /// <summary>
                    ///   Triggers the <see cref="OnGoodBye"/> event.
                    ///   This invocation is queued in the per-scope
                    ///   async queue.
                    /// </summary>
                    internal async Task TriggerOnGoodBye(ulong connectionId)
                    {
                        await queueManager.QueueTask(async () => {
                            await (OnGoodBye?.InvokeAsync(connectionId) ?? Task.CompletedTask);
                        });
                    }
                }
            }
        }
    }
}

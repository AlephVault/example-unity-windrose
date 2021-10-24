using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Scopes.Types.Constants;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols.Messages;
using AlephVault.Unity.Support.Utils;
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
                public partial class ScopesProtocolServerSide : ProtocolServerSide<ScopesProtocolDefinition>
                {
                    // A reverse map between a connection id and the scope it belongs to.
                    // Virtual / reserved scopes can be specified here.
                    private Dictionary<ulong, uint> scopeForConnection = new Dictionary<ulong, uint>();

                    /// <summary>
                    ///   This event is triggered when a new connection arrived and was
                    ///   set to the limbo in that moment.
                    /// </summary>
                    public event Func<ulong, Task> OnWelcome = null;

                    /// <summary>
                    ///   This event is triggered when a connection leaves a scope.
                    ///   Sent when changing scope (leaving the current one), even
                    ///   if the scope is virtual/reserved. A default implementation
                    ///   will exist: to notify the scope if non-reserved, and to
                    ///   remove the connection from the scope (also, ig non-reserved).
                    /// </summary>
                    public event Func<ulong, uint, Task> OnLeavingScope = null;

                    /// <summary>
                    ///   This event is triggered when a connection joins a scope.
                    ///   Sent when changing scope (joining a new one), even if
                    ///   the scope is virtual/reserved. A default implementation
                    ///   will exist: to add the connection to the scope (if
                    ///   non-reserved), and to notify the scope (also, if
                    ///   non-reserved).
                    /// </summary>
                    public event Func<ulong, uint, Task> OnJoiningScope = null;

                    /// <summary>
                    ///   This event is triggered when a connection left the game.
                    ///   A default implementation will exist: to remove the connection
                    ///   and to notify the scope (if non-reserved).
                    /// </summary>
                    public event Func<ulong, uint, Task> OnGoodBye = null;

                    // Clears all of the connections in a given scope.
                    private async Task ClearConnectionsFromScope(ScopeServerSide scope)
                    {
                        // Create the limbo message.
                        var message = new MovedToScope()
                        {
                            PrefabIndex = Scope.LimboPrefab,
                            ScopeIndex = Scope.Limbo
                        };
                        // Then send it to each connection in the scope.
                        foreach (ulong connection in scope.connections)
                        {
                            scopeForConnection[connection] = Scope.Limbo;
                            try
                            {
                                await UntilSendIsDone(SendMovedToScope(connection, message));
                            }
                            catch { /* Diaper-ignore */ }
                        }
                        scope.connections.Clear();
                    }

                    // Default implementation for the OnLeavingScope event.
                    private async Task DefaultOnLeavingScope(ulong connectionId, uint scopeId)
                    {
                        // There is no notification to send here.
                        switch(scopeId)
                        {
                            case Scope.Limbo:
                            case Scope.Maintenance:
                                break;
                            default:
                                if (loadedScopes.TryGetValue(scopeId, out ScopeServerSide scope))
                                {
                                    scope.connections.Remove(connectionId);
                                    await scope.TriggerOnLeaving(connectionId);
                                };
                                break;
                        }
                    }

                    // Default implementation for the OnJoiningScope event.
                    private async Task DefaultOnJoiningScope(ulong connectionId, uint scopeId)
                    {
                        // There is no notification to send here.
                        switch (scopeId)
                        {
                            case Scope.Limbo:
                            case Scope.Maintenance:
                                break;
                            default:
                                if (loadedScopes.TryGetValue(scopeId, out ScopeServerSide scope)) {
                                    scope.connections.Add(connectionId);
                                    await UntilSendIsDone(SendMovedToScope(connectionId, new MovedToScope() { PrefabIndex = scopePrefabId, ScopeIndex = currentScopeId }));
                                    await scope.TriggerOnJoining(connectionId);
                                };
                                break;
                        }
                    }

                    // Default implementation for the OnGoodBye event.
                    // Yes: It also takes the current scope id, but must
                    // depart silently in that case.
                    private async Task DefaultOnGoodBye(ulong connectionId, uint scopeId)
                    {
                        // There is no notification to send here.
                        switch (scopeId)
                        {
                            case Scope.Limbo:
                            case Scope.Maintenance:
                                break;
                            default:
                                if (loadedScopes.TryGetValue(scopeId, out ScopeServerSide scope))
                                {
                                    scope.connections.Remove(connectionId);
                                    await scope.TriggerOnGoodBye(connectionId);
                                };
                                break;
                        }
                    }

                    /// <summary>
                    ///   Sends a connection, which must exist, to another scope.
                    ///   It is removed from the current scope (it will have one),
                    ///   and added to a new scope.
                    /// </summary>
                    /// <param name="connectionId">The id of the connection being moved</param>
                    /// <param name="newScope">The new scope to move the connection to</param>
                    /// <param name="force">Whether to execute the logic, even if the scope is the same</param>
                    public Task SendTo(ulong connectionId, uint newScope, bool force = false)
                    {
                        return queueManager.QueueTask(async () => {
                            uint scopePrefabId;
                            try
                            {
                                switch(newScope)
                                {
                                    case Scope.Limbo:
                                        scopePrefabId = Scope.LimboPrefab;
                                        break;
                                    case Scope.Maintenance:
                                        scopePrefabId = Scope.Maintenance;
                                        break;
                                    default:
                                        scopePrefabId = loadedScopes[newScope].PrefabId;
                                        break;
                                }
                            }
                            catch(KeyNotFoundException)
                            {
                                throw new InvalidOperationException("The specified new scope does not exist");
                            }

                            uint currentScopeId;
                            try
                            {
                                currentScopeId = scopeForConnection[connectionId];
                            }
                            catch(KeyNotFoundException)
                            {
                                throw new InvalidOperationException("The specified connection does not exist");
                            }

                            if (force || currentScopeId != newScope)
                            {
                                await (OnLeavingScope?.InvokeAsync(connectionId, currentScopeId, async (e) => {
                                    Debug.LogError(
                                        $"An error of type {e.GetType().FullName} has occurred in server side's OnLeavingScope event. " +
                                        $"If the exceptions are not properly handled, the game state might be inconsistent. " +
                                        $"The exception details are: {e.Message}"
                                    );
                                }) ?? Task.CompletedTask);
                                scopeForConnection[connectionId] = newScope;
                                await (OnJoiningScope?.InvokeAsync(connectionId, currentScopeId, async (e) => {
                                    Debug.LogError(
                                        $"An error of type {e.GetType().FullName} has occurred in server side's OnJoiningScope event. " +
                                        $"If the exceptions are not properly handled, the game state might be inconsistent. " +
                                        $"The exception details are: {e.Message}"
                                    );
                                }) ?? Task.CompletedTask);
                            }
                        });
                    }

                    /// <summary>
                    ///   Sends a connection, which must exist, to the
                    ///   limbo. This task is queued.
                    /// </summary>
                    /// <param name="connectionId">The id of the connection being moved</param>
                    public Task SendToLimbo(ulong connectionId)
                    {
                        return SendTo(connectionId, Scope.Limbo);
                    }
                }
            }
        }
    }
}
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Scopes.Types.Constants;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols.Messages;
using AlephVault.Unity.Support.Authoring.Behaviours;
using System;
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
                /// <summary>
                ///   The server side implementation of the scopes-managing protocol.
                ///   It manages the scopes that has to load, the loaded scopes, the
                ///   current connections and methods to synchronize objects and manage
                ///   all of the notifications and stuff between this class and other
                ///   classes in the package, like (scoped) objects.
                /// </summary>
                [RequireComponent(typeof(AsyncQueueManager))]
                public partial class ScopesProtocolServerSide : ProtocolServerSide<ScopesProtocolDefinition>
                {
                    // The queue management dependency.
                    private AsyncQueueManager queueManager;

                    /// <summary>
                    ///   List of the prefabs that will be used to instantiate scopes.
                    ///   This list is mapped 1:1 with the scopes they instantiate,
                    ///   being those stored in <see cref="defaultScopes"/>. Clients
                    ///   must register corresponding prefabs in an 1:1 basis on their
                    ///   sides for things go ok with synchronization.
                    /// </summary>
                    [SerializeField]
                    private ScopeServerSide[] defaultScopePrefabs;

                    /// <summary>
                    ///   List of prefabs that will be used dynamically to instantiate
                    ///   scopes. This list is not mapped 1:1, but dynamically: scopes
                    ///   for the prefabs listed here may be arbitrarily added and/or
                    ///   removed from the game (with proper dispose mechanics). Clients
                    ///   must register corresponding prefabs in an 1:1 basis on their
                    ///   sides for things go ok with synchronization.
                    /// </summary>
                    [SerializeField]
                    private ScopeServerSide[] extraScopePrefabs;

                    protected override void Initialize()
                    {
                        // The max ID of a scope is {Scope.MaxScopes - 1}. The minimum ID is 1.
                        // This means that the maximum amount of default scopes needs to be
                        // corrected by -1.
                        if (defaultScopePrefabs.Length > Scope.MaxScopes - 1)
                        {
                            // TODO exception.
                        }
                        // Aside from the default prefabs, extra prefabs may be registered.
                        // In this case, the prefab id is given, and the ID of the scope will
                        // be between {defaultScopePrefabs.Length} and {Scope.MaxScopes - 1}.
                        if (extraScopePrefabs.Length > Scope.MaxScopePrefabs)
                        {
                            // TODO exception.
                        }
                        queueManager = GetComponent<AsyncQueueManager>();
                        base.Initialize();
                        WorldLoadStatus = LoadStatus.Empty;
                        SendWelcome = MakeSender("Welcome");
                        SendMovedToScope = MakeSender<MovedToScope>("MovedToScope");
                        SendObjectSpawned = MakeSender<ObjectSpawned>("ObjectSpawned");
                        BroadcastObjectSpawned = MakeBroadcaster<ObjectSpawned>("ObjectSpawned");
                        BroadcastObjectDespawned = MakeBroadcaster<ObjectDespawned>("ObjectDespawned");
                        SendObjectRefreshed = MakeSender<ObjectRefreshed>("ObjectRefreshed");
                        SendFocusChanged = MakeSender<FocusChanged>("FocusChanged");
                        SendFocusReleased = MakeSender<FocusReleased>("FocusReleased");
                    }

                    /// <summary>
                    ///   Sets a handling on the incoming messages from the client. Here,
                    ///   only LocalError matters, and will cause the connection to close.
                    ///   On the other side, the client connection will be doing exactly
                    ///   the same in this case: closing the connection on their side.
                    /// </summary>
                    protected override void SetIncomingMessageHandlers()
                    {
                        AddIncomingMessageHandler("LocalError", async (proto, connectionId) =>
                        {
                            // The connection will be closed. Further logic will be
                            // handled on connection close.
                            server.Close(connectionId);
                        });
                    }

                    /// <summary>
                    ///   Handles the server start. This, among other potential things,
                    ///   starts the whole game world.
                    /// </summary>
                    public override async Task OnServerStarted()
                    {
                        // TODO Anything else here?
                        await LoadWorld();
                        // TODO Anything else here?
                    }

                    /// <summary>
                    ///   Handles what happens when the client connection is started.
                    ///   Typically, this will start and install the connection into
                    ///   the limbo scope, and prepare it to be ready to interact
                    ///   with the whole system (e.g. be able to remove it and put
                    ///   it in another scope, and stuff like that.
                    /// </summary>
                    /// <param name="clientId">the connection being started.</param>
                    public override async Task OnConnected(ulong clientId)
                    {
                        await SendWelcome(clientId);
                        // TODO implement appropriate connect logic for a connection.
                    }

                    /// <summary>
                    ///   Handles what happens when the client connection is terminated.
                    ///   Typically, this will cleanup any user interaction (i.e. as it
                    ///   happens: the connection has been closed). In this case, this
                    ///   connection will be removed from every scope and also will trigger
                    ///   some events that are related to this (e.g. unwatching objects, or
                    ///   for some games: removing owned objects and backing them up).
                    /// </summary>
                    /// <param name="clientId">The connection being closed</param>
                    /// <param name="reason">The disconnection reason</param>
                    public override async Task OnDisconnected(ulong clientId, Exception reason)
                    {
                        // TODO Implement appropriate disconnect logic for a connection
                        // TODO that terminated for any reason.
                    }

                    /// <summary>
                    ///   Handles the server stop. This, among other potential things,
                    ///   stops the whole game world.
                    /// </summary>
                    public override async Task OnServerStopped(Exception e)
                    {
                        // TODO Anything else here (specially using e)?
                        await UnloadWorld();
                        // TODO Anything else here (specially using e)?
                    }
                }
            }
        }
    }
}
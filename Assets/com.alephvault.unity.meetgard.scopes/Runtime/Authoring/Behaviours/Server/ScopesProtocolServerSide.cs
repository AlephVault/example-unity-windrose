using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols.Messages;
using AlephVault.Unity.Support.Authoring.Behaviours;
using AlephVault.Unity.Support.Utils;
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

                    /// <summary>
                    ///   The interval to wait between each check for the appropriate
                    ///   world load status.
                    /// </summary>
                    [SerializeField]
                    private float worldLoadStatusWaitInterval = 0.5f;

                    protected override void Initialize()
                    {
                        queueManager = GetComponent<AsyncQueueManager>();
                        base.Initialize();
                        worldLoadStatusWaitInterval = Values.Max(0.1f, worldLoadStatusWaitInterval);
                        WorldLoadStatus = Types.Constants.LoadStatus.Empty;
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

                    public override async Task OnConnected(ulong clientId)
                    {
                        await SendWelcome(clientId);
                        // TODO implement appropriate connect logic for a connection.
                    }

                    /// <summary>
                    ///   Handlers what happens when the client connection is terminated.
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
                }
            }
        }
    }
}
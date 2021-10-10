﻿using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols;
using AlephVault.Unity.Meetgard.Scopes.Types.Protocols.Messages;
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
                    // A sender for the Welcome message.
                    private Func<ulong, Task> SendWelcome;

                    // A sender for the MovedToScope message.
                    private Func<ulong, MovedToScope, Task> SendMovedToScope;

                    // A sender for the ObjectSpawned message.
                    // Use case: when a new connection arrives, for each object.
                    private Func<ulong, ObjectSpawned, Task> SendObjectSpawned;

                    // A broadcaster for the ObjectSpawned message.
                    // Use case: when a new object spawns, for each connection.
                    private Func<IEnumerable<ulong>, ObjectSpawned, Dictionary<ulong, Task>> BroadcastObjectSpawned;

                    // A broadcaster for the ObjectRefreshed message.
                    // Use case: when a new connection requests refresh, for each object.
                    private Func<ulong, ObjectRefreshed, Task> SendObjectRefreshed;

                    // A broadcaster for the ObjectDespawned message.
                    // Use case: when an object despawns, for each connection.
                    private Func<IEnumerable<ulong>, ObjectDespawned, Dictionary<ulong, Task>> BroadcastObjectDespawned;

                    // A sender for the FocusChanged message.
                    private Func<ulong, FocusChanged, Task> SendFocusChanged;

                    // A sender for the FocusReleased message.
                    private Func<ulong, FocusReleased, Task> SendFocusReleased;
                }
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.NetRose.Authoring.Behaviours.Server;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Samples
    {
        namespace Common
        {
            namespace Protocols
            {
                [RequireComponent(typeof(NetRoseProtocolServerSide))]
                public class ClientMovementProtocolServerSide : ProtocolServerSide<ClientMovementProtocolDefinition>
                {
                    private class ObjectOwnage
                    {
                        public float LastCommandTime = 0;
                        public ObjectServerSide OwnedObject = null;
                    }

                    private Dictionary<ulong, ObjectOwnage> objects = new Dictionary<ulong, ObjectOwnage>();

                    public override async Task OnConnected(ulong clientId)
                    {
                        ObjectServerSide obj = null;
                        // TODO Initialize the object appropriately (index=1 or index=2).
                        // TODO Set the owner to this connection.
                        // TODO Spawn it at (8, 6) in map at [0] in default scope at [4].
                        objects[clientId] = new ObjectOwnage() { LastCommandTime = 0, OwnedObject = obj };
                    }

                    public override async Task OnDisconnected(ulong clientId, Exception reason)
                    {
                        if (objects.TryGetValue(clientId, out ObjectOwnage ownage))
                        {
                            objects.Remove(clientId);
                            // It will de-spawn and destroy the object.
                            Destroy(ownage.OwnedObject);
                        }
                    }

                    protected override void SetIncomingMessageHandlers()
                    {
                        AddIncomingMessageHandler("Move:Down", async (proto, connectionId) => {
                            // TODO throttled move.
                        });
                        AddIncomingMessageHandler("Move:Left", async (proto, connectionId) => {
                            // TODO throttled move.
                        });
                        AddIncomingMessageHandler("Move:Right", async (proto, connectionId) => {
                            // TODO throttled move.
                        });
                        AddIncomingMessageHandler("Move:Up", async (proto, connectionId) => {
                            // TODO throttled move.
                        });
                    }
                }
            }
        }
    }
}
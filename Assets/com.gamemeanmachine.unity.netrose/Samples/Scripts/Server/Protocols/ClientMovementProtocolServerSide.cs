using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Server;
using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.NetRose.Authoring.Behaviours.Server;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects;
using GameMeanMachine.Unity.WindRose.Types;
using GameMeanMachine.Unity.NetRose.Samples.Common.Protocols;

namespace GameMeanMachine.Unity.NetRose
{
    namespace Samples
    {
        namespace Server
        {
            namespace Protocols
            {
                [RequireComponent(typeof(NetRoseProtocolServerSide))]
                public class ClientMovementProtocolServerSide : ProtocolServerSide<ClientMovementProtocolDefinition>
                {
                    private class ObjectOwnage
                    {
                        public float LastCommandTime = 0;
                        public INetRoseModelServerSide OwnedObject = null;
                    }

                    private Dictionary<ulong, ObjectOwnage> objects = new Dictionary<ulong, ObjectOwnage>();

                    [SerializeField]
                    private int char1index = 1;

                    [SerializeField]
                    private int char2index = 2;

                    private ScopesProtocolServerSide ScopesProtocolServerSide;

                    protected override void Setup()
                    {
                        ScopesProtocolServerSide = GetComponent<ScopesProtocolServerSide>();
                    }

                    public override async Task OnServerStarted()
                    {
                        objects = new Dictionary<ulong, ObjectOwnage>();
                    }

                    public override async Task OnServerStopped(System.Exception e)
                    {
                        objects = null;
                    }

                    public override async Task OnConnected(ulong clientId)
                    {
                        // Which index to take the character from?
                        int index = (clientId % 2 == 0) ? char1index : char2index;
                        // Instantiate it.
                        ObjectServerSide obj = ScopesProtocolServerSide.InstantiateHere((uint)index);
                        // Get the netrose component of it.
                        OwnableModelServerSide ownableObj = obj.GetComponent<OwnableModelServerSide>();
                        // Give it the required connection id.
                        ownableObj.ConnectionId = clientId;
                        // Attach it to a map.
                        ownableObj.MapObject.Attach(
                            ScopesProtocolServerSide.LoadedScopes[4].GetComponent<Scope>()[0],
                            8, 6, true
                        );
                        // Add it to the dictionary.
                        objects[clientId] = new ObjectOwnage() { LastCommandTime = 0, OwnedObject = obj.GetComponent< INetRoseModelServerSide>() };
                    }

                    public override async Task OnDisconnected(ulong clientId, System.Exception reason)
                    {
                        if (objects.TryGetValue(clientId, out ObjectOwnage ownage))
                        {
                            objects.Remove(clientId);
                            // It will de-spawn and destroy the object.
                            Destroy(ownage.OwnedObject.MapObject);
                        }
                    }

                    private void DoThrottled(ulong connectionId, Action<MapObject> callback)
                    {
                        ObjectOwnage ownage = objects[connectionId];
                        MapObject obj = ownage.OwnedObject.MapObject;
                        float time = Time.time;
                        if (ownage.LastCommandTime + obj.Speed * 0.75 <= time)
                        {
                            ownage.LastCommandTime += time;
                            callback(obj);
                        }
                    }

                    protected override void SetIncomingMessageHandlers()
                    {
                        AddIncomingMessageHandler("Move:Down", async (proto, connectionId) => {
                            DoThrottled(connectionId, (obj) =>
                            {
                                obj.StartMovement(Direction.DOWN);
                            });
                        });
                        AddIncomingMessageHandler("Move:Left", async (proto, connectionId) => {
                            DoThrottled(connectionId, (obj) =>
                            {
                                obj.StartMovement(Direction.LEFT);
                            });
                        });
                        AddIncomingMessageHandler("Move:Right", async (proto, connectionId) => {
                            DoThrottled(connectionId, (obj) =>
                            {
                                obj.StartMovement(Direction.RIGHT);
                            });
                        });
                        AddIncomingMessageHandler("Move:Up", async (proto, connectionId) => {
                            DoThrottled(connectionId, (obj) =>
                            {
                                obj.StartMovement(Direction.UP);
                            });
                        });
                    }
                }
            }
        }
    }
}
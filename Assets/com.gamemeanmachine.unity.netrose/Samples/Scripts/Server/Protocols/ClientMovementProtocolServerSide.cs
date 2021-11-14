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
                        var _ = RunInMainThread(() =>
                        {
                            // Which index to take the character from?
                            int index = (clientId % 2 == 1) ? char1index : char2index;
                            // Instantiate it.
                            ObjectServerSide obj = ScopesProtocolServerSide.InstantiateHere((uint)index);
                            // Get the netrose component of it.
                            OwnableModelServerSide ownableObj = obj.GetComponent<OwnableModelServerSide>();
                            // Give it the required connection id.
                            ownableObj.ConnectionId = clientId;
                            // Initialize it in no map.
                            ownableObj.MapObject.Initialize();
                            // Add it to the dictionary.
                            objects[clientId] = new ObjectOwnage() { LastCommandTime = 0, OwnedObject = obj.GetComponent<INetRoseModelServerSide>() };
                            // Attach it to a map.
                            ownableObj.MapObject.Attach(
                                ScopesProtocolServerSide.LoadedScopes[4].GetComponent<Scope>()[0],
                                8, 6, true
                            );
                        });
                    }

                    public override async Task OnDisconnected(ulong clientId, System.Exception reason)
                    {
                        var _ = RunInMainThread(() =>
                        {
                            if (objects.TryGetValue(clientId, out ObjectOwnage ownage))
                            {
                                objects.Remove(clientId);
                                // It will de-spawn and destroy the object.
                                Destroy(ownage.OwnedObject.MapObject);
                            }
                        });
                    }

                    private void DoThrottled(ulong connectionId, Action<MapObject> callback)
                    {
                        ObjectOwnage ownage = objects[connectionId];
                        MapObject obj = ownage.OwnedObject.MapObject;
                        float time = Time.time;
                        if (ownage.LastCommandTime + 0.75 / obj.Speed <= time)
                        {
                            ownage.LastCommandTime = time;
                            callback(obj);
                        }
                    }

                    protected override void SetIncomingMessageHandlers()
                    {
                        AddIncomingMessageHandler("Move:Down", async (proto, connectionId) => {
                            var _ = RunInMainThread(() =>
                            {
                                DoThrottled(connectionId, (obj) =>
                                {
                                    obj.Orientation = Direction.DOWN;
                                    obj.StartMovement(Direction.DOWN);
                                });
                            });
                        });
                        AddIncomingMessageHandler("Move:Left", async (proto, connectionId) => {
                            var _ = RunInMainThread(() =>
                            {
                                DoThrottled(connectionId, (obj) =>
                                {
                                    obj.Orientation = Direction.LEFT;
                                    obj.StartMovement(Direction.LEFT);
                                });
                            });
                        });
                        AddIncomingMessageHandler("Move:Right", async (proto, connectionId) => {
                            var _ = RunInMainThread(() =>
                            {
                                DoThrottled(connectionId, (obj) =>
                                {
                                    obj.Orientation = Direction.RIGHT;
                                    obj.StartMovement(Direction.RIGHT);
                                });
                            });
                        });
                        AddIncomingMessageHandler("Move:Up", async (proto, connectionId) => {
                            var _ = RunInMainThread(() =>
                            {
                                DoThrottled(connectionId, (obj) =>
                                {
                                    obj.Orientation = Direction.UP;
                                    obj.StartMovement(Direction.UP);
                                });
                            });
                        });
                    }
                }
            }
        }
    }
}
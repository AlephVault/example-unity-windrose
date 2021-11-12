using AlephVault.Unity.Binary.Wrappers;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Types;
using GameMeanMachine.Unity.NetRose.Types.Models;
using GameMeanMachine.Unity.NetRose.Types.Protocols;
using GameMeanMachine.Unity.NetRose.Types.Protocols.Messages;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.World;
using GameMeanMachine.Unity.WindRose.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace GameMeanMachine.Unity.NetRose
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Client
            {
                /// <summary>
                ///   A NetRose protocol is tightly related to a <see cref="ScopesProtocolClientSide"/>.
                ///   In this sense, this protocol sends nothing to the server (save for a local error,
                ///   which is the same case of the scopes protocol), but receives updates from the
                ///   server which reflects, now, in a windrose-aware way.
                /// </summary>
                [RequireComponent(typeof(ScopesProtocolClientSide))]
                public class NetRoseProtocolClientSide : ProtocolClientSide<NetRoseProtocolDefinition>
                {
                    /// <summary>
                    ///   The related <see cref="ScopesProtocolClientSide"/>.
                    /// </summary>
                    public ScopesProtocolClientSide ScopesProtocolClientSide { get; private set; }

                    /// <summary>
                    ///   The current NetRose scope. It is null if the current
                    ///   scope object is null, or it is not a NetRoseScopeClientSide
                    ///   (attached) object.
                    /// </summary>
                    public NetRoseScopeClientSide CurrentNetRoseScope { get; private set; }

                    /// <summary>
                    ///   The current map set (i.e. the current WindRose scope).
                    ///   It is null if the current scope is null, or it is not
                    ///   a WindRose's Scope (attached) object.
                    /// </summary>
                    public Scope CurrentMaps { get; private set; }

                    /// <summary>
                    ///   The lag tolerance for the object. It is the maximum
                    ///   number of delayed steps in the objects queue. If more
                    ///   than this number of steps/movement in the per-object
                    ///   queue is queued, then the queue is accelerated to run
                    ///   all of the steps (ultimately causing a bit of bad
                    ///   experience but recovering from the lag).
                    /// </summary>
                    private ushort lagTolerance = 5;

                    /// <summary>
                    ///   See <see cref="lagTolerance"/>.
                    /// </summary>
                    public ushort LagTolerance => lagTolerance;

                    /// <summary>
                    ///   An after-awake setup.
                    /// </summary>
                    protected override void Setup()
                    {
                        lagTolerance = Math.Max(lagTolerance, (ushort)5);
                        ScopesProtocolClientSide = GetComponent<ScopesProtocolClientSide>();
                        ScopesProtocolClientSide.OnMovedToScope += OnMovedToScope;
                    }

                    private void OnDestroy()
                    {
                        ScopesProtocolClientSide.OnMovedToScope -= OnMovedToScope;
                    }

                    private void OnMovedToScope(ScopeClientSide obj)
                    {
                        CurrentNetRoseScope = ScopesProtocolClientSide.CurrentScope == null ? null : ScopesProtocolClientSide.GetComponent<NetRoseScopeClientSide>();
                    }

                    protected override void SetIncomingMessageHandlers()
                    {
                        AddIncomingMessageHandler<ObjectMessage<Attachment>>("Object:Attached", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, async (obj) => {
                                    Map map;
                                    try
                                    {
                                        map = CurrentNetRoseScope.Maps[(int)message.Content.MapIndex];
                                    }
                                    catch (KeyNotFoundException)
                                    {
                                        await ScopesProtocolClientSide.LocalError("UnknownMap");
                                        return;
                                    }

                                    ushort x = message.Content.Position.X;
                                    ushort y = message.Content.Position.Y;

                                    if (!await CheckIsValidMapPosition(map, x, y)) return;

                                    obj.OnAttached(map, x, y);
                                }
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Nothing>>("Object:Detached", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, async (obj) => {
                                    obj.OnDetached();
                                }
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<MovementStart>>("Object:Movement:Started", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, async (obj) => {
                                    ushort x = message.Content.Position.X;
                                    ushort y = message.Content.Position.Y;

                                    if (!await CheckInValidMapPosition(obj, x, y)) return;

                                    obj.OnMovementStarted(x, y, message.Content.Direction);
                                }
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Movement:Cancelled", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, async (obj) => {
                                    ushort x = message.Content.X;
                                    ushort y = message.Content.Y;

                                    if (!await CheckInValidMapPosition(obj, x, y)) return;

                                    obj.OnMovementCancelled(x, y);
                                }
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Movement:Finished", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, async (obj) => {
                                    ushort x = message.Content.X;
                                    ushort y = message.Content.Y;

                                    if (!await CheckInValidMapPosition(obj, x, y)) return;

                                    obj.OnMovementFinished(x, y);
                                }
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Teleported", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, async (obj) => {
                                    ushort x = message.Content.X;
                                    ushort y = message.Content.Y;

                                    if (!await CheckInValidMapPosition(obj, x, y)) return;

                                    obj.OnTeleported(x, y);
                                }
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<UInt>>("Object:Speed:Changed", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, async (obj) => obj.OnSpeedChanged(message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Enum<Direction>>>("Object:Orientation:Changed", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, async (obj) => obj.OnOrientationChanged(message.Content)
                            );
                        });
                    }

                    // Queues an action in the main thread that checks the current pair
                    // scopeId / objectId for validity and executes a particular action,
                    // or raises a LocaError if invalid. It also raises a LocalError if
                    // the current scope is not a NetRose scope.
                    private Task RunInMainThreadValidatingScopeAndObject(uint scopeId, uint objectId, Func<INetRoseMapObjectClientSide, Task> callback)
                    {
                        return RunInMainThread(async () =>
                        {
                            if (!await ScopesProtocolClientSide.RequireIsCurrentScopeAndHoldsObjects(scopeId))
                            {
                                return;
                            }

                            if (CurrentNetRoseScope == null)
                            {
                                await ScopesProtocolClientSide.LocalError("ScopeIsNotNetRose");
                                return;
                            }

                            ObjectClientSide obj = ScopesProtocolClientSide.GetObject(objectId);
                            if (obj == null)
                            {
                                await ScopesProtocolClientSide.LocalError("UnknownObject");
                                return;
                            }

                            INetRoseMapObjectClientSide netRoseObj = obj.GetComponent<INetRoseMapObjectClientSide>();
                            if (netRoseObj == null)
                            {
                                await ScopesProtocolClientSide.LocalError("ObjectIsNotNetRose");
                                return;
                            }

                            await callback(netRoseObj);
                        });
                    }

                    // Checks the position to be valid in the map.
                    private async Task<bool> CheckIsValidMapPosition(Map map, ushort x, ushort y)
                    {
                        if (map.Width <= x || map.Height <= y)
                        {
                            await ScopesProtocolClientSide.LocalError("ObjectPositionOutOfBounds");
                            return false;
                        }

                        return true;
                    }

                    // Checks the object to be in a valid map position.
                    private async Task<bool> CheckInValidMapPosition(INetRoseMapObjectClientSide obj, ushort x, ushort y)
                    {
                        Map map = obj.MapObject.ParentMap;
                        if (map == null)
                        {
                            await ScopesProtocolClientSide.LocalError("ObjectNotInMap");
                            return false;
                        }

                        return await CheckIsValidMapPosition(map, x, y);
                    }
                }
            }
        }
    }
}
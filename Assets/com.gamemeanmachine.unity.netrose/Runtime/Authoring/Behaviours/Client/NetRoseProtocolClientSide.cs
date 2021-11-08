using AlephVault.Unity.Binary.Wrappers;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Types;
using GameMeanMachine.Unity.NetRose.Types.Models;
using GameMeanMachine.Unity.NetRose.Types.Protocols;
using GameMeanMachine.Unity.NetRose.Types.Protocols.Messages;
using GameMeanMachine.Unity.WindRose.Types;
using System;
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
                    ///   An after-awake setup.
                    /// </summary>
                    protected override void Setup()
                    {
                        ScopesProtocolClientSide = GetComponent<ScopesProtocolClientSide>();
                    }

                    protected override void SetIncomingMessageHandlers()
                    {
                        AddIncomingMessageHandler<ObjectMessage<Attachment>>("Object:Attached", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => OnAttached(obj, message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Nothing>>("Object:Detached", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => OnDetached(obj)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<MovementStart>>("Object:Movement:Started", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => OnMovementStarted(obj, message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Movement:Cancelled", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => OnMovementCancelled(obj, message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Movement:Finished", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => OnMovementFinished(obj, message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Teleported", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => OnTeleported(obj, message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<UInt>>("Object:Speed:Changed", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => OnSpeedChanged(obj, message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Enum<Direction>>>("Object:Orientation:Changed", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => OnOrientationChanged(obj, message.Content)
                            );
                        });
                    }

                    // Queues an action in the main thread that checks the current pair
                    // scopeId / objectId for validity and executes a particular action,
                    // or raises a LocaError if invalid.
                    private Task RunInMainThreadValidatingScopeAndObject(uint scopeId, uint objectId, Action<NetRoseMapObjectClientSide> callback)
                    {
                        return RunInMainThread(async () =>
                        {
                            if (!await ScopesProtocolClientSide.RequireIsCurrentScopeAndHoldsObjects(scopeId))
                            {
                                return;
                            }

                            ObjectClientSide obj = ScopesProtocolClientSide.GetObject(objectId);
                            if (obj == null)
                            {
                                await ScopesProtocolClientSide.LocalError("UnknownObject");
                                return;
                            }

                            NetRoseMapObjectClientSide netRoseObj = obj.GetComponent<NetRoseMapObjectClientSide>();
                            if (netRoseObj == null)
                            {
                                await ScopesProtocolClientSide.LocalError("ObjectIsNotNetRose");
                                return;
                            }

                            callback(netRoseObj);
                        });
                    }

                    private void OnAttached(NetRoseMapObjectClientSide obj, Attachment attachment)
                    {
                        // TODO implement.
                    }

                    private void OnDetached(NetRoseMapObjectClientSide obj)
                    {
                        // TODO implement.
                    }

                    private void OnMovementStarted(NetRoseMapObjectClientSide obj, MovementStart movementStart)
                    {
                        // TODO implement.
                    }

                    private void OnMovementCancelled(NetRoseMapObjectClientSide obj, Position position)
                    {
                        // TODO implement.
                    }

                    private void OnMovementFinished(NetRoseMapObjectClientSide obj, Position position)
                    {
                        // TODO implement.
                    }

                    private void OnTeleported(NetRoseMapObjectClientSide obj, Position position)
                    {
                        // TODO implement.
                    }

                    private void OnSpeedChanged(NetRoseMapObjectClientSide obj, uint speed)
                    {
                        // TODO implement.
                    }

                    private void OnOrientationChanged(NetRoseMapObjectClientSide obj, Direction orientation)
                    {
                        // TODO implement.
                    }
                }
            }
        }
    }
}
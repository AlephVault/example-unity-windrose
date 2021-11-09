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
                    }

                    protected override void SetIncomingMessageHandlers()
                    {
                        AddIncomingMessageHandler<ObjectMessage<Attachment>>("Object:Attached", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => obj.OnAttached(message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Nothing>>("Object:Detached", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => obj.OnDetached()
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<MovementStart>>("Object:Movement:Started", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => obj.OnMovementStarted(message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Movement:Cancelled", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => obj.OnMovementCancelled(message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Movement:Finished", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => obj.OnMovementFinished(message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Teleported", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => obj.OnTeleported(message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<UInt>>("Object:Speed:Changed", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => obj.OnSpeedChanged(message.Content)
                            );
                        });
                        AddIncomingMessageHandler<ObjectMessage<Enum<Direction>>>("Object:Orientation:Changed", (proto, message) => {
                            return RunInMainThreadValidatingScopeAndObject(
                                message.ScopeId, message.ObjectId, (obj) => obj.OnOrientationChanged(message.Content)
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
                }
            }
        }
    }
}
using AlephVault.Unity.Binary.Wrappers;
using AlephVault.Unity.Meetgard.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Client;
using AlephVault.Unity.Meetgard.Types;
using GameMeanMachine.Unity.NetRose.Types.Models;
using GameMeanMachine.Unity.NetRose.Types.Protocols;
using GameMeanMachine.Unity.NetRose.Types.Protocols.Messages;
using GameMeanMachine.Unity.WindRose.Types;
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
                            return RunInMainThread(() => OnAttached(message.ScopeId, message.ObjectId, message.Content));
                        });
                        AddIncomingMessageHandler<ObjectMessage<Nothing>>("Object:Detached", (proto, message) => {
                            return RunInMainThread(() => OnDetached(message.ScopeId, message.ObjectId));
                        });
                        AddIncomingMessageHandler<ObjectMessage<MovementStart>>("Object:Movement:Started", (proto, message) => {
                            return RunInMainThread(() => OnMovementStarted(message.ScopeId, message.ObjectId, message.Content));
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Movement:Cancelled", (proto, message) => {
                            return RunInMainThread(() => OnMovementCancelled(message.ScopeId, message.ObjectId, message.Content));
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Movement:Finished", (proto, message) => {
                            return RunInMainThread(() => OnMovementFinished(message.ScopeId, message.ObjectId, message.Content));
                        });
                        AddIncomingMessageHandler<ObjectMessage<Position>>("Object:Teleported", (proto, message) => {
                            return RunInMainThread(() => OnTeleported(message.ScopeId, message.ObjectId, message.Content));
                        });
                        AddIncomingMessageHandler<ObjectMessage<UInt>>("Object:Speed:Changed", (proto, message) => {
                            return RunInMainThread(() => OnSpeedChanged(message.ScopeId, message.ObjectId, message.Content));
                        });
                        AddIncomingMessageHandler<ObjectMessage<Enum<Direction>>>("Object:Orientation:Changed", (proto, message) => {
                            return RunInMainThread(() => {
                                OnOrientationChanged(message.ScopeId, message.ObjectId, message.Content);
                            });
                        });
                    }

                    private NetRoseMapObjectClientSide ValidateScopeAndObject(uint scopeId, uint objectId)
                    {
                        if (scopeId != ScopesProtocolClientSide.CurrentScope.Id)
                        {
                            // throw local error.
                        }
                        return null;
                    }

                    private void OnAttached(uint scopeId, uint objectId, Attachment attachment)
                    {
                        // TODO implement.
                    }

                    private void OnDetached(uint scopeId, uint objectId)
                    {
                        // TODO implement.
                    }

                    private void OnMovementStarted(uint scopeId, uint objectId, MovementStart movementStart)
                    {
                        // TODO implement.
                    }

                    private void OnMovementCancelled(uint scopeId, uint objectId, Position position)
                    {
                        // TODO implement.
                    }

                    private void OnMovementFinished(uint scopeId, uint objectId, Position position)
                    {
                        // TODO implement.
                    }

                    private void OnTeleported(uint scopeId, uint objectId, Position position)
                    {
                        // TODO implement.
                    }

                    private void OnSpeedChanged(uint scopeId, uint objectId, uint speed)
                    {
                        // TODO implement.
                    }

                    private void OnOrientationChanged(uint scopeId, uint objectId, Direction orientation)
                    {
                        // TODO implement.
                    }
                }
            }
        }
    }
}